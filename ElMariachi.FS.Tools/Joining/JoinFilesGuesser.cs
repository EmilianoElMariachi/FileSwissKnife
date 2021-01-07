using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ElMariachi.FS.Tools.Utils;

namespace ElMariachi.FS.Tools.Joining
{
    public static class JoinFilesGuesser
    {
        /// <summary>
        /// Try to guess the list of files to join according to the given file example
        /// </summary>
        /// <param name="exampleFile"></param>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <param name="guessMissingFiles"></param>
        /// <returns></returns>
        public static bool TryGuessFilesToJoin(string exampleFile, out string[] inputFiles, out string outputFile, bool guessMissingFiles = true)
        {
            var exampleFileInfo = new FileInfo(exampleFile);
            var exampleFileDirectory = exampleFileInfo.Directory;
            if (exampleFileDirectory == null || !exampleFileDirectory.Exists)
            {
                inputFiles = new string[0];
                outputFile = "";
                return false;
            }

            var allFiles = exampleFileDirectory.GetFiles();

            var regex = new Regex("\\d+");

            var exampleFileName = exampleFileInfo.Name;

            BestMatchInfo? bestMatchInfo = null;
            foreach (Match numberMatch in regex.Matches(exampleFileName))
            {
                var prefixTmp = exampleFileName.Substring(0, numberMatch.Index);
                var suffixTmp = exampleFileName.Substring(numberMatch.Index + numberMatch.Length);
                var refNumber = numberMatch.Value;
                var matchingFiles = ListMatchingFiles(prefixTmp, refNumber, suffixTmp, allFiles, out var refNumberSeemsPadded);
                if (matchingFiles.Count < 2) // NOTE: At least 2 files needed (the current matching file + another)
                    continue;

                if (bestMatchInfo == null || matchingFiles.Count > bestMatchInfo.MatchingFiles.Count)
                    bestMatchInfo = new BestMatchInfo(matchingFiles, prefixTmp, suffixTmp, refNumber, refNumberSeemsPadded);
            }

            if (bestMatchInfo == null)
            {
                inputFiles = new string[0];
                outputFile = "";
                return false;
            }

            if (guessMissingFiles)
                EnrichWithMissingFiles(bestMatchInfo, exampleFileDirectory.FullName);

            inputFiles = bestMatchInfo.MatchingFiles.Select(m => m.FileInfo.FullName).ToArray();
            outputFile = Path.Combine(exampleFileDirectory.FullName, BuildOutputFileName(bestMatchInfo.Prefix, bestMatchInfo.Suffix, exampleFileDirectory.FullName));
            return true;
        }

        private static void EnrichWithMissingFiles(BestMatchInfo bestMatchInfo, string outputDirPath)
        {
            var a = bestMatchInfo.MatchingFiles.First(); // NOTE: there are at least 2 files in the list (see previous note)

            for (var i = 1; i < bestMatchInfo.MatchingFiles.Count; i++)
            {
                var b = bestMatchInfo.MatchingFiles[i];

                var nbMissingNumbers = b.Number - a.Number - 1;

                var missingNum = a.Number;
                while ((nbMissingNumbers--) > 0)
                {
                    missingNum++;

                    var missingNumStr = bestMatchInfo.RefNumberSeemsPadded ? missingNum.ToPaddedString(bestMatchInfo.RefNumberStr.Length) : missingNum.ToString();
                    var missingFileName = bestMatchInfo.Prefix + missingNumStr + bestMatchInfo.Suffix;
                    var missingFilePath = Path.Combine(outputDirPath, missingFileName);

                    var missingMatchingFile = new MatchingFile(new FileInfo(missingFilePath), missingNumStr, missingNum);
                    bestMatchInfo.MatchingFiles.Insert(i++, missingMatchingFile);
                }

                a = b;
            }
        }

        private static string BuildOutputFileName(string prefix, string suffix, string outputDirPath)
        {
            string outputFileName;

            if (prefix == "" && suffix == "") // Case 123 => Parent dir name : the file names are only composed of numbers, we take the parent directory name as file name
            {
                outputFileName = Path.GetFileName(outputDirPath);
                return outputFileName;
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                // Case <prefix>123<empty>: when no suffix, the file names are ending with a number,
                // therefore we might have extra extension like MyMovie.mp4.part001 or MyMovie.mp4.001

                var parts = prefix.Split('.');

                if (parts.Length >= 3)
                {
                    // Here, we have at least 2 '.'  in the name
                    outputFileName = string.Join('.', parts, 0, parts.Length - 1);
                    return outputFileName;
                }
            }

            if (prefix.EndsWith(".") && suffix.StartsWith("."))
            {
                // Case <prefix>.123.<suffix> => XXX.YYY
                outputFileName = prefix + suffix.Substring(1);
            }
            else if (prefix == "" && suffix.StartsWith("."))
            {
                // Case <empty>123.<suffix> => YYY
                outputFileName = suffix.Substring(1);
            }
            else if (suffix == "" && prefix.EndsWith("."))
            {
                // Case <prefix>.123<empty> => XXX
                outputFileName = prefix.Substring(0, prefix.Length - 1);
            }
            else
            {
                // Case <prefix>.123<suffix>
                // or   <prefix>123.<suffix>
                outputFileName = prefix + suffix;
            }

            return outputFileName;
        }

        private class BestMatchInfo
        {
            public BestMatchInfo(List<MatchingFile> matchingFiles, string prefix, string suffix, string refNumberStr, bool refNumberSeemsPadded)
            {
                MatchingFiles = matchingFiles;
                Prefix = prefix;
                Suffix = suffix;
                RefNumberStr = refNumberStr;
                RefNumberSeemsPadded = refNumberSeemsPadded;
            }

            /// <summary>
            /// The list is sorted by file number
            /// </summary>
            public List<MatchingFile> MatchingFiles { get; }

            public string Prefix { get; }

            public string Suffix { get; }

            public string RefNumberStr { get; }

            public bool RefNumberSeemsPadded { get; }
        }

        private static List<MatchingFile> ListMatchingFiles(string prefix, string refNumber, string suffix, IEnumerable<FileInfo> allFiles, out bool refNumberSeemsPadded)
        {
            var regex = new Regex($"^{Regex.Escape(prefix)}(\\d+){Regex.Escape(suffix)}$");
            var matchingFiles = new Dictionary<long, MatchingFile>();

            refNumberSeemsPadded = refNumber.StartsWith("0");

            foreach (var file in allFiles)
            {
                var match = regex.Match(file.Name);
                if (!match.Success)
                    continue;

                var fileNumStr = match.Groups[1].Value;
                if (!long.TryParse(fileNumStr, out var fileNum))
                    continue;

                var matchingFile = new MatchingFile(file, fileNumStr, fileNum);

                if (!matchingFiles.ContainsKey(fileNum))
                {
                    matchingFiles.Add(fileNum, matchingFile);
                }
                else
                {
                    // Here, a file with the same number is already found.
                    // The strategy is here is to prefer a file with a padded number when reference number is padded
                    // and a non padded number when reference number is not padded
                    if (refNumberSeemsPadded)
                    {
                        if (matchingFile.NumberStr.Length == refNumber.Length)
                            matchingFiles[fileNum] = matchingFile;
                    }
                    else
                    {
                        if (!matchingFile.NumberStr.StartsWith("0"))
                            matchingFiles[fileNum] = matchingFile;
                    }
                }
            }

            var files = matchingFiles.Values.ToList();
            files.Sort((m1, m2) =>
            {
                if (m1.Number == m2.Number)
                    return 0;
                if (m1.Number < m2.Number)
                    return -1;
                return 1;
            });

            return files;
        }

        private class MatchingFile
        {
            public MatchingFile(FileInfo fileInfo, string numberStr, long number)
            {
                FileInfo = fileInfo;
                NumberStr = numberStr;
                Number = number;
            }

            public FileInfo FileInfo { get; }

            public string NumberStr { get; }

            public long Number { get; }
        }
    }
}

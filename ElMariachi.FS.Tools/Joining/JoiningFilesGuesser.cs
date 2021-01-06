using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ElMariachi.FS.Tools.Joining
{
    public static class JoiningFilesGuesser
    {
        /// <summary>
        /// Try to guess the list of files to join according to the given file example
        /// </summary>
        /// <param name="exampleFile"></param>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static bool TryGuessFilesToJoin(string exampleFile, out string[] inputFiles, out string outputFile)
        {
            var exampleFileInfo = new FileInfo(exampleFile);
            var allFiles = exampleFileInfo.Directory?.GetFiles() ?? new FileInfo[0];

            var regex = new Regex("\\d+");

            var exampleFileName = exampleFileInfo.Name;

            BestMatchInfo? bestMatchInfo = null;
            foreach (Match numberMatch in regex.Matches(exampleFileName))
            {
                var prefixTmp = exampleFileName.Substring(0, numberMatch.Index);
                var suffixTmp = exampleFileName.Substring(numberMatch.Index + numberMatch.Length);
                var refNumber = numberMatch.Value;
                var matchingFiles = ListMatchingFiles(prefixTmp, refNumber, suffixTmp, allFiles, out var refNumberSeemsPadded);
                if (matchingFiles.Count <= 0)
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

            inputFiles = EnrichWithMissingFiles(bestMatchInfo).MatchingFiles.Select(m => m.FileInfo.FullName).ToArray();
            outputFile = BuildOutputFile(bestMatchInfo.Prefix, bestMatchInfo.Suffix, exampleFileInfo);
            return true;
        }

        private static BestMatchInfo EnrichWithMissingFiles(BestMatchInfo bestMatchInfo)
        {
            //TODO: finir la fonction d'enrichissement des fichiers manquant
            return bestMatchInfo;
        }

        private static string BuildOutputFile(string prefix, string suffix, FileInfo exampleFileInfo)
        {
            string outputFileName;
            if (prefix == "" && suffix == "") // Case 123 => Parent dir name
            {
                outputFileName = Path.GetFileName(exampleFileInfo.DirectoryName);
            }
            else if (prefix.EndsWith(".") && suffix.StartsWith(".")) // Case AAA.123.BBB => AAA.BBB
            {
                outputFileName = prefix + suffix.Substring(1);
            }
            else if (prefix == "" && suffix.StartsWith(".")) // Case 123.AAA => AAA
            {
                outputFileName = suffix.Substring(1);
            }
            else if (suffix == "" && prefix.EndsWith(".")) // Case AAA.123 => AAA
            {
                outputFileName = prefix.Substring(0, prefix.Length - 1);
            }
            else
            {
                outputFileName = prefix + suffix;
            }

            return Path.Combine(exampleFileInfo.DirectoryName, outputFileName);
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
                if(!long.TryParse(fileNumStr, out var fileNum))
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

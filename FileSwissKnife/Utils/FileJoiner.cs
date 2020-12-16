using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FileSwissKnife.Localization;

namespace FileSwissKnife.Utils
{
    public class FileJoiner : ProgressReporterBase
    {

        private void CheckPrerequisites(string[] inputFiles, string outputFile)
        {
            if (inputFiles == null)
                throw new ArgumentNullException(nameof(inputFiles)); // TODO: à localiser?

            foreach (var inputFile in inputFiles)
            {
                if (string.IsNullOrEmpty(inputFile))
                    throw new ArgumentException(); // TODO: à localiser
            }

            if (string.IsNullOrEmpty(outputFile))
                throw new ArgumentException(Localizer.Instance.OutputFileCantBeUndefined);

            if (!File.Exists(outputFile))
                return;

            var currentMainWindow = Application.Current.MainWindow;
            var messageBoxResult = MessageBox.Show(currentMainWindow, string.Format(Localizer.Instance.CanOverrideOutputFile, outputFile), Localizer.Instance.Override, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes)
                throw new InvalidOperationException(string.Format(Localizer.Instance.YouChooseNotToOverride, outputFile));
        }

        public Task Run(CancellationToken cancellationToken, string[] inputFiles, string outputFile)
        {
            CheckPrerequisites(inputFiles, outputFile);

            return Task.Run(() =>
            {
                try
                {
                    var totalBytes = ComputeTotalBytesSize(inputFiles);

                    var buffer = new byte[4096];

                    using var outStream = File.Create(outputFile);
                    outStream.SetLength(totalBytes);

                    var nbBytesJoined = 0L;

                    foreach (var file in inputFiles)
                    {
                        using var fileStream = File.OpenRead(file);

                        while (true)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var nbBytesRead = fileStream.Read(buffer, 0, buffer.Length);

                            if (nbBytesRead <= 0)
                                break;

                            outStream.Write(buffer, 0, nbBytesRead);
                            nbBytesJoined += nbBytesRead;

                            var percent = (double)(nbBytesJoined * 100 / (decimal)totalBytes);
                            NotifyProgressChanged(percent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(outputFile);
                    }
                    catch
                    {
                    }

                    if (!(ex is OperationCanceledException))
                        throw;
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Try to guess the list of files to join according to the given file example
        /// </summary>
        /// <param name="fileExample"></param>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static bool TryGuessFilesToJoin(string fileExample, out string[]? inputFiles, out string? outputFile)
        {
            inputFiles = null;
            outputFile = null;

            var regex = new Regex("^(.*?)(\\d+)([^\\d]*)$"); // <Anything><Digits><NonDigits>

            var fileName = Path.GetFileName(fileExample);

            var match = regex.Match(fileName);

            if (!match.Success)
                return false;

            var prefix = match.Groups[1].Value.Trim();
            //var digits = match.Groups[2].Value;
            var suffix = match.Groups[3].Value.Trim();


            var dir = Path.GetDirectoryName(fileExample);

            var filesWithNumber = new List<Tuple<string, long>>();

            foreach (var fileTmp in Directory.GetFiles(dir))
            {
                var fileNameTmp = Path.GetFileName(fileTmp);
                var matchTmp = regex.Match(fileNameTmp);
                if (!matchTmp.Success || matchTmp.Groups[1].Value.Trim() != prefix || matchTmp.Groups[3].Value.Trim() != suffix)
                    continue;

                if (long.TryParse(matchTmp.Groups[2].Value, out var fileNum))
                    filesWithNumber.Add(new Tuple<string, long>(fileTmp, fileNum));
            }

            if (filesWithNumber.Count <= 0)
                return false;

            string outputFileName;
            if (prefix == "" && suffix == "") // Case 123 => Parent dir name
            {
                outputFileName = Path.GetFileName(dir);
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

            outputFile = Path.Combine(dir, outputFileName);


            filesWithNumber.Sort((t1, t2) =>
            {
                var (_, fileNum1) = t1;
                var (_, fileNum2) = t2;
                if (fileNum1 == fileNum2)
                    return 0;
                if (fileNum1 < fileNum2)
                    return -1;
                return 1;
            });

            inputFiles = filesWithNumber.Select(tuple => tuple.Item1).ToArray();
            return true;
        }

        private static long ComputeTotalBytesSize(IEnumerable<string> files)
        {
            return files.Sum(file => new FileInfo(file).Length);
        }


    }
}

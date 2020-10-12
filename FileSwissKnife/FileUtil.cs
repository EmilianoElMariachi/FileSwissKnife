﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace FileSwissKnife
{
    public static class FileUtil
    {
        public delegate void ProgressHandler(double percent);

        public static void Join(string[] inputFiles, string outputFile, CancellationToken cancellationToken, ProgressHandler? progressHandler)
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
                        progressHandler?.Invoke(percent);
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    File.Delete(outputFile);
                }
                catch
                {
                }
                throw;
            }

        }

        public static string[] GuessFilesToJoin(string fileExample, out string? outputFile)
        {
            var regex = new Regex("(.*?)(\\d+)([^\\d]*)");

            var fileName = Path.GetFileName(fileExample);

            var match = regex.Match(fileName);

            if (!match.Success)
            {
                outputFile = null;
                return new string[0];
            }

            var prefix = match.Groups[1].Value.Trim();
            //var digits = match.Groups[2].Value;
            var suffix = match.Groups[3].Value.Trim();


            var dir = Path.GetDirectoryName(fileExample);

            var filesWithNumber = new List<Tuple<string, int>>();

            foreach (var fileTmp in Directory.GetFiles(dir))
            {
                var fileNameTmp = Path.GetFileName(fileTmp);
                var matchTmp = regex.Match(fileNameTmp);
                if (matchTmp.Success && matchTmp.Groups[1].Value.Trim() == prefix && matchTmp.Groups[3].Value.Trim() == suffix)
                {
                    filesWithNumber.Add(new Tuple<string, int>(fileTmp, int.Parse(matchTmp.Groups[2].Value)));
                }
            }

            if (filesWithNumber.Count <= 0)
            {
                outputFile = null;
                return new string[0];
            }

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


            filesWithNumber.Sort((t1, t2) => t1.Item2 - t2.Item2);


            return filesWithNumber.Select(tuple => tuple.Item1).ToArray();
        }

        private static long ComputeTotalBytesSize(IEnumerable<string> files)
        {
            return files.Sum(file => new FileInfo(file).Length);
        }
    }
}

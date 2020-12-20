using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileSwissKnife.Utils
{
    public class FileSplitter : ProgressReporterBase
    {

        public Task Split(string inputFile, string outputFolder, long splitSize, CancellationToken ct)
        {
            var fileInfo = new FileInfo(inputFile);

            if (fileInfo.Length <= splitSize)
                throw new InvalidOperationException($"Split size «{splitSize}» is greater than file size «{fileInfo.Length}»."); //TODO: à localiser

            if (splitSize < 0)
                throw new ArgumentOutOfRangeException(nameof(splitSize), "Split size should be strictly greater than 0.");

            return Task.Run(() =>
            {
                var namingOptions = new NamingOptions
                {
                    NumPrefix = ".part",
                    NumPos = NumPos.AfterExt,
                    StartNumber = 1,
                };

                using var parts = PrepareParts(inputFile, outputFolder, splitSize, namingOptions, ct);

                try
                {

                    var buffer = new byte[1000000];
                    var totalBytes = parts.InputStream.Length;
                    var totalBytesRemaining = totalBytes;

                    foreach (var part in parts)
                    {
                        ct.ThrowIfCancellationRequested();

                        using var outputFile = File.Create(part.FilePath);

                        var remainingBytes = part.FileSize;
                        do
                        {
                            ct.ThrowIfCancellationRequested();

                            var nbBytesToRead = (int)Math.Min(remainingBytes, buffer.Length);
                            var nbBytesRead = parts.InputStream.Read(buffer, 0, nbBytesToRead);
                            if (nbBytesRead != nbBytesToRead)
                                throw new Exception("Aïe! Lé où l'problème?");

                            outputFile.Write(buffer, 0, nbBytesRead);

                            var percentage = (double)((totalBytes - totalBytesRemaining) / (decimal)totalBytes * 100);
                            NotifyProgressChanged(percentage, part.FileName);

                            totalBytesRemaining -= nbBytesRead;
                            remainingBytes -= nbBytesRead;
                        } while (remainingBytes > 0);
                    }
                }
                catch (OperationCanceledException)
                {
                    foreach (var part in parts.Where(part => File.Exists(part.FilePath)))
                        File.Delete(part.FilePath);

                    throw;
                }

                //TODO: implémenter la progression
            }, ct);
        }

        private class Parts : List<Part>, IDisposable
        {
            public Parts(string inputFilePath)
            {
                InputStream = File.OpenRead(inputFilePath);
            }

            public Stream InputStream { get; }

            public void Dispose()
            {
                InputStream.Dispose();
            }
        }

        private class Part
        {
            public Part(string filePath, string fileName, long fileSize)
            {
                FilePath = filePath;
                FileName = fileName;
                FileSize = fileSize;
            }

            public string FilePath { get; }

            public string FileName { get; }

            public long FileSize { get; }
        }

        private static Parts PrepareParts(string inputFilePath, string outputFolder, long splitSize, NamingOptions namingOptions, CancellationToken ct)
        {
            var parts = new Parts(inputFilePath);

            var baseName = Path.GetFileNameWithoutExtension(inputFilePath);
            var extension = Path.GetExtension(inputFilePath);
            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            var totalBytesRemaining = parts.InputStream.Length;

            var fileNum = namingOptions.StartNumber;
            while (totalBytesRemaining > 0)
            {
                ct.ThrowIfCancellationRequested();

                var nextFileSize = Math.Min(totalBytesRemaining, splitSize);

                totalBytesRemaining -= nextFileSize;

                var fileName = BuildFileName(baseName, extension, namingOptions, fileNum++);

                var filePath = Path.Combine(outputFolder, fileName);

                parts.Add(new Part(filePath, fileName, nextFileSize));
            }

            return parts;
        }

        private static string BuildFileName(string baseName, string extension, NamingOptions namingOptions, int fileNum)
        {
            var fileNumFull = $"{namingOptions.NumPrefix}{fileNum}{namingOptions.NumSuffix}";

            switch (namingOptions.NumPos)
            {
                case NumPos.BeforeBaseName:
                    return fileNumFull + baseName + "." + extension;
                case NumPos.AfterBaseName:
                    return baseName + fileNumFull + "." + extension;
                case NumPos.BeforeExt:
                    return baseName + "." + fileNumFull + extension;
                case NumPos.AfterExt:
                    return baseName + "." + extension + fileNumFull;
                default:
                    throw new ArgumentOutOfRangeException(nameof(namingOptions.NumPos));
            }
        }

        public class NamingOptions
        {
            public NumPos NumPos { get; set; }

            public string? NumSuffix { get; set; }

            public string? NumPrefix { get; set; }

            public int StartNumber { get; set; }
        }

        public enum NumPos
        {
            /// <summary>
            /// 123README.TXT
            /// </summary>
            BeforeBaseName,
            /// <summary>
            /// README123.TXT
            /// </summary>
            AfterBaseName,
            /// <summary>
            /// README.123TXT
            /// </summary>
            BeforeExt,
            /// <summary>
            /// README.TXT123
            /// </summary>
            AfterExt
        }
    }
}

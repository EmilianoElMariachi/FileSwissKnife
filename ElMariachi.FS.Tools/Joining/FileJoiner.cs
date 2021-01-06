using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElMariachi.FS.Tools.Progression;

namespace ElMariachi.FS.Tools.Joining
{
    public class FileJoiner : ProgressReporterBase
    {
        public const int DefaultBufferSize = 1024 * 1024 * 4;

        public FileJoiner()
        {
        }

        public FileJoiner(int bufferSize)
        {
            if (bufferSize < 1)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer should be greater than or equal to 1.");
            BufferSize = bufferSize;
        }

        public int BufferSize { get; } = DefaultBufferSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException" />
        public Task Run(string[] inputFiles, string outputFile, CancellationToken ct)
        {
            if (inputFiles == null)
                throw new ArgumentNullException(nameof(inputFiles));

            if (outputFile == null)
                throw new ArgumentNullException(nameof(outputFile));

            foreach (var inputFile in inputFiles)
            {
                if (!File.Exists(inputFile))
                    throw new FileNotFoundException($"Input file «{inputFile}» not found.");
            }

            return Task.Run(() =>
            {
                try
                {
                    var totalBytes = ComputeTotalBytesSize(inputFiles);

                    var buffer = new byte[BufferSize];

                    using var outStream = File.Create(outputFile);
                    outStream.SetLength(totalBytes);

                    var nbBytesJoined = 0L;

                    foreach (var file in inputFiles)
                    {
                        using var fileStream = File.OpenRead(file);

                        while (true)
                        {
                            ct.ThrowIfCancellationRequested();

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
                catch
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
            }, ct);
        }

        private static long ComputeTotalBytesSize(IEnumerable<string> files)
        {
            return files.Sum(file => new FileInfo(file).Length);
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElMariachi.FS.Tools.Progression;

namespace ElMariachi.FS.Tools.Hashing
{
    public class FileHasher : ProgressReporterBase
    {

        public const int DefaultBufferSize = 1024 * 1024 * 4;

        public FileHasher()
        {
        }

        public FileHasher(int bufferSize)
        {
            if (bufferSize < 1)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer should be greater than or equal to 1.");
            BufferSize = bufferSize;
        }

        public int BufferSize { get; } = DefaultBufferSize;

        public Task ComputeAsync(string file, IEnumerable<Hash> hashes, CancellationToken ct)
        {
            var tuples = hashes.Select(hash =>
                {
                    var algorithm = HashAlgorithm.Create(hash.AlgorithmName);
                    if (algorithm == null)
                        throw new ArgumentException($"Hash algorithm «{hash}» is unknown.");

                    return new Tuple<HashAlgorithm, Hash>(algorithm, hash);
                })
                .ToList();

            return Task.Run(() =>
            {

                using var fileStream = File.OpenRead(file);

                var buffer = new byte[BufferSize];

                var totalBytes = fileStream.Length;
                var nbBytesHashed = 0L;

                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    var nbBytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    nbBytesHashed += nbBytesRead;

                    if (nbBytesRead <= 0)
                    {
                        foreach (var (hashAlgorithm, hash) in tuples)
                        {
                            ct.ThrowIfCancellationRequested();

                            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                            hash.HexValue = ToHexString(hashAlgorithm.Hash);
                            hash.Value = hashAlgorithm.Hash;
                        }

                        NotifyProgressChanged(100);
                        break;
                    }

                    foreach (var (hashAlgorithm, _) in tuples)
                        hashAlgorithm.TransformBlock(buffer, 0, nbBytesRead, buffer, 0);

                    NotifyProgressChanged((double)((decimal)nbBytesHashed / totalBytes) * 100);

                }

            }, ct);
        }

        private static string ToHexString(IEnumerable<byte> hash)
        {
            var sb = new StringBuilder();

            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}

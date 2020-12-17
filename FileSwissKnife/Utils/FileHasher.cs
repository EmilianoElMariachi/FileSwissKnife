using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSwissKnife.Utils
{
    public class FileHasher : ProgressReporterBase
    {

        private static string ToHexString(IEnumerable<byte> hash)
        {
            var sb = new StringBuilder();

            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        public void CheckPrerequisites()
        {
            //TODO: à implémenter
        }

        public Task ComputeAsync(string file, IEnumerable<Hash> hashes, CancellationToken cancellationToken)
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

                var buffer = new byte[1000000]; // TODO: rendre paramétrable?

                var totalBytes = fileStream.Length;
                var nbBytesHashed = 0L;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nbBytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    nbBytesHashed += nbBytesRead;

                    if (nbBytesRead <= 0)
                    {
                        foreach (var (hashAlgorithm, hash) in tuples)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                            hash.ComputedValue = ToHexString(hashAlgorithm.Hash);
                        }

                        NotifyProgressChanged(100);
                        break;
                    }

                    foreach (var (hashAlgorithm, _) in tuples)
                        hashAlgorithm.TransformBlock(buffer, 0, nbBytesRead, buffer, 0);

                    NotifyProgressChanged((double)((decimal)nbBytesHashed / totalBytes) * 100);

                }

            }, cancellationToken);

        }
    }
}

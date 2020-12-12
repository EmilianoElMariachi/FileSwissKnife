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
            var sBuilder = new StringBuilder();

            foreach (var b in hash)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public void CheckPrerequisites()
        {
            //TODO: à implémenter
        }

        public Task ComputeAsync(CancellationToken cancellationToken, string file, IEnumerable<Hash> hashes)
        {

            var tuples = hashes
                .Select(hash =>
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

                    var nbBytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    nbBytesHashed += nbBytesRead;

                    if (nbBytesRead <= 0)
                    {
                        foreach (var (hashAlgorithm, hash) in tuples)
                        {
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

    public class Hash
    {
        public Hash(string hashAlgorithmName)
        {
            AlgorithmName = hashAlgorithmName ?? throw new ArgumentNullException(nameof(hashAlgorithmName));
        }

        public string AlgorithmName { get; }

        public string ComputedValue { get; set; }
    }
}

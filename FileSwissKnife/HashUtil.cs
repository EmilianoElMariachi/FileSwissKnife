using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileSwissKnife
{
    class HashUtil
    {
        public static string[] ComputeHashes(string file, string[] hashAlgorithmNames)
        {

            var hashAlgorithms = new List<HashAlgorithm>();
            foreach (var hashAlgorithm in hashAlgorithmNames)
            {
                hashAlgorithms.Add(HashAlgorithm.Create(hashAlgorithm) ?? throw new ArgumentException());
            }

            using var fileStream = File.OpenRead(file);

            var buffer = new byte[1000000];

            while (true)
            {

                var nbBytesRead = fileStream.Read(buffer, 0, buffer.Length);

                if (nbBytesRead <= 0)
                    break;

                foreach (var hashAlgorithm in hashAlgorithms)
                {
                    hashAlgorithm.TransformBlock(buffer, 0, nbBytesRead, buffer, 0);
                }
            }

            var hashes = new string[hashAlgorithmNames.Length];
            for (var i = 0; i < hashAlgorithms.Count; i++)
            {
                var hashAlgorithm = hashAlgorithms[i];
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                hashes[i] = ToHexString(hashAlgorithm.Hash);
            }

            return hashes;
        }

        private static string ToHexString(byte[] hash)
        {
            var sBuilder = new StringBuilder();

            foreach (var b in hash)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            return sBuilder.ToString();
        }



    }
}

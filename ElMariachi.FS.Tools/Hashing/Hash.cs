using System;

namespace ElMariachi.FS.Tools.Hashing
{
    public class Hash
    {
        public Hash(string hashAlgorithmName)
        {
            AlgorithmName = hashAlgorithmName ?? throw new ArgumentNullException(nameof(hashAlgorithmName));
        }

        public string AlgorithmName { get; }

        public string HexValue { get; set; }

        public byte[] Value { get; set; }
    }
}
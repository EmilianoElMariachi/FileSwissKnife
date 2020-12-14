using System;

namespace FileSwissKnife.Utils
{
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
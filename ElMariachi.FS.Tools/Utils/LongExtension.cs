using System;

namespace ElMariachi.FS.Tools.Utils
{
    public static class LongExtension
    {

        public static string ToPaddedString(this long number, int padding, char paddingChar = '0')
        {
            var fileNumStr = number.ToString();

            var nbMissingZeros = Math.Max(0, padding - fileNumStr.Length);
            fileNumStr = new string(paddingChar, nbMissingZeros) + fileNumStr;

            return fileNumStr;
        }

    }
}

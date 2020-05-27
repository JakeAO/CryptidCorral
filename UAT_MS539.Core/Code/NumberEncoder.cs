using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code
{
    public static class NumberEncoder
    {
        public const string Base2Encoding = "01";
        public const string Base8Encoding = "01234567";
        public const string Base10Encoding = "0123456789";
        public const string Base16Encoding = "0123456789ABCDEF";
        public const string Base24Encoding = "0123456789ABCDEFGHIJKLMN";

        public static string Encode(long number, in string encoding)
        {
            if (string.IsNullOrWhiteSpace(encoding))
                throw new ArgumentNullException($"Provided encoding \"{encoding}\" is invalid because it is null or empty.");
            if (encoding.Contains(' '))
                throw new ArgumentException($"Provided encoding \"{encoding}\" is invalid because it contains a space.");

            uint eLength = (uint) encoding.Length;

            List<char> encodedChars = new List<char>(10);
            long rem = 0;
            do
            {
                rem = number % eLength;
                number /= eLength;

                encodedChars.Add(encoding[(int) rem]);
            } while (number > 0);

            encodedChars.Reverse();
            return string.Join(null, encodedChars);
        }

        public static bool TryEncode(long number, in string encoding, out string result)
        {
            try
            {
                result = Encode(number, encoding);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static long Decode(string number, in string encoding)
        {
            if (string.IsNullOrWhiteSpace(encoding))
                throw new ArgumentNullException($"Provided encoding \"{encoding}\" is invalid because it is null or empty.");
            if (encoding.Contains(' '))
                throw new ArgumentException($"Provided encoding \"{encoding}\" is invalid because it contains a space.");

            uint eLength = (uint) encoding.Length;

            long result = 0;
            for (int i = 0; i < number.Length; i++)
            {
                char encodedChar = number[number.Length - i - 1];
                int charIndex = encoding.IndexOf(encodedChar);
                if (charIndex < 0)
                    throw new ArgumentException($"Character '{encodedChar}' does not appear in the given encoding \"{encoding}\".");

                uint charValue = (uint) encoding.IndexOf(encodedChar);
                result += charValue * (long) Math.Pow(eLength, i);
            }

            return result;
        }

        public static bool TryDecode(string number, in string encoding, out long result)
        {
            try
            {
                result = Decode(number, encoding);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}
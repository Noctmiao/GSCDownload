using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidanceStsbilityCommsDownLoad
{
    public static class Converter
    {
        private static char[] hexDigits = {
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        public static char[] Separators = { ' ', '\t', '\r', '\n' };

        /// <summary>
        /// converts a byte array to a Hex string
        /// 3 11 9 -> 0011 1011 1001 -> 3B9
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return String.Empty;

            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        /// <summary>
        /// adds spaces to the hex string for each 2 hex digits
        /// </summary>
        /// <param name="hexString">hex string with or without spaces</param>
        /// <returns>hex string with spaces</returns>
        public static String AddSpacesToHexString(String hexString)
        {
            if (hexString == String.Empty) return String.Empty;

            hexString = hexString.Replace(" ", "");
            int length = hexString.Length;
            // add spaces between bytes
            for (int i = 1; i < length / 2; i++)
            {
                hexString = hexString.Insert(3 * i - 1, " ");
            }

            return hexString;
        }

        /// <summary>
        /// convert a HEX string to byte array
        /// "1F 09 00 08 05 00 2A 85" --> 31 9 0 8 5 0 42 133
        /// </summary>
        /// <param name="hexString">each two HEX digits are separated by space</param>
        public static byte[] HEXStringToBytes(String hexString)
        {
            String binString = HEXStringToBinaryString(hexString);
            byte[] bytes = BinaryStringToBytes(binString);
            return bytes;
        }
        /// <summary>
        /// convert a HEX string to Binary string, each 8 bits are separated by space
        /// "This is a test" -->
        /// </summary>
        /// <param name="s">each two HEX digits are separated by space</param>
        public static String HEXStringToBinaryString(String s)
        {
            if (validateHexString(s) == false) return String.Empty;

            String[] twoHexDigits = s.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            foreach (String twoHexDigit in twoHexDigits)
            {
                String temp = twoHexDigit.ToUpper();
                int v = BaseToDecimal(temp, 16);
                String binaryString = DecimalToBase(v, 2);
                while (binaryString.Length < 8) binaryString = '0' + binaryString;
                if (sb.Length > 0) sb.Append(" ");
                sb.Append(binaryString);
            }

            return sb.ToString();
        }

        /// <summary>
        /// converts a binary string to a byte array
        /// 011 1011 1001 -> 3 185
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] BinaryStringToBytes(String s)
        {
            if (s == String.Empty || s.Length == 0) return null;
            s = s.Replace(" ", "");
            int n = s.Length / 8;
            if (s.Length % 8 > 0) n++;
            byte[] bytes = new byte[n];

            for (int i = n - 1; i >= 0; i--)
            {
                int pos = s.Length - (n - i) * 8;
                if (pos < 0) pos = 0;
                String ss = s.Substring(pos, 8);
                bytes[i] = (byte)BaseToDecimal(ss, 2);
            }

            return bytes;
        }
        /// <summary>
        /// converts a string (representing a number with given base) to an integer.
        /// the string can NOT be too long (see param length)
        /// </summary>
        /// <param name="input">length
        ///     LE 32 if nBase=2
        ///     LE 20 if nBase=3
        ///     LE 16 if nBase=4
        ///     LE 10 if nBase=8
        ///     LE 9  if nBase=10
        ///     LE 8  if nBase=12
        ///     LE 8  if nBase=16
        /// </param>
        /// <param name="nBase">LE 16</param>
        /// <returns></returns>
        public static int BaseToDecimal(string input, int nBase)
        {
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            if (nBase < 2 || nBase > characters.Length)
            {
                throw new ArgumentOutOfRangeException("nBase", nBase, "Range: 2.." + characters.Length);
            }
            int result = 0;
            bool negative = false;
            if (input.StartsWith("-"))
            {
                negative = true;
                input = input.Substring(1);
            }
            for (int i = 0; i < input.Length; i++)
            {
                int value = characters.IndexOf(input[i]);
                if (value >= nBase || value < 0)
                {
                    throw new ArgumentOutOfRangeException("input[" + i + "]", input[i], "This character is not valid for base " + nBase);
                }
                result += value * (int)Math.Pow(nBase, input.Length - i - 1);
            }
            if (negative)
            {
                result *= -1;
            }
            return result;
        }
        /// <summary>
        /// converts an integer to a string with given base
        /// </summary>
        /// <param name="input"></param>
        /// <param name="nBase">LE 16</param>
        /// <returns></returns>
        public static string DecimalToBase(int input, int nBase)
        {
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            if (nBase < 2 || nBase > characters.Length)
            {
                throw new ArgumentOutOfRangeException("nBase", nBase, "Range: 2.." + characters.Length);
            }
            if (input == 0)
            {
                return characters[0].ToString();
            }
            bool negative = false;
            if (input < 0)
            {
                negative = true; input *= -1;
            }
            StringBuilder sb = new StringBuilder();
            while (input != 0)
            {
                sb.Insert(0, (characters[input % nBase]));
                input /= nBase;
            }
            if (negative)
            {
                sb.Insert(0, "-");
            }
            return sb.ToString();
        }
        /// <summary>
        /// converts a byte array of size 4 to an integer
        /// assume [0] lsb, and [3] msb
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int BytesToInt(byte[] data, int start)
        {
            if (data == null || data.Length < 4) return int.MinValue;
            if (start < 0 || data.Length - start < 4) return int.MinValue;

            // working code
            int pos = start;
            int r = data[pos] + data[pos + 1] * 256 + data[pos + 2] * 256 * 256 + data[pos + 3] * 256 * 256 * 256;
            //r = data[pos] * 256 * 256 * 256 + data[pos + 1] * 256 * 256 + data[pos + 2] * 256 + data[pos + 3];

            //int r = BitConverter.ToInt32(data, start);
            return r;
        }
        /// <summary>
        /// converts a byte array of size 2 to a short integer
        /// assume [0] lsb, and [1] msb
        /// </summary>
        public static short BytesToShort(byte[] data, int start)
        {
            if (data == null || data.Length < 2) return short.MinValue;
            if (start < 0 || data.Length - start < 2) return short.MinValue;

            //int pos = start;
            //int r = data[pos] + data[pos + 1] * 256;

            short r = BitConverter.ToInt16(data, start);
            return r;
        }
        /// <summary>
        /// converts a unsigned short integer to a byte array of size 2,
        /// lsb to [0], msb to [1], equivalent to BitConverter.GetBytes(input)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] UShortToBytes(ushort input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            return bytes;
        }
        #region private methods
        /// <summary>
        /// a HEX string contains a number of two HEX digits separated by space
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool validateHexString(String s)
        {
            bool valid = true;

            if (s == null || s == String.Empty) return true;
            //int length = s.Length;

            // check each two hex digits is separated by a space
            String[] twoHexDigits = s.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (String twoHexDigit in twoHexDigits)
            {
                if (twoHexDigit.Length != 1 && twoHexDigit.Length != 2)
                {
                    valid = false;
                    break;
                }
                else
                {
                    if (isHexDigit(twoHexDigit[0]) == false ||
                        (twoHexDigit.Length == 2 && isHexDigit(twoHexDigit[1])) == false)
                    {
                        valid = false;
                        break;
                    }
                }
            }

            return valid;
        }
        private static bool isHexDigit(char c)
        {
            c = char.ToUpper(c);
            foreach (char d in hexDigits)
            {
                if (d == c) return true;
            }

            return false;
        }
        public static uint BytesToUInt(byte[] data, int start)
        {
            if (data == null || data.Length < 4) return uint.MinValue;
            if (start < 0 || data.Length - start < 4) return uint.MinValue;

            //// working code
            //int pos = start;
            //uint r = (uint)(data[pos] + data[pos + 1] * 256 + data[pos + 2] * 256 * 256 + data[pos + 3] * 256 * 256 * 256);
            ////r = data[pos] * 256 * 256 * 256 + data[pos + 1] * 256 * 256 + data[pos + 2] * 256 + data[pos + 3];

            uint r = BitConverter.ToUInt32(data, start);
            return r;
        }
        #endregion
    }
}

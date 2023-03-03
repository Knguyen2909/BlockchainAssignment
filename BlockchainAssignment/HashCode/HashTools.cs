using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace BlockchainAssignment.HashCode
{
    public static class HashTools
    {
        /// <summary>
        /// Takes byte array and returns hexadecimal string
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        /// <param name="hex"></param>
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string combineHash(string hash1, string hash2)
        {
            byte[] bytes1 = StringToByteArray(hash1);
            byte[] bytes2 = StringToByteArray(hash2);

            SHA256 hashSys = SHA256.Create();
            byte[] combinedbytes = hashSys.ComputeHash(bytes1.Concat(bytes2).ToArray());

            return ByteArrayToString(combinedbytes);
        }
    }
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utility {

    public sealed class CryptographyUtility {

        // 1024   位
        private static byte[] DES_KEY = { 31, 206, 132, 43, 86, 221, 68, 235, 21, 229, 104, 29, 80, 219, 38, 199, 47, 199, 177, 175, 214, 165, 37, 120, 54, 168, 160, 102, 153, 225, 165, 154, 133, 193, 227, 147, 156, 44, 121, 168, 12, 89, 230, 65, 71, 201, 162, 91, 161, 187, 155, 36, 60, 43, 67, 227, 185, 180, 108, 48, 125, 102, 160, 197, 25, 244, 59, 214, 110, 157, 252, 109, 19, 177, 205, 161, 180, 27, 185, 31, 26, 225, 9, 63, 102, 190, 128, 103, 63, 130, 194, 49, 106, 91, 113, 148, 184, 32, 40, 82, 142, 204, 237, 4, 191, 45, 17, 139, 93, 161, 46, 157, 251, 76, 21, 155, 92, 97, 95, 91, 115, 212, 150, 235, 218, 202, 3, 183, 64, 56, 233, 147, 243, 102, 145, 78, 244, 5, 128, 56, 172, 245, 9, 157, 83, 36, 54, 29, 70, 213, 55, 87, 193, 204, 222, 185, 53, 1, 41, 244, 42, 89, 74, 37, 184, 155, 170, 148, 186, 198, 166, 87, 225, 39, 174, 126, 202, 249, 152, 182, 147, 229, 113, 106, 0, 145, 210, 38, 135, 215, 25, 243, 220, 139, 14, 83, 170, 228, 229, 183, 122, 162, 98, 127, 46, 224, 118, 153, 30, 96, 239, 59, 104, 115, 96, 68, 232, 223, 36, 38, 201, 131, 12, 227, 172, 179, 122, 85, 24, 90, 211, 230, 20, 224, 59, 241, 130, 242, 59, 32, 27, 56, 8, 39, 232, 222, 139, 105, 42, 201, 107, 202, 192, 190, 238, 247, 150, 127, 210, 123, 102, 32, 129, 195, 140, 9, 39, 135, 55, 111, 194, 245, 164, 4, 10, 145, 122, 228, 251, 206, 40, 17, 42, 78, 239, 9, 37, 85, 242, 68, 91, 2, 31, 192, 186, 251, 182, 68, 155, 222, 182, 3, 50, 187, 7, 117, 38, 188, 180, 93, 124, 177, 132, 4, 83, 84, 245, 50, 212, 131, 227, 210, 149, 242, 20, 191, 214, 227, 74, 15, 218, 54, 73, 28, 24, 37, 223, 111, 70, 122, 226, 105, 160, 148, 200, 169, 126, 27, 109, 101, 5, 132, 112, 24, 251, 227, 109, 63, 167, 72, 158, 67, 114, 164, 131, 70, 104, 104, 235, 46, 117, 134, 235, 89, 160, 121, 49, 1, 42, 107, 64, 187, 118, 228, 9, 85, 197, 49, 32, 52, 183, 160, 200, 254, 219, 159, 247, 251, 17, 130, 64, 22, 47, 63, 239, 127, 152, 22, 119, 129, 177, 108, 20, 59, 7, 227, 204, 141, 154, 45, 128, 55, 71, 182, 118, 213, 189, 237, 126, 1, 130, 53, 240, 168, 42, 42, 139, 55, 244, 138, 189, 174, 185, 158, 130, 75, 139, 140, 246, 247, 210, 106, 97, 227, 1, 9, 206, 120, 200, 26, 193, 170, 152, 118, 255, 123, 124, 108, 17, 220, 88, 149, 152, 16, 111, 210, 126, 168, 52, 42, 73, 48, 247, 134, 46, 23, 212, 12, 167, 33, 92, 18, 110, 218, 200, 18, 21, 67, 159, 6, 207, 30, 123, 25, 159, 60, 209, 80, 116, 46, 138, 37, 68, 158, 208, 145, 179, 18, 71, 231, 167, 111, 197, 240, 128, 69, 39, 190, 71, 92, 188, 59, 9, 45, 91, 43, 181, 221, 230, 241, 70, 131, 152, 120, 150, 54, 206, 208, 9, 55, 95, 3, 251, 183, 95, 95, 89, 138, 153, 90, 21, 19, 9, 23, 113, 79, 28, 23, 66, 165, 219, 214, 43, 86, 161, 35, 215, 77, 89, 73, 119, 87, 181, 16, 253, 185, 63, 81, 205, 251, 210, 42, 68, 140, 220, 51, 122, 129, 6, 70, 26, 182, 141, 99, 92, 137, 216, 165, 22, 59, 136, 11, 49, 80, 227, 189, 94, 248, 25, 184, 15, 202, 73, 71, 211, 176, 146, 158, 121, 110, 169, 86, 70, 18, 46, 192, 202, 81, 180, 116, 224, 154, 59, 145, 115, 198, 249, 243, 169, 30, 212, 129, 103, 77, 55, 80, 108, 69, 144, 171, 246, 57, 43, 168, 146, 49, 197, 30, 33, 34, 243, 12, 68, 165, 215, 116, 142, 184, 211, 13, 182, 158, 245, 91, 153, 39, 238, 248, 219, 221, 83, 132, 192, 8, 125, 167, 111, 147, 254, 65, 0, 218, 210, 134, 42, 200, 61, 194, 117, 194, 193, 180, 113, 131, 244, 88, 67, 208, 15, 107, 66, 204, 67, 67, 207, 111, 76, 59, 39, 48, 5, 251, 185, 146, 12, 220, 204, 111, 48, 209, 234, 44, 130, 24, 28, 158, 18, 61, 20, 37, 99, 159, 132, 147, 254, 52, 151, 142, 183, 187, 123, 1, 155, 69, 189, 197, 251, 223, 119, 24, 140, 212, 159, 61, 129, 22, 73, 42, 39, 48, 92, 15, 39, 2, 205, 101, 119, 38, 147, 215, 68, 61, 183, 116, 85, 232, 145, 61, 91, 86, 163, 50, 198, 251, 152, 142, 255, 70, 17, 232, 234, 109, 102, 134, 94, 108, 217, 152, 119, 87, 78, 213, 76, 159, 212, 78, 28, 43, 232, 152, 133, 11, 206, 209, 141, 41, 72, 43, 105, 62, 101, 49, 20, 171, 237, 215, 215, 49, 21, 232, 15, 248, 197, 3, 8, 205, 230, 39, 39, 73, 114, 212, 93, 121, 98, 222, 158, 148, 249, 53, 35, 174, 47, 57, 241, 28, 192, 160, 178, 252, 119, 235, 21, 255, 137, 141, 70, 202, 201, 191, 67, 33, 3, 31, 204, 145, 212, 155, 73, 14, 56, 66, 55, 224, 197, 97, 201, 140, 17, 235, 13, 70, 116, 42, 210, 134, 191, 93, 240, 212, 152, 211, 203, 57, 241, 1, 170, 15, 108, 96, 72, 37, 87, 163, 196, 52, 179, 53, 58, 40, 196, 185, 184, 222, 178, 252, 172, 216, 76, 70, 209, 118 };
        private static byte[] DES_8KEY = { 21, 229, 104, 29, 80, 219, 38, 19 };
        // 9*6=54 位
        //private static string CONFUSION = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ01";
        private const char ALL_ZERO = 'X';

        private CryptographyUtility() { }

        /// <summary>
        /// 加密指定的字符串
        /// 字符串加密后将不可还原
        /// </summary>
        /// <param name="input">要加密的字符串</param>
        /// <returns></returns>
        public static string ToMD5(string input) {
            if (input == null)
                throw new ArgumentNullException("input");

            byte[] dataToHash = StringUtility.ConventToByteArray(input);
            byte[] hashvalue = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(dataToHash);
            return BitConverter.ToString(hashvalue).Replace("-", string.Empty);
        }

        /// <summary>
        /// 加密指定的字符串
        /// 字符串加密后将不可还原
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode">加密时使用的编码</param>
        /// <returns></returns>
        public static string ToMD5(string input, Encoding encode) {
            if (input == null)
                throw new ArgumentNullException("input");

            byte[] dataToHash = encode.GetBytes(input);
            byte[] hashvalue = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(dataToHash);
            return BitConverter.ToString(hashvalue).Replace("-", string.Empty);
        }


        public static string Encrypt(string input) {
            return Encrypt(input, null);
        }

        public static string Decrypt(string input) {
            return Decrypt(input, null);
        }

        public static string Encrypt(string input, byte[] key) {
            if (key == null) { key = (byte[])DES_KEY.Clone(); }
            string s = _Encrypt(input, key);
            //Array.Reverse(key);
            //return _Encrypt(s, key);
            return s;
        }

        public static string Decrypt(string input, byte[] key) {
            if (key == null) { key = (byte[])DES_KEY.Clone(); }
            //byte[] key2 = (byte[])key.Clone();
            //Array.Reverse(key2);
            //string s = _Decrypt(input, key2);
            return _Decrypt(input, key);
        }

        private static string _Encrypt(string input, byte[] key) {
            // 加密一个字符串，方法如下
            // 统计字符串字符总数（10位数） 记为 part0
            //      part0的第一位数标记补充0的数量
            //      如一个加密后的字串 0000000125...
            //      表示为 7125
            //
            // 记录每个char转换成数字后的位数（最少1位，最多5位）+ 4 并排列成行，记为 part1
            // 将所有字符转换成char对应的数字+desKey[索引] 并排类成行，记为 part2
            // part4 为密钥MD5
            // 结果为 part0 + part1 + part3 + part4
            int length = input.Length;
            int zeroLength = 10 - length.ToString().Length;
            string part0;
            if (zeroLength == 10) {
                part0 = ALL_ZERO.ToString();
            } else {
                part0 = zeroLength.ToString() + length.ToString();
            }
            //string part0 = input.Length.ToString().PadLeft(10, '0');
            StringBuilder part2 = new StringBuilder(length);
            StringBuilder part3 = new StringBuilder();
            int tempInt; int keyCount = key.Length;
            int index = 0;
            foreach (char c in input) {
                tempInt = (int)c;
                tempInt += key[index]; // 加入偏移
                part2.Append(tempInt.ToString().Length + key[index] % 4);
                part3.Append(tempInt);
                index++;
                if (index > keyCount - 1)
                    index = 0;
            }
            string part4 = GetMD5Part(key, part2.ToString(), part3.ToString()); // MD5串
            int part4Len = part4.Length;
            zeroLength = 10 - part4Len.ToString().Length;
            //Console.WriteLine("{0} {1} {2} {3} {4} {5}", part0, part2, part3, part4, part4Len, zeroLength);
            return part0 + part2.ToString() + part3.ToString() + part4 + part4Len + zeroLength;
        }

        private static string _Decrypt(string input, byte[] key) {
            const string ERROR = "Input string is not a valid DigitalEncrypt string.";
            try {
                char zeroCount = input[0];
                int length = 0; // 获得解密后的字符串长度
                string part1 = null;
                if (zeroCount != ALL_ZERO) {
                    part1 = input.Substring(1, 10 - Convert.ToInt32(zeroCount.ToString()));
                    length = int.Parse(part1);
                }
                if (length == 0) return string.Empty;
                //string part1 = input.Substring(0, 10);
                //int length = int.Parse(part1.TrimStart('0')); // 获得解密后的字符串长度
                string part2 = input.Substring(part1.Length + 1, length);
                // 获取MD5串
                int totalLength = input.Length;
                zeroCount = input[totalLength - 1];
                //totalLength += (10 - Convert.ToInt32(zeroCount.ToString()) + 1);
                int paddingLen = Convert.ToInt32(zeroCount.ToString()); // 末尾补0数
                int part4Len = Convert.ToInt32(input.Substring(totalLength - (10 - paddingLen) - 1, 10 - paddingLen));
                string part4 = input.Substring(totalLength - part4Len - (10 - paddingLen) - 1, part4Len);
                string part3 = input.Substring(part1.Length + 1 + length, (totalLength - part4Len - (10 - paddingLen) - 1) - (part1.Length + 1 + length));

                // 验证密钥
                bool keyMatched = GetMD5Part(key, part2, part3) == part4;
                if (!keyMatched) {
                    throw new FormatException(ERROR);
                }

                int tempLength;
                int index = 0; // 解密进度索引
                int keyIndex = 0;
                int keyCount = key.Length;
                string tempString;
                StringBuilder result = new StringBuilder(length);
                foreach (char lengthC in part2) {
                    tempLength = int.Parse(lengthC.ToString()) - (key[keyIndex] % 4); // 获取当前Char转换成int后占位长度
                    tempString = part3.Substring(index, tempLength);
                    result.Append((char)(int.Parse(tempString) - key[keyIndex]));
                    index += tempLength;
                    keyIndex++;
                    if (keyIndex > keyCount - 1)
                        keyIndex = 0;
                }
                return result.ToString();
            } catch {
                throw new FormatException(ERROR);
            }
        }

        private static string GetMD5Part(byte[] key, string indexPart, string contentPart) {
            //Console.WriteLine("IndexPart:{0}\nContentPart:{1}\n\n", indexPart, contentPart);
            string md5 = ToMD5(ArrayToString(key) + indexPart + contentPart);
            int total = 0;
            int keyCount = key.Length;
            int index = 0;
            foreach (char c in md5) {
                //sb.Append((int)c);
                total += ((int)c + key[index]);
                index++;
                if (index > keyCount - 1)
                    index = 0;
            }
            //Console.WriteLine(total);
            return total.ToString();
        }

        /// <summary> 
        /// DES加密 
        /// </summary> 
        /// <param name="encryptString"></param> 
        /// <returns></returns> 
        public static string DesEncrypt(string encryptString) {
            // byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] keyIV = DES_8KEY;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyIV, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary> 
        /// DES解密 
        /// </summary> 
        /// <param name="decryptString"></param> 
        /// <returns></returns> 
        public static string DesDecrypt(string decryptString) {
            //byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] keyIV = DES_8KEY;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyIV, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        private static string ArrayToString(byte[] array) {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in array) {
                sb.Append(b);
            }
            return sb.ToString();
        }


    }
}

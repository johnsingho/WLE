using System;
using System.Collections;
using System.Text;

namespace Common.Utility {
    public sealed class StringUtility {

        public static string ToBase64(string input) {
            byte[] bytes = ConventToByteArray(input);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(string input) {
            byte[] bytes = Convert.FromBase64String(input);
            return ConventToString(bytes);
        }

        public static int GetCharRepeatTimes(char which, string source, int maxValue, bool ignoreCase) {
            if (source == null)
                throw new ArgumentNullException("input");

            int result = 0;
            foreach (char c in source) {
                if (result >= maxValue)
                    break;
                if (string.Compare(c.ToString(), which.ToString(), ignoreCase) == 0) {
                    result++;
                }
            }
            return result;
        }

        public static int GetCharRepeatTimes(char which, string source, int maxValue) {
            return GetCharRepeatTimes(which, source, maxValue, false);
        }

        public static int GetCharRepeatTimes(char which, string source, bool ignoreCase) {
            return GetCharRepeatTimes(which, source, int.MaxValue, ignoreCase);
        }

        public static int GetCharRepeatTimes(char which, string source) {
            return GetCharRepeatTimes(which, source, int.MaxValue, false);
        }

        /// <summary>
        /// 获取一个值，该值指示指定的字符是否含有中文字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsChineseCharIn(string input) {
            if (string.IsNullOrEmpty(input))
                return false;

            int temp;
            foreach (char c in input) {
                temp = (int)c;
                if (c >= 0X4e00 && c <= 0X9fa5)
                    return true;
            }
            return false;
        }

        public static byte[] ConventToByteArray(string input) {
            return
                new UnicodeEncoding().GetBytes(input);
        }

        public static string ConventToString(byte[] byteArray) {
            return
                new UnicodeEncoding().GetString(byteArray);
        }

        /// <summary>
        /// 将一个数字转换成字符串，如 ,a,b,c,d,
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToArrayString(ICollection array) {
            if (array == null) return null;
            if (array.Count < 1) return string.Empty;
            const char SP = ',';
            StringBuilder sb = new StringBuilder();
            foreach (object o in array) {
                sb.Append(SP);
                sb.Append(o.ToString());
            }
            sb.Append(SP);
            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <param name="sp">分隔符</param>
        /// <returns></returns>
        public static string[] ToStringArray(string input, params string[] sp) {
            if (string.IsNullOrEmpty(input)) { return new string[0]; }
            return input.Split(sp, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 将逗号分割的字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <returns></returns>
        public static string[] ToStringArray(string input) {
            //if (string.IsNullOrEmpty(input)) { return new string[0]; }
            //return input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return ToStringArray(input, ",");
        }

        /// <summary>
        /// 将字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <param name="sp">分割符</param>
        /// <returns></returns>
        public static int[] ToInt32Array(string input, params string[] sp) {
            string[] arr = ToStringArray(input, sp);
            int[] result = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                result[i] = Convert.ToInt32(arr[i]);
            }
            return result;
        }

        /// <summary>
        /// 将逗号分割的字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <returns></returns>
        public static int[] ToInt32Array(string input) {
            //string[] arr = ToStringArray(input);
            //int[] result = new int[arr.Length];
            //for (int i = 0; i < arr.Length; i++) {
            //    result[i] = Convert.ToInt32(arr[i]);
            //}
            //return result;
            return ToInt32Array(input, ",");
        }

        /// <summary>
        /// 将字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <param name="sp">分隔符</param>
        /// <returns></returns>
        public static long[] ToInt64Array(string input, params string[] sp) {
            string[] arr = ToStringArray(input, sp);
            long[] result = new long[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                result[i] = Convert.ToInt64(arr[i]);
            }
            return result;
        }

        /// <summary>
        /// 将逗号分割的字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <returns></returns>
        public static long[] ToInt64Array(string input) {
            //string[] arr = ToStringArray(input);
            //long[] result = new long[arr.Length];
            //for (int i = 0; i < arr.Length; i++) {
            //    result[i] = Convert.ToInt64(arr[i]);
            //}
            //return result;
            return ToInt64Array(input, ",");
        }

        /// <summary>
        /// 将字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <param name="sp">分隔符</param>
        /// <returns></returns>
        public static decimal[] ToDecimalArray(string input, params string[] sp) {
            string[] arr = ToStringArray(input, sp);
            decimal[] result = new decimal[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                result[i] = Convert.ToDecimal(arr[i]);
            }
            return result;
        }

        /// <summary>
        /// 将逗号分割的字符串转换为数组，如无数据，返回空集合
        /// </summary>
        /// <param name="input">字符串，如 ,a,b,c,d,</param>
        /// <returns></returns>
        public static decimal[] ToDecimalArray(string input) {
            //string[] arr = ToStringArray(input);
            //decimal[] result = new decimal[arr.Length];
            //for (int i = 0; i < arr.Length; i++) {
            //    result[i] = Convert.ToDecimal(arr[i]);
            //}
            //return result;
            return ToDecimalArray(input, ",");
        }

        public static string LimitStr(string sin, int nMax)
        {
            if (string.IsNullOrEmpty(sin)) { return sin; }
            var nLen = sin.Length;
            return sin.Substring(0, nMax > nLen ? nLen : nMax);
        }
    }
}

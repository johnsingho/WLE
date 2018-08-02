using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.DotNetCode
{
    /// <summary>
    /// DateTimeHelper
    /// By H.Z.XIN
    /// Modified:
    ///     2018-08-01 整理
    /// 
    /// </summary>
    public class DateTimeHelper
    {
        public static string GetToday()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static string GetLocalTimeStr(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string GetLocalDateStr(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        public static string GetLocalDateStrNull(DateTime? dt)
        {
            if(null==dt || !dt.HasValue) { return string.Empty; }
            return dt.Value.ToString("yyyy-MM-dd");
        }
        public static string GetLocalDateStrNull(object obj)
        {
            var dat = (DateTime)obj;
            if (obj == null)
            {
                return string.Empty;
            }
            return dat.ToString("yyyy-MM-dd");
        }

    }
}

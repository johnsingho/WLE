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
    ///     2018-08-23 整理
    ///     2018-11-28 add date equal
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
        public static bool EquStr(DateTime dt, string symd)
        {
            DateTime dtemp = default(DateTime);
            DateTime.TryParse(symd, out dtemp);
            return (dt.Year == dtemp.Year
                    && dt.Month==dtemp.Month
                    && dt.Day== dtemp.Day
                );
        }

        //用于可能是日期字符串的情况
        public static string GetLocalDateStrNull(object obj)
        {
            if(null==obj || obj == DBNull.Value)
            {
                return string.Empty;
            }
            var stim = obj.ToString();
            var tim = default(DateTime);
            if(DateTime.TryParse(stim, out tim))
            {
                return tim.ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        public static string GetLocalDateTimeStrNull(object obj)
        {
            if (null == obj || obj == DBNull.Value)
            {
                return string.Empty;
            }
            var stim = obj.ToString();
            var tim = default(DateTime);
            if (DateTime.TryParse(stim, out tim))
            {
                return tim.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return string.Empty;
        }

    }
}

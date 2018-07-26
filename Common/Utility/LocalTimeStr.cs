using System;

namespace Common.Utility
{
    public class LocalTimeStr
    {
        public static string GetLocalTimeStr(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string GetLocalDateStr(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
    }
}
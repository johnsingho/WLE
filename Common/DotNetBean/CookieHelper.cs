using System;
using System.Web;

namespace Common.DotNetBean
{
    public class CookieHelper
    {
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes((double)expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static string GetCookie(string strName)
        {
            string result;
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                result = HttpContext.Current.Request.Cookies[strName].Value.ToString();
            }
            else
            {
                result = "";
            }
            return result;
        }
    }
}
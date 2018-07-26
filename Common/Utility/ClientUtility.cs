using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;

namespace Common.Utility {

    /// <summary>
    /// �ͻ��������ʵ����
    /// </summary>
    public sealed class ClientUtility {

        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        private readonly HttpBrowserCapabilities browser = null;
        private const string MOBILE_DEVICES = @"(nokia|sonyericsson|blackberry|samsung|sec\-|windows ce|motorola|mot\-|up.b|midp\-)";

        public ClientUtility() {
            browser = HttpContext.Current.Request.Browser;
        }

        /// <summary>
        /// ��ȡ��Ψһ��ʶ��ǰ��������ַ���
        /// </summary>
        /// <returns></returns>
        public string GetUnique() {
            string s = string.Format(
                "{0}_{1}_{2}",
                GetName(),
                GetVersion(),
                GetID()
                );
            return s.Replace(' ', '_');
        }

        /// <summary>
        /// ��ȡ��ǰ����������ƣ�
        /// ��IE��Firefox��
        /// </summary>
        /// <returns></returns>
        public string GetName() {
            try {
                return browser.Browser;
            }
            catch(Exception) {
                return null;
            }
        }

        /// <summary>
        /// ��ȡ�ͻ��˵��Ե�IP, �κ�����������쳣������null
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP() {
            try {
                return HttpContext.Current.Request.UserHostAddress;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// Get mac address
        /// </summary>
        /// <param name="remoteip"></param>
        /// <returns></returns>
        static public Int64 GetClientMac() {
            var ipAddress = GetClientIP();
            Int32 ldest = inet_addr(ipAddress);
            try {
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                return macinfo;
            }
            catch(Exception err) {
                Console.WriteLine("error:{0}", err.Message);
            }
            return 0;
        }


        /// <summary>
        /// ��ȡ�ͻ��˵��Ե�����
        /// �κ�����������쳣������null
        /// </summary>
        /// <returns></returns>
        public static string GetClientName(bool withDomain) {
            try {
                string name = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.UserHostAddress).HostName;
                if(withDomain) { return name; }
                string[] names = name.Split('.');
                if(names.Length > 0) {
                    return names[0];
                }
                return name;
            }
            catch {
                return null;
            }
        }



        /// <summary>
        /// ��ȡ�ͻ��˵��Ե�����
        /// �κ�����������쳣������null
        /// </summary>
        /// <returns></returns>
        public static string GetClientName() {
            return GetClientName(true);
        }

        /// <summary>
        /// ��ȡ����������汾��
        /// </summary>
        /// <returns></returns>
        public int GetMajorVersion() {
            try {
                return browser.MajorVersion;
            }
            catch(Exception) {
                return -1;
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ��������Ƿ����ֻ��ͻ���
        /// </summary>
        public static bool IsMobileDevice {
            get {
                HttpContext context = HttpContext.Current;
                if(context != null) {
                    HttpRequest request = context.Request;
                    if(request.Browser.IsMobileDevice)
                        return true;

                    System.Text.RegularExpressions.Regex MobileRegex = new System.Text.RegularExpressions.Regex(MOBILE_DEVICES, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    if(!string.IsNullOrEmpty(request.UserAgent) && MobileRegex.IsMatch(request.UserAgent))
                        return true;
                }

                return false;
            }
        }

        private string GetVersion() {
            try {
                return browser.Version;
            }
            catch(Exception) {
                return null;
            }
        }

        private string GetID() {
            try {
                return browser.Id;
            }
            catch(Exception) {
                return null;
            }
        }

    }
}

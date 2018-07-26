using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Common.Utility {
    public sealed class WebUtility {

        /// <summary>
        /// 获取当前 Web 站点的虚拟根路径
        /// </summary>
        public static string SiteVirtualRoot {
            get {
                string root = null;
                try {
                    root = HttpContext.Current.Request.ApplicationPath;
                } catch (NullReferenceException) {
                    root = "/";
                }
                if (root.EndsWith("/")) {
                    return root;
                }
                return root + "/";
            }
        }

        /// <summary>
        /// 获取当前 Web 站点的物理根路径
        /// </summary>
        public static string SitePhysicalRoot {
            get {
                string root = null;
                try {
                    root = HttpContext.Current.Request.PhysicalApplicationPath;
                } catch (NullReferenceException) {
                    // 不在WEB环境内
                    // 使用当前程序集路径
                    root = Environment.CurrentDirectory;
                    var dir = new DirectoryInfo(root);
                    root = dir.Parent.FullName;
                }
                if (root.EndsWith("\\")) {
                    return root;
                }
                return root + "\\";
            }
        }

        /// <summary>
        /// 将指定的Cookie写入客户端
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expires"></param>
        /// <param name="domain"></param>
        public static void WriteCookie(string name, string value, DateTime expires, string domain) {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(name.ToUpper());
            cookie.Value = HttpContext.Current.Server.UrlEncode(value);
            if (!string.IsNullOrEmpty(domain)) {
                cookie.Domain = domain;
            }
            cookie.Path = Utility.WebUtility.SiteVirtualRoot;
            cookie.Expires = expires;
            context.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 将指定的Cookie写入客户端
        /// </summary>
        /// <param name="name">Cookie的名称</param>
        /// <param name="value">Cookie的值</param>
        /// <param name="expires">Cookie失效时间</param>
        public static void WriteCookie(string name, string value, DateTime expires) {
            WriteCookie(name, value, expires, null);
        }

        /// <summary>
        /// 将指定的Cookie写入客户端
        /// 这个Cookie值将在浏览器关闭后自动失效
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="domain"></param>
        public static void WriteCookie(string name, string value, string domain) {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(name.ToUpper());
            cookie.Value = HttpContext.Current.Server.UrlEncode(value);
            cookie.Path = Utility.WebUtility.SiteVirtualRoot;
            if (!string.IsNullOrEmpty(domain)) {
                cookie.Domain = domain;
            }
            context.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 将指定的Cookie写入客户端
        /// 这个Cookie值将在浏览器关闭后自动失效
        /// </summary>
        /// <param name="name">Cookie的名称</param>
        /// <param name="value">Cookie的值</param>
        public static void WriteCookie(string name, string value) {
            //HttpContext context = HttpContext.Current;
            //HttpCookie cookie = new HttpCookie(name.ToUpper());
            //cookie.Value = HttpContext.Current.Server.UrlEncode(value);
            //cookie.Path = Utility.WebUtility.SiteVirtualRoot;
            //context.Response.AppendCookie(cookie);
            WriteCookie(name, value, null);
        }

        /// <summary>
        /// 获取指定的Cookie值
        /// 该值可能为 Null
        /// </summary>
        /// <param name="name">要获取的Cookie值的名称(不区分大小写)</param>
        /// <returns></returns>
        public static string GetCookie(string name) {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies[name.ToUpper()];
            if (cookie != null)
                return HttpContext.Current.Server.UrlDecode(cookie.Value);
            return null;
        }

        public static void RemoveCookie(string name) {
            WriteCookie(name, string.Empty, DateTime.Now.AddDays(-1));
        }

        /// <summary>
        /// 获取当前页面的URL
        /// 如果URL被重写，返回重写后的URL
        /// </summary>
        /// <returns></returns>
        public static string GetRawUrl() {
            string url = HttpContext.Current.Request.RawUrl;
            int index = url.ToUpper().IndexOf("DEFAULT.ASPX");
            if (index > -1)
                url = url.Remove(index, 12);
            return url;
        }

        /// <summary>
        /// 获取当前页面的URL路径部分，不包含任何的查询字串
        /// </summary>
        /// <returns></returns>
        public static string GetURLPath() {
            string url = HttpContext.Current.Request.Url.AbsolutePath;
            int index = url.ToUpper().IndexOf("DEFAULT.ASPX");
            if (index > -1)
                url = url.Remove(index, 12);
            return url;
        }

        /// <summary>
        /// 将指定的文件流写入Response供客户端下载
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="resource"></param>
        /// <param name="fileName">文件名（需含扩展名）</param>
        public static void WriteFile(Stream sourceFile, long contentLength, HttpResponse response, string fileName) {
            WriteFile("Application/octet-stream", sourceFile, contentLength, response, fileName);
        }

        public static void WriteFile(string contentType, Stream sourceFile, long contentLength, HttpResponse response, string fileName) {
            using (sourceFile) {
                // 构建HTTP头
                response.Cache.SetCacheability(HttpCacheability.Public);
                response.Cache.SetLastModified(DateTime.Now);
                response.ContentType = contentType;
                //response.AddHeader("Content-Disposition", "attachment;filename=" + HttpContext.Current.Server.UrlEncode(fileName));
                fileName = fileName.Replace(' ', '_');
                fileName = HttpContext.Current.Request.Browser.IsBrowser("IE") ? HttpContext.Current.Server.UrlEncode(fileName) : fileName;
                response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                response.AddHeader("Content-Length", contentLength.ToString());

                // 写入文件
                int readCount;
                int buffer = 16384;
                byte[] bytes = new Byte[buffer];

                readCount = sourceFile.Read(bytes, 0, buffer);
                while (readCount > 0) {
                    if (response.IsClientConnected) {
                        response.OutputStream.Write(bytes, 0, readCount);
                        response.Flush();
                        readCount = sourceFile.Read(bytes, 0, buffer);
                    } else {
                        readCount = -1;
                    }
                }
                response.End();
            }
        }

        /// <summary>
        /// 把用户的输入转化为可以在页面安全显示的HTML字符
        /// 并删除首尾的空格
        /// </summary>
        /// <param name="input"></param>
        /// <param name="removeHTMLTags">是否要删除字符中的HTML标签</param>
        /// <returns></returns>
        public static string GetSafeText(string input, bool removeHTMLTags) {
            if (removeHTMLTags) {
                //input = Regex.Replace(input, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//<br>
                //input = Regex.Replace(input, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//&nbsp;
                input = Regex.Replace(input, "<(.|\\n)*?>", string.Empty);	//any other tags
            }
            return HttpContext.Current.Server.HtmlEncode(input.Trim());
        }

        /// <summary>
        /// 将一个C#字符串转化为可以在页面显示的字符
        /// 并保留其格式
        /// </summary>
        /// <param name="normalText"></param>
        /// <returns></returns>
        public static string ToHTMLString(string normalText) {
            normalText = normalText.Replace("\n", "<br/>");
            normalText = HttpContext.Current.Server.HtmlEncode(normalText);
            normalText = normalText.Replace(" ", "&nbsp;");
            normalText = normalText.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            return normalText;
        }

    }
}

using System;
using System.Web;
using System.Web.UI;

namespace Common.Utility
{
    public class WebPageScriptHelper
    {
        // scriptBody sample: "showTipsMsg('{0}','2500','4');top.main.windowload();OpenClose();"
        public static void ExecuteScript(System.Web.UI.Page page, string scriptBody, string scriptkey="")
        {
            string sk = scriptkey;
            if (string.IsNullOrEmpty(scriptkey))
            {
                var newgid = Guid.NewGuid().ToString();
                sk = string.Format("scr_{0}", newgid);
            }
            Page p = HttpContext.Current.Handler as Page;
            p.ClientScript.RegisterStartupScript(typeof(string), sk, scriptBody, true);
        }
    }
}

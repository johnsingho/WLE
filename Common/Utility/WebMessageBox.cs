using System;
using System.Collections.Generic;
using System.Web;


namespace Common.Utility
{
    public class WebMessageBox {
        public static void Show(System.Web.UI.Page page, string message)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "msg", "BootstrapDialog.alert(\"" + message + "\");", true);
        }

        public static void ShowSuccess(System.Web.UI.Page page, string message, string redirectUrl)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "msg", "ShowSuccessDialog(\"" + message + "\",\"" + redirectUrl + "\");", true);
        }

        public static void ShowSuccess(System.Web.UI.Page page, string message)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "msg", "BootstrapDialog.success(\"" + message + "\");", true);
        }

        public static void Confirm(System.Web.UI.Page page, string message, string okCB)
        {
            var scall = string.Format(@"BootstrapDialog.confirm('{0}',{1});", message, okCB);
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "msg", scall, true);
        }
    }
}
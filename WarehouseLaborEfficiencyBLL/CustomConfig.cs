using System.Configuration;

namespace WarehouseLaborEfficiencyBLL
{
    public class CustomConfig
    {
        public static string ConnStrMain;
        public static bool bDeleteTempFile;

        static CustomConfig()
        {
            ConnStrMain = ConfigurationManager.ConnectionStrings["main_ConnStr"].ConnectionString;
            var sTemp = ConfigurationManager.AppSettings["DeleteTempFile"].Trim();
            bool.TryParse(sTemp, out bDeleteTempFile);
        }
    }
}

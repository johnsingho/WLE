using Common;
using System;
using System.IO;
using System.Web;
using WarehouseLaborEfficiencyBLL;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    public class BLLHelper
    {
        public static bool ImpUpload(string dataType, HttpPostedFileBase file, out string sErrImp)
        {
            sErrImp = string.Empty;
            var fileName = Path.GetFileName(file.FileName);
            var fileTemp = Path.Combine(GetTempDirBase(), fileName);
            file.SaveAs(fileTemp);
            
            long nRec = 0;
            try
            {
                var fi = new FileInfo(fileTemp);
                switch (dataType)
                {
                    case "WeekData":
                        nRec = WLE_Data.ImportWeekData(fi, out sErrImp);
                        break;
                    case "MonthData":
                        nRec = WLE_Data.ImportMonthData(fi, out sErrImp);
                        break;
                    case "HCData":
                        nRec = WLE_Data.ImportHCData(fi, out sErrImp);
                        break;
                }
                
                LogHelper.WriteInfo(typeof(BLLHelper), string.Format("DoUploadCond:{0}", nRec));
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(BLLHelper), ex);
                sErrImp = string.Format("Excel数据有问题。<br>{0}", ex.Message);
            }
            finally
            {
                if (CustomConfig.bDeleteTempFile)
                {
                    File.Delete(fileTemp);
                }
            }
            return (nRec > 0);
        }


        private static string GetTempDirBase()
        {
            var spath = System.Web.Hosting.HostingEnvironment.MapPath("~/temp");
            if (!System.IO.Directory.Exists(spath))
            {
                System.IO.Directory.CreateDirectory(spath);
            }
            return spath;
        }
        

    }
}
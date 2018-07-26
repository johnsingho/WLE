using System;
using System.IO;
using System.Web;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    public class BLLHelper
    {
        public static bool ImpUpload(HttpPostedFileBase file, out string sErrImp)
        {
            sErrImp = string.Empty;
            var fileName = Path.GetFileName(file.FileName);
            var fileTemp = Path.Combine(GetTempDirBase(), fileName);
            file.SaveAs(fileTemp);

            var recvCmpDal = new WLE_DAL(CustomConfig.ConnStrBaan, CustomConfig.ConnStrMain);
            int nRec = 0;
            try
            {
                nRec = recvCmpDal.DoImpUpload(new FileInfo(fileTemp), out sErrImp);
                LogHelper.WriteInfo(typeof(BLLHelper), string.Format("DoUploadCond:{0}", nRec));
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(BLLHelper), ex);
                sErrImp = ex.Message;
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

        internal static bool SyncBaanData(out string sErrSync)
        {
            sErrSync = string.Empty;
            var recvCmpDal = new CommodityRecv_DAL(CustomConfig.ConnStrBaan, CustomConfig.ConnStrMain);
            int nRec = 0;
            try
            {
                nRec = recvCmpDal.DoSyncData(out sErrSync);
                LogHelper.WriteInfo(typeof(CommodityRecv_DAL), string.Format("SyncBaanData:{0}", nRec));
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(CommodityRecv_DAL), ex);
                sErrSync = ex.Message;
            }
            return (nRec > 0);
        }


    }
}
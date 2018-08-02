using System;
using System.Web;
using System.Web.Mvc;
using WarehouseLaborEfficiencyWeb.DAL;

namespace WarehouseLaborEfficiencyWeb.Controllers
{
    public class QueryController : Controller
    {
        private string GetTempDirBase()
        {
            var spath = HttpContext.Server.MapPath("~/temp"); ;
            if (!System.IO.Directory.Exists(spath))
            {
                System.IO.Directory.CreateDirectory(spath);
            }
            return spath;
        }


        [HttpPost]
        public ActionResult GetWeekDataInitInfo()
        {
            var lstData = QueryHelper.GetWarehouseList();
            var obj = new TRes
            {
                bok = true,
                data = lstData,
                extra = QueryHelper.GetWeekdateList(),
            };

            if(null==lstData || 0 == lstData.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }

        [HttpPost]
        public ActionResult QueryWeekData(string bu, string startDate, string endDate)
        {
            var datWeek = QueryHelper.GetWeekData(bu, startDate, endDate);
            var obj = new TRes
            {
                bok = true,
                data = datWeek
            };
            if (null==datWeek.data || 0==datWeek.columns.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }
        

        [HttpPost]
        public ActionResult UploadData(HttpPostedFileBase file)
        {
            var res = new TRes
            {
                bok = false,
                msg = ""
            };
            if (!ModelState.IsValid)
            {
                res.msg = "数据无效";
                ModelState.AddModelError("", res.msg);
                return Json(res, JsonRequestBehavior.AllowGet);
            }

            if (file == null || file.ContentLength == 0)
            {
                res.msg = "文件有问题";
                ModelState.AddModelError("", res.msg);
                return Json(res, JsonRequestBehavior.AllowGet);
            }

            var sErrImp = string.Empty;
            bool bImp = BLLHelper.ImpUpload(file, out sErrImp);
            res.bok = bImp;
            if (!bImp)
            {
                res.msg = sErrImp;
                ModelState.AddModelError("", res.msg);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


    }
}
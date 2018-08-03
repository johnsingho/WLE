﻿using Common.DotNetExcel;
using System;
using System.Web;
using System.Web.Mvc;
using WarehouseLaborEfficiencyBLL;
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

        #region WeekData
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
        
        #endregion

        #region Month Data
        [HttpPost]
        public ActionResult GetMonthDataInitInfo()
        {
            var lstData = QueryHelper.GetWarehouseList();
            var obj = new TRes
            {
                bok = true,
                data = lstData,
                extra = QueryHelper.GetMonthdateList(),
            };

            if (null == lstData || 0 == lstData.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }

        [HttpPost]
        public ActionResult QueryMonthData(string bu, string startDate, string endDate)
        {
            var dat = QueryHelper.GetMonthData(bu, startDate, endDate);
            var obj = new TRes
            {
                bok = true,
                data = dat
            };
            if (null == dat.data || 0 == dat.columns.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }
        #endregion

        #region HCData        
        [HttpPost]
        public ActionResult QueryHCData(string bu)
        {
            var dat = QueryHelper.GetHCData(bu);
            var obj = new TRes
            {
                bok = true,
                data = dat
            };
            if (null == dat.data || 0 == dat.columns.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }
        #endregion

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


        [HttpGet]
        public ActionResult DownloadData(string dType, string bu, string startWeek, string endWeek)
        {
            switch (dType)
            {
                case "WeekData":
                    return DownloadData_WeekData(bu, startWeek, endWeek);
                case "MonthData":
                    return DownloadData_MonthData(bu, startWeek, endWeek);
                case "HCData":
                    return DownloadData_HCData(bu, startWeek, endWeek);
            }
            return new EmptyResult();
        }

        private ActionResult DownloadData_WeekData(string bu, string startWeek, string endWeek)
        {
            var fn = string.Format("{0}_{1}_{2}.xlsx", "WeekData", startWeek, endWeek);
            var bys = WLE_Data.GetWeekData_Down(bu, startWeek, endWeek);
            return File(bys, ExcelType.XLSX_MIME, fn);
        }

        private ActionResult DownloadData_MonthData(string bu, string startWeek, string endWeek)
        {
            var fn = string.Format("{0}_{1}_{2}.xlsx", "MonthData", startWeek, endWeek);
            var bys = WLE_Data.GetMonthData_Down(bu, startWeek, endWeek);
            return File(bys, ExcelType.XLSX_MIME, fn);
        }

        private ActionResult DownloadData_HCData(string bu, string startWeek, string endWeek)
        {
            return null;
        }

    }
}
using Common.DotNetCode;
using Common.DotNetExcel;
using System;
using System.Collections.Generic;
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

        public ActionResult GetWarehouseInfo()
        {
            var lstData = QueryHelper.GetWarehouseList();
            var obj = new TRes
            {
                bok = true,
                data = lstData
            };

            if (null == lstData || 0 == lstData.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }

        #region WeekData
        [HttpPost]
        public ActionResult GetWeekDataInitInfo()
        {
            var lstData = QueryHelper.GetWarehouseList();
            var obj = new TRes
            {
                bok = true,
                data = lstData
            };

            try
            {
                obj.extra = QueryHelper.GetWeekdateList();
            }
            catch (Exception ex)
            {
                obj.bok = false;
                obj.msg = "数据有问题:"+ex.Message;
                //throw;
            }

            if (null==lstData || 0 == lstData.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }

        [HttpPost]
        public ActionResult QueryWeekData(string bu, string startDate, string endDate)
        {
            var obj = new TRes
            {
                bok = true
            };

            try
            {
                var datWeek = QueryHelper.GetWeekData(bu, startDate, endDate);
                obj.data = datWeek;
                if (null == datWeek.data || 0 == datWeek.columns.Count)
                {
                    obj.bok = false;
                    obj.msg = "没有查询到数据";
                }
            }
            catch (Exception ex)
            {
                obj.bok = false;
                obj.msg = "数据有问题:" + ex.Message;
            }

            return Json(obj);
        }

        [HttpPost]
        public ActionResult QueryWeekDataAll(string date, string warehouse)
        {
            var obj = new TRes
            {
                bok = true
            };

            try
            {
                var datWeek = QueryHelper.QueryWeekDataAll(date, warehouse);
                obj.data = datWeek;
                //if (0 == datWeek.Count)
                //{
                //    obj.bok = false;
                //    obj.msg = "没有查询到数据";
                //}
            }
            catch (Exception ex)
            {
                obj.bok = false;
                obj.msg = "数据有问题:" + ex.Message;
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
        public ActionResult QueryMonthData(string selKind)
        {
            var dat = QueryHelper.GetMonthData(selKind);
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

        [HttpPost]
        public ActionResult QueryMonthDataAll(string date, string warehouse)
        {
            var obj = new TRes
            {
                bok = true
            };

            try
            {
                var dat = QueryHelper.QueryMonthDataAll(date, warehouse);
                obj.data = dat;
                //if (0 == datWeek.Count)
                //{
                //    obj.bok = false;
                //    obj.msg = "没有查询到数据";
                //}
            }
            catch (Exception ex)
            {
                obj.bok = false;
                obj.msg = "数据有问题:" + ex.Message;
            }

            return Json(obj);
        }
        #endregion

        #region HCData        
        [HttpPost]
        public ActionResult QueryHCData(string warehouses)
        {
            var dat = QueryHelper.GetHCData(warehouses);
            var obj = new TRes
            {
                bok = true,
                data = dat
            };

            if (0==dat.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }

        [HttpPost]
        public ActionResult QueryHCDataAll(string date, string warehouse)
        {
            var obj = new TRes
            {
                bok = true
            };

            try
            {
                var dat = QueryHelper.QueryHCDataAll(date, warehouse);
                obj.data = dat;
                //if (0 == datWeek.Count)
                //{
                //    obj.bok = false;
                //    obj.msg = "没有查询到数据";
                //}
            }
            catch (Exception ex)
            {
                obj.bok = false;
                obj.msg = "数据有问题:" + ex.Message;
            }

            return Json(obj);
        }
        
        #endregion

        #region HCRate
        [HttpPost]
        public ActionResult GetHCRateMonth()
        {
            var lstData = QueryHelper.GetHCRateMonth();
            var obj = new TRes
            {
                bok = true,
                data = lstData
            };

            if (null == lstData || 0 == lstData.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }

        [HttpPost]
        public ActionResult QueryHCRate(string selMonth, string warehouses)
        {
            var dat = QueryHelper.QueryHCRate(selMonth, warehouses);
            var obj = new TRes
            {
                bok = true,
                data = dat
            };

            if (0 == dat.Count)
            {
                obj.bok = false;
                obj.msg = "没有查询到数据";
            }
            return Json(obj);
        }
        #endregion

        [HttpPost]
        public ActionResult UploadData(string dataType, HttpPostedFileBase file)
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
            if (!CommonInfo.HasRight(TRightID.UPLOAD))
            {
                res.msg = "没有上传权限";
                ModelState.AddModelError("", res.msg);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(dataType))
            {
                res.msg = "请选择数据类型";
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
            bool bImp = BLLHelper.ImpUpload(dataType, file, out sErrImp);
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
            if (!CommonInfo.HasRight(TRightID.DOWNLOAD))
            {
                return new HttpUnauthorizedResult();
            }
            switch (dType)
            {
                case "WeekData":
                    return DownloadData_WeekData(bu, startWeek, endWeek);
                //case "MonthData":
                //    return DownloadData_MonthData(bu, startWeek, endWeek);
                case "HCData":
                    return DownloadData_HCData(bu);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult DownloadMonthData(string selKind)
        {
            if (!CommonInfo.HasRight(TRightID.DOWNLOAD))
            {
                return new HttpUnauthorizedResult();
            }
            var fn = string.Format("{0}_{1}.xlsx", "MonthData", selKind);
            var bys = WLE_Data.GetMonthData_Down(selKind);
            if (null == bys) { return new EmptyResult(); }
            return File(bys, ExcelType.XLSX_MIME, fn);
        }


        private ActionResult DownloadData_WeekData(string bu, string startWeek, string endWeek)
        {
            var fn = string.Format("{0}_{1}_{2}.xlsx", "WeekData", startWeek, endWeek);
            var bys = WLE_Data.GetWeekData_Down(bu, startWeek, endWeek);
            if (null == bys) { return new EmptyResult(); }
            return File(bys, ExcelType.XLSX_MIME, fn);
        }
        
        private ActionResult DownloadData_HCData(string bu)
        {
            var fn = string.Format("{0}_{1}.xlsx", "HCData", DateTimeHelper.GetToday());
            var bys = WLE_Data.GetHCData_Down(bu);
            if (null == bys) { return new EmptyResult(); }
            return File(bys, ExcelType.XLSX_MIME, fn);
        }

        #region 数据编辑         
        [HttpPost]
        public ActionResult DeleteWeekData(IEnumerable<int> ids)
        {
            var sErr = string.Empty;
            var bRet = QueryHelper.DeleteWeekData(ids, out sErr);
            var obj = new TRes
            {
                bok = bRet,
                msg = sErr
            };
            return Json(obj);
        }
        [HttpPost]
        public ActionResult DeleteMonthData(IEnumerable<int> ids)
        {
            var sErr = string.Empty;
            var bRet = QueryHelper.DeleteMonthData(ids, out sErr);
            var obj = new TRes
            {
                bok = bRet,
                msg = sErr
            };
            return Json(obj);
        }
        [HttpPost]
        public ActionResult DeleteHCData(IEnumerable<int> ids)
        {
            var sErr = string.Empty;
            var bRet = QueryHelper.DeleteHCData(ids, out sErr);
            var obj = new TRes
            {
                bok = bRet,
                msg = sErr
            };
            return Json(obj);
        }

        #endregion


    }
}
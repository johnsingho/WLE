using System;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using WarehouseLaborEfficiencyWeb.DAL;
using Common.Utility;

namespace WarehouseLaborEfficiencyWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("CommodityRecv");
        }
        
        private string GetTempDirBase()
        {            
            var spath = HttpContext.Server.MapPath("~/temp"); ;
            if (!System.IO.Directory.Exists(spath))
            {
                System.IO.Directory.CreateDirectory(spath);
            }
            return spath;
        }

        public ActionResult CommodityRecv()
        {
            //TODO
            return View();
        }
        
        [HttpPost]
        public ActionResult GetCompResult()
        {
            var lst = QueryHelper.GetCompResult();
            var res = from x in lst                      
                      select new
                      {
                          PONUMBER = x.PONUMBER,
                          ITEM = x.ITEM,
                          QTY = !x.Qty.HasValue ? 0 : x.Qty.Value,
                          UNIT = x.UNIT,
                          Receiver = x.Receiver,
                          RECEIVEDATE = !x.RECEIVEDATE.HasValue ? "" : LocalTimeStr.GetLocalDateStr(x.RECEIVEDATE.Value)
                      };
            return Json(res);
        }

        [HttpPost]
        public ActionResult GetConditions()
        {
            var lst = QueryHelper.GetConditions();
            return Json(lst);
        }
        
        [HttpPost]
        public ActionResult DelCondition(int id, string CostItemNumber)
        {
            var res = new TRes
            {
                bok = false,
                msg = ""
            };
            res.bok = QueryHelper.DelCondition(id, CostItemNumber);
            return Json(res);
        }

        [HttpPost]
        public ActionResult UploadCond(HttpPostedFileBase file)
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

        [HttpPost]
        public ActionResult SingleCondAdd(string txtCommodityCode, string txtCostItemNumber, string txtCommodityCodeDesc)
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

            if (string.IsNullOrEmpty(txtCostItemNumber))
            {
                res.msg = "Cost Item Number是必填项!";
                ModelState.AddModelError("", res.msg);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var sErr = string.Empty;
            res.bok = QueryHelper.SingleCondAdd(txtCommodityCode, txtCostItemNumber, txtCommodityCodeDesc, out sErr);
            res.msg = sErr;
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReSyncBaan()
        {            
            var res = new TRes
            {
                bok = false,
                msg = ""
            };
            var sErr = string.Empty;
            res.bok = QueryHelper.ReSyncBaan(out sErr);
            res.msg = sErr;
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CondMng()
        {
            //johnsing 2018-05-11 暂不启用登录限制
            //if (!CommonInfo.IsLogin())
            //{
            //    return RedirectToAction("Index");
            //}
            return View();
        }

        [HttpGet]
        public ActionResult MailMng()
        {
            //johnsing 2018-05-11 暂不启用登录限制
            //if (!CommonInfo.IsLogin())
            //{
            //    return RedirectToAction("Index");
            //}
            return View();
        }


        [HttpPost]
        public ActionResult GetAllMails()
        {
            var lst = QueryHelper.GetAllMails();
            return Json(lst);
        }

        [HttpPost]
        public ActionResult DoAddMail(string txtEnName, string txtCnName, string txtMailAddr, string txtMailAddrType)
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

            if (string.IsNullOrEmpty(txtMailAddr))
            {
                res.msg = "邮件地址是必填项!";
                ModelState.AddModelError("", res.msg);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var sErr = string.Empty;
            var mailType = TMailType.To;
            Enum.TryParse<TMailType>(txtMailAddrType, out mailType);
            res.bok = QueryHelper.MailAdd(txtEnName, txtCnName, txtMailAddr, mailType, out sErr);
            res.msg = sErr;
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DelMail(int id, string mailAddr)
        {
            var res = new TRes
            {
                bok = false,
                msg = ""
            };
            res.bok = QueryHelper.DelMail(id, mailAddr);
            return Json(res);
        }

        [HttpPost]
        public ActionResult EnableUser(int id, bool enable)
        {
            var res = new TRes
            {
                bok = false
            };

            string errmsg = string.Empty;
            bool bRet = QueryHelper.EnableUser(id, enable, out errmsg);
            res.bok = bRet;
            res.msg = errmsg;
            return Json(res);
        }

        public ActionResult ManualCalc()
        {
            //TODO
            return View();
        }
        

    }
}

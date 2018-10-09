using System.Web.Mvc;
using WarehouseLaborEfficiencyWeb.DAL;

namespace WarehouseLaborEfficiencyWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("WeekData");
        }
        
        #region Page
        public ActionResult WeekData()
        {
            if (!CommonInfo.IsLogin())
            {
                return RedirectToAction("Signin", "Login");
            }
            return View();
        }
        public ActionResult MonthData()
        {
            if (!CommonInfo.IsLogin())
            {
                return RedirectToAction("Signin", "Login");
            }
            return View();
        }
        public ActionResult HCData()
        {
            if (!CommonInfo.IsLogin())
            {
                return RedirectToAction("Signin", "Login");
            }
            return View();
        }
        #endregion

        #region 数据编辑
        [HttpGet]
        public ActionResult ModifyData(string dataType)
        {
            switch (dataType)
            {
                case "WeekData":
                    return RedirectToAction("ModifyWeekData");
                case "MonthData":
                    return RedirectToAction("ModifyMonthData");
                case "HCData":
                    return RedirectToAction("ModifyHCData");
                default:break;
            }
            return new HttpNotFoundResult();
        }

        [HttpGet]
        public ActionResult ModifyWeekData()
        {
            if (!CommonInfo.HasRight(TRightID.MODIFY))
            {
                return new HttpUnauthorizedResult("没有权限！");
            }            
            return View();
        }
        [HttpGet]
        public ActionResult ModifyMonthData()
        {
            if (!CommonInfo.HasRight(TRightID.MODIFY))
            {
                return new HttpUnauthorizedResult("没有权限！"); 
            }
            return View();
        }
        [HttpGet]
        public ActionResult ModifyHCData()
        {
            if (!CommonInfo.HasRight(TRightID.MODIFY))
            {
                return new HttpUnauthorizedResult("没有权限！");
            }
            return View();
        }
        #endregion
    }
}

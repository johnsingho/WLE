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
            //if (!CommonInfo.IsLogin())
            //{
            //    return RedirectToAction("Signin", "Login");
            //}
            return View();
        }
        #endregion

    }
}

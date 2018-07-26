using WarehouseLaborEfficiencyWeb.DAL;
using Common.Authorization;
using Common.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WarehouseLaborEfficiencyWeb.Controllers
{
    public class LogInModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }

    public class LoginController : Controller
    {
        public ActionResult UserMng()
        {
            //TODO
            return View();
        }

        public ActionResult Signout()
        {
            CommonInfo.Logout();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Signin(string ReturnUrl)
        {
            var model = new LogInModel
            {
                ReturnUrl = Url.Action("Index", "Home")
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Signin(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string ad = model.Username;
            string pwd = model.Password;
            string msg = "";

            //CacheFactory.ClearCache(); //TODO remove

            try
            {
                UserBasicInfo domainUser = null;
                if (!CommonInfo.IsLogin())
                {
                    var user = SysUserInfo.GetUserInfoByAd(ad);
                    if (user == null)
                    {
                        msg = "账号不存在，请先注册! Login user not exist, please register first!";
                        ModelState.AddModelError("", msg);
                        return View();
                    }

                    //验证域密码
                    if (user != null)
                    {
                        domainUser = new UserBasicInfo(user.id, user.ADAccount, user.Email, user.FullName, user.IsAdmin);
                    }
                    if (user == null /*|| false == ConfigInfo.DebugRun*/)
                    {
                        domainUser = new ActiveDirectoryHelper().GetDomainUser(ad, pwd, out msg);
                        if (domainUser == null)
                        {
                            ModelState.AddModelError("", msg);
                            return View();
                        }
                        if (user != null)
                        {
                            domainUser = new UserBasicInfo(user.id, user.ADAccount, user.Email, user.FullName, user.IsAdmin);
                        }
                        // 更新用户邮箱
                        if (user != null && string.IsNullOrEmpty(domainUser.Email) == false &&
                            domainUser.Email != user.Email)
                        {
                            user.Email = domainUser.Email;
                            SysUserInfo.Update(user);
                        }
                    }
                    // 帐号被停用
                    if (!user.IsValid)
                    {
                        msg = "账号已被停用. Your account was disabled.";
                        ModelState.AddModelError("", msg);
                        return View();
                    }
                }
                else
                {
                    domainUser = CommonInfo.CurrentUser;
                }
                CommonInfo.Login(domainUser);
                SysUserInfo.UpdateUserLoginTimeByAd(ad);
                return RedirectToLocal(model.ReturnUrl);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(this.GetType(), ex);
                ModelState.AddModelError("", ex);
                return View();
            }
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }


        ///-------------------------------------------------

        [HttpPost]
        public ActionResult GetAllUsers()
        {
            var users = SysUserInfo.LoadAll();
            return Json(users);
        }

        [HttpPost]
        public ActionResult DoRegister(string inputad)
        {
            var res = new TRes
            {
                bok = false
            };
            string msg = string.Empty;
            res.bok = SysUserInfo.InsertUserInfo(inputad, ref msg);
            res.msg = msg;
            return Json(res);
        }
        
        [HttpPost]
        public ActionResult DelRegister(int? id)
        {
            var res = new TRes
            {
                bok = false
            };
            if (!id.HasValue)
            {
                res.msg = "无效用户ID";
                return Json(res);
            }
            
            //TODO 删除系统唯一的一个帐号怎样处理?
            string errmsg = string.Empty;
            bool bRet = SysUserInfo.DeleteUser(id.Value, out errmsg);
            res.bok = bRet;
            res.msg = errmsg;
            return Json(res);
        }

        [HttpPost]
        public ActionResult ModifyRegister(int id, bool enable)
        {
            var res = new TRes
            {
                bok = false
            };

            string errmsg = string.Empty;
            bool bRet = SysUserInfo.EnableUser(id, enable, out errmsg);
            res.bok = bRet;
            res.msg = errmsg;
            return Json(res);
        }
    }
}
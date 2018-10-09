using System.Web;
using System;
using System.Security.Policy;
using Common.Authorization;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    public class CommonInfo
    {
        private static CookieKey _cook = new CookieKey()
        {
            skUser = "FLEXUSERKEY_2018WLE",
            skSession = "FLEXSESSION_2018WLE",
            skBrowser = "FLEXBK_2018WLE"
        };
        
        public static readonly string SiteTitle = "Warehouse Labor Efficiency Dashboard";

        /// <summary>
        /// 获取当前登录的用户, 该值可能为 null
        /// </summary>
        public static UserBasicInfo CurrentUser
        {
            get
            {   
                if (!IsLogin())
                    return null;
                else
                {
                    var vUserState = UserState.GetInstance(_cook);
                    var userInfo = vUserState.GetLoginUser();
                    return userInfo;
                }
            }
        }

        public static bool IsLogin()
        {
            var vUserState = UserState.GetInstance(_cook);            
            return vUserState.IsLogin;
        }

        public static void Logout()
        {
            var vUserState = UserState.GetInstance(_cook);
            vUserState.Logout();
        }

        internal static void Login(UserBasicInfo domainUser)
        {
            var vUserState = UserState.GetInstance(_cook);
            vUserState.Login(domainUser);
        }

        public static bool HasRight(int nRightID)
        {
            var vUserState = UserState.GetInstance(_cook);
            if (!vUserState.IsLogin)
            {
                return false;
            }
            var loginUser = vUserState.GetLoginUser();
            return SysUserInfo.HasRight(loginUser.AdName, nRightID);
        }
    }
}

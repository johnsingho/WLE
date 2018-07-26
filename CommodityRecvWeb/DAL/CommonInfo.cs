using System.Web;
using System;
using System.Security.Policy;
using Common.Authorization;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    public class CommonInfo
    {
        private static UserBasicInfo _currentUser = null;
        private static CookieKey _cook = new CookieKey()
        {
            skUser = "FLEXUSERKEY_2018CommodityRecv",
            skSession = "FLEXSESSION_2018CommodityRecv",
            skBrowser = "FLEXBK_2018CommodityRecv"
        };
        
        public static readonly string SiteTitle = "Commodity Receive Dashboard";

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
                    if (_currentUser == null)
                    {
                        var vUserState = UserState.GetInstance(_cook);
                        _currentUser = vUserState.GetLoginUser();
                    }
                    return _currentUser;
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
    }
}

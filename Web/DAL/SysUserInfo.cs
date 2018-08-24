using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using WarehouseLaborEfficiencyWeb.Database;
using Common.Authorization;
using System.Data.SqlClient;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    public class TSysUserInfo
    {
        public int id { get; set; }
        public string ADAccount { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsValid { get; set; }
    }

    public class SysUserInfo
    {
        internal static List<TSysUserInfo> LoadAll()
        {
            using (var context = new WarehouseLaborEffEntities())
            {
                var users = from x in context.sys_user
                            select new TSysUserInfo
                            {
                                id = x.id,
                                ADAccount = x.ADAccount,
                                FullName = x.FullName,
                                Email = x.Email,
                                IsValid = x.IsValid
                            };
                return users.ToList();
            }
        }
        
        //取用户的roleID
        public static List<string> GetUserRoleID(string sAd)
        {
            //var userInfo = CommonInfo.CurrentUser;
            if (string.IsNullOrEmpty(sAd)) { return new List<string>(); }
            using (var context = new WarehouseLaborEffEntities())
            {
                var roleIDs = from r in context.sys_user
                              join ur in context.sys_user_role_conn
                              on r.id equals ur.RefUserID
                              where r.ADAccount==sAd
                              select ur.RefRoleID;
                return roleIDs.ToList();
            }
        }

        internal static List<SelectListItem> LoadRoles()
        {
            using (var context = new WarehouseLaborEffEntities())
            {
                var users = from r in context.sys_roles
                            select new SelectListItem
                            {
                                Text = r.RoleName,
                                Value = r.id,
                                Selected = false
                            };
                var ret = users.ToList();
                return ret;
            }
        }
        
        public static bool ChangeRole(string userAD, string[] roleIDs, out string sErr)
        {
            sErr = string.Empty;
            using (var context = new WarehouseLaborEffEntities())
            {
                var userID = (from u in context.sys_user
                                where (0 == String.Compare(u.ADAccount, userAD, StringComparison.InvariantCultureIgnoreCase))
                                select u.id).FirstOrDefault();
                if(0 == userID)
                {
                    return false;
                }

                //先删后加
                var bOk = false;
                try
                {
                    var qry = from x in context.sys_user_role_conn
                              where x.RefUserID == userID
                              select x;
                    context.sys_user_role_conn.RemoveRange(qry);

                    var lst = new List<sys_user_role_conn>();
                    foreach (var rid in roleIDs)
                    {
                        lst.Add(new sys_user_role_conn
                        {
                            RefRoleID = rid,
                            RefUserID = userID
                        });
                    }
                    context.sys_user_role_conn.AddRange(lst);
                    context.SaveChanges();
                    bOk = true;
                    sErr = string.Empty;
                }
                catch (Exception ex)
                {
                    sErr = ex.Message;
                }
                return bOk;
            }
        }

        public static sys_user GetUserInfoByAd(string sAdName)
        {
            sys_user user = null;
            using (var context = new WarehouseLaborEffEntities())
            {
                var people = from p in context.sys_user
                             where (0 == String.Compare(p.ADAccount, sAdName, StringComparison.InvariantCultureIgnoreCase))
                             select p;
                if (people.Any())
                {
                    user = people.First();
                }
            }
            return user;
        }



        public static bool HasOtherUsers()
        {
            //sys_user user = null;
            using (var context = new WarehouseLaborEffEntities())
            {
                var peoples = context.sys_user;
                if (peoples.Any())
                {
                    return true;
                }
            }
            return false;
        }

        private static DomainUserInfo GetAdInfo(string inputad, out string msg)
        {
            var adh = new Common.Authorization.ActiveDirectoryHelper();
            var adUser = adh.GetDomainUserByAD(inputad, out msg);
            return adUser;
        }

        public static bool InsertUserInfo(string inputad, ref string errmsg)
        {
            bool bOk = false;
            var adUser = GetAdInfo(inputad, out errmsg);
            if (adUser == null)
            {
                errmsg = "AD login failed!";
                return false;
            }
            var adInfo = GetUserInfoByAd(inputad);
            if (adInfo != null)
            {
                errmsg = "You had been registered!";
                return false;
            }
            using (var context = new WarehouseLaborEffEntities())
            {
                var entity = new sys_user()
                {
                    ADAccount = adUser.ADAccount,
                    Email = adUser.Email,
                    FullName = adUser.FirstName + ' ' + adUser.LastName,
                    IsAdmin = false,
                    IsValid = true
                };
                try
                {
                    context.sys_user.Add(entity);
                    context.SaveChanges();
                    bOk = true;
                    errmsg = string.Empty;
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                }
            }
            return bOk;
        }

        public static bool EnableUser(int uid, bool bEnabled, out string errmsg)
        {
            bool bOk = false;
            using (var context = new WarehouseLaborEffEntities())
            {
                var persons = from p in context.sys_user
                              where p.id == uid
                              select p;
                foreach (var obj in persons)
                {
                    obj.IsValid = bEnabled;
                }
                try
                {
                    context.SaveChanges();
                    bOk = true;
                    errmsg = string.Empty;
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                }
            }
            return bOk;
        }

        public static bool DeleteUser(int id, out string errmsg)
        {
            bool bOk = false;
            errmsg = string.Empty;
            using (var mContext = new WarehouseLaborEffEntities())
            {
                var persons = from p in mContext.sys_user
                              where p.id == id
                              select p;
                if (persons.Any())
                {
                    var obj = persons.First();
                    mContext.sys_user.Remove(obj);
                    try
                    {
                        mContext.SaveChanges();
                        bOk = true;
                    }
                    catch (Exception ex)
                    {
                        errmsg = ex.Message;
                    }
                }
            }
            return bOk;
        }
        
        public static void UpdateUserLoginTimeByAd(string sAdName)
        {
            using (var context = new WarehouseLaborEffEntities())
            {
                var people = from p in context.sys_user
                             where (0 == String.Compare(p.ADAccount, sAdName, StringComparison.InvariantCultureIgnoreCase))
                             select p;
                if (people.Any())
                {
                    var user = people.First();
                    user.LastLogon = DateTime.Now;
                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public static void Update(sys_user user)
        {
            using (var context = new WarehouseLaborEffEntities())
            {
                //user.LastLogon = DateTime.Now;
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static bool HasRight(string sAD, int nRightID)
        {
            using (var context = new WarehouseLaborEffEntities())
            {
                var qry = context.Database.SqlQuery<int>("select FN_Check_UserRight(@ad, @rightID)",
                                                        new MySqlParameter("@ad", sAD),
                                                        new MySqlParameter("@rightID", nRightID)
                                                        );
                return qry.FirstOrDefault() > 0;
            }
        }

    }
}

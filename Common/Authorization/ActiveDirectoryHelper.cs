using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;

namespace Common.Authorization {

    public class ActiveDirectoryHelper {

        /// <summary>
        /// 使用一个系统帐号检索活动目录并返回一个值
        /// 该值指示指定的域用户名是否是域用户
        /// </summary>
        /// <param name="adAccount"></param>
        /// <param name="domainUser"></param>
        /// <returns></returns>
        public bool IsDomainUser(string adAccount, string username, string password, out UserBasicInfo domainUser) {
            string msg;
            var adUser = GetDomainUserByAD(adAccount, username, password, out msg);
            domainUser = new UserBasicInfo { AdName = adUser.ADAccount, NickName = adUser.FirstName + " " + adUser.LastName, Email = adUser.Email };
            return domainUser != null;
        }

        /// <summary>
        /// 检查一个试图登录系统的用户
        /// </summary>
        /// <param name="adAccount"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsDomainUser(string adAccount, string password) {
            return GetDomainUser(adAccount, password) != null;
        }

        public UserBasicInfo GetDomainUser(string adAccount, string validatePwd) {
            string error = null;
            return GetDomainUser(adAccount, validatePwd, out error);
        }


        public UserBasicInfo GetDomainUser(string adAccount, string validatePwd, out string errorMessage) {
            DirectoryEntry entry = new DirectoryEntry(LDAPPath, adAccount, validatePwd);
            try {
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + adAccount + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if(result != null) {
                    DirectoryEntry de = result.GetDirectoryEntry();
                    UserBasicInfo ui = new UserBasicInfo();
                    ui.AdName = adAccount;
                    if(de.Properties["GivenName"].Value != null) {
                        ui.NickName = de.Properties["GivenName"].Value.ToString();
                    }
                    if(de.Properties["sn"].Value != null) {
                        ui.NickName += " " + de.Properties["sn"].Value.ToString();
                    }
                    if(de.Properties["mail"].Value != null) {
                        ui.Email = de.Properties["mail"].Value.ToString();
                    }
                    errorMessage = null;
                    return ui;
                }
            }
            catch(Exception ex) {
                errorMessage = ex.Message;
                return null;
            }

            errorMessage = null;
            return null;
        }


        private string LDAPPath {
            get { return ConfigurationManager.AppSettings["LDAPPath"]; }
        }

        private List<DomainUserInfo> GetDirectReports(string distinguishedName) {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            DirectoryEntry entry = new DirectoryEntry(null, null, null);
            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = string.Format("(manager={0})", distinguishedName);
            search.PropertiesToLoad.Add("cn");
            var results = search.FindAll();
            DirectoryEntry de = null;
            List<DomainUserInfo> users = new List<DomainUserInfo>();
            foreach(SearchResult result in results) {
                de = result.GetDirectoryEntry();
                users.Add(GetDomainUser(de));
            }
            return users;
        }

        public DomainUserInfo GetDomainUserByDisplayName(string fullname, out string errMsg) {
            var filter = string.Format("(displayName={0})", fullname);
            return _GetDomainUser(filter, out errMsg);
        }

        public DomainUserInfo GetDomainUserByAD(string adAccount, string username, string password, out string errMsg) {
            var filter = "(SAMAccountName=" + adAccount + ")";
            return _GetDomainUser(filter, username, password, out errMsg);
        }
        public DomainUserInfo GetDomainUserByAD(string adAccount, out string errMsg) {
            var filter = "(SAMAccountName=" + adAccount + ")";
            return _GetDomainUser(filter, out errMsg);
        }

        public DomainUserInfo GetDomainUserByEmail(string email, string username, string password, out string errMsg) {
            string filter = "(mail=" + email + ")";
            return _GetDomainUser(filter, username, password, out errMsg);
        }

        private DomainUserInfo _GetDomainUser(string filter, string username, string password, out string errMsg) {
            try {
                errMsg = "";
                DirectorySearcher search = new DirectorySearcher(new DirectoryEntry(LDAPPath, username, password));
                search.Filter = filter;
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if(result != null) {
                    DirectoryEntry de = result.GetDirectoryEntry();
                    return GetDomainUser(de);
                }
                return null;
            }
            catch(Exception ex) {
                errMsg = ex.Message;
                return null;
            }
        }
        private DomainUserInfo _GetDomainUser(string filter, out string errMsg) {
            try {
                errMsg = "";
                DirectorySearcher search = new DirectorySearcher(new DirectoryEntry(LDAPPath));
                search.Filter = filter;
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if(result != null) {
                    DirectoryEntry de = result.GetDirectoryEntry();
                    return GetDomainUser(de);
                }
                return null;
            } catch(Exception ex) {
                errMsg = ex.Message;
                return null;
            }
        }
        private DomainUserInfo GetDomainUser(DirectoryEntry entity) {
          
            DomainUserInfo user = new DomainUserInfo();
            user.ADAccount = GetPropertieValue(entity, "sAMAccountName");
            user.FirstName = GetPropertieValue(entity, "givenName");
            user.LastName = GetPropertieValue(entity, "sn");
            user.MobilePhone = GetPropertieValue(entity, "mobile");
            user.JobTitle = GetPropertieValue(entity, "title");
            user.Office = GetPropertieValue(entity, "physicalDeliveryOfficeName");
            user.DistinguishedName = GetPropertieValue(entity, "distinguishedName");
            user.ManagerDescription = GetPropertieValue(entity, "manager");
            var eid= GetPropertieValue(entity, "employeeID");
            user.EmployeeID = System.Text.RegularExpressions.Regex.Replace(eid, "\\D", "");
            user.Email = GetPropertieValue(entity, "mail");

            return user;
        }

        private string GetPropertieValue(DirectoryEntry entity, string propertieName) {
            var value = entity.Properties[propertieName].Value;
            if(value != null) {
                return value.ToString();
            }
            return null;
        }

        int level = 0;
        void WriteReports(DomainUserInfo manager) {
            var repots = GetDirectReports(manager.DistinguishedName);
            if(repots.Count > 0) {
                level++;
            }
            foreach(var rep in repots) {
                Console.WriteLine("{0}\t{1}", rep, level);
            }
            foreach(var rep in repots) {
                WriteReports(rep);
            }
        }

    }
}

using WarehouseLaborEfficiencyWeb.Database;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    public class QueryHelper
    {
        internal static List<v_CommodityRecvCmp> GetCompResult()
        {
            using (var context = new CommodityRecv_DashboardEntities())
            {
                var qry = from c in context.v_CommodityRecvCmp.AsNoTracking()
                          orderby c.ITEM, c.RECEIVEDATE
                          select c;
                return qry.ToList();
            }
        }
        internal static List<tbl_cr_Condition> GetConditions()
        {
            using(var context = new CommodityRecv_DashboardEntities())
            {
                var qry = from c in context.tbl_cr_Condition
                          orderby c.CostItemNumber
                          select c;
                return qry.ToList();
            }
        }

        internal static object GetAllMails()
        {
            using (var context = new CommodityRecv_DashboardEntities())
            {
                var qry = from c in context.tbl_cr_mailReceiver
                          orderby c.enName
                          select c;
                return qry.ToList();
            }
        }

        internal static bool DelCondition(int id, string costItemNumber)
        {
            using (var context = new CommodityRecv_DashboardEntities())
            {
                var its = from p in context.tbl_cr_Condition
                          where p.CostItemNumber.Equals(costItemNumber)
                          select p;
                if (!its.Any())
                {
                    return false;
                }
                context.tbl_cr_Condition.Remove(its.First());
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(typeof(QueryHelper), ex);
                    return false;
                }
                return true;
            }
        }

        internal static bool SingleCondAdd(string txtCommodityCode, 
                                           string txtCostItemNumber, 
                                           string txtCommodityCodeDesc, 
                                           out string sErr)
        {
            sErr = string.Empty;
            bool bOk = false;
            using (var context = new CommodityRecv_DashboardEntities())
            {
                //insert
                var entity = new tbl_cr_Condition()
                {
                    CommodityCode=txtCommodityCode.Trim(),
                    CostItemNumber=txtCostItemNumber.Trim(),
                    CommodityCodeDescription=txtCommodityCodeDesc.Trim()
                };
                try
                {
                    context.tbl_cr_Condition.Add(entity);
                    context.SaveChanges();
                    bOk = true;
                }
                catch (Exception ex)
                {
                    //sErr = ex.Message
                    sErr = string.Format("{0} 可能已存在", txtCostItemNumber);
                }
            }
            return bOk;
        }

        internal static bool ReSyncBaan(out string msg)
        {
            CommodityRecv_DAL dal = new CommodityRecv_DAL(CustomConfig.ConnStrBaan, CustomConfig.ConnStrMain);
            return dal.DoSyncData(out msg) > 0;
        }

        internal static bool MailAdd(string txtEnName, string txtCnName, string txtMailAddr, TMailType eMailType, out string sErr)
        {
            sErr = string.Empty;
            bool bOk = false;
            using (var context = new CommodityRecv_DashboardEntities())
            {
                //insert
                var entity = new tbl_cr_mailReceiver()
                {
                    enName=txtEnName.Trim(),
                    cnName=txtCnName.Trim(),
                    mailAddr=txtMailAddr.Trim(),
                    mailAddrType=(int)eMailType,
                    isValid= true //true有效，false无效
                };
                try
                {
                    context.tbl_cr_mailReceiver.Add(entity);
                    context.SaveChanges();
                    bOk = true;
                }
                catch (Exception ex)
                {
                    //sErr = ex.Message
                    sErr = string.Format("{0} 可能已存在", txtMailAddr);
                }
            }
            return bOk;
        }

        internal static bool DelMail(int id, string mailAddr)
        {
            using (var context = new CommodityRecv_DashboardEntities())
            {
                var its = from p in context.tbl_cr_mailReceiver
                          where p.mailAddr.Equals(mailAddr)
                          select p;
                if (!its.Any())
                {
                    return false;
                }
                context.tbl_cr_mailReceiver.Remove(its.First());
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(typeof(QueryHelper), ex);
                    return false;
                }
                return true;
            }
        }

        internal static bool EnableUser(int id, bool enable, out string errmsg)
        {
            errmsg = string.Empty;
            using (var context = new CommodityRecv_DashboardEntities())
            {
                var its = from p in context.tbl_cr_mailReceiver
                          where p.id==id
                          select p;
                if (!its.Any())
                {
                    errmsg = "用户不存在";
                    return false;
                }
                var item = its.First();
                item.isValid = enable;
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(typeof(QueryHelper), ex);
                    errmsg = ex.Message;
                    return false;
                }
                return true;
            }
        }
    }
}
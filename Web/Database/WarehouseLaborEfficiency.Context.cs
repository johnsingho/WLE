﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WarehouseLaborEfficiencyWeb.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WarehouseLaborEfficiencyEntities : DbContext
    {
        public WarehouseLaborEfficiencyEntities()
            : base("name=WarehouseLaborEfficiencyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sys_role_rights> sys_role_rights { get; set; }
        public virtual DbSet<sys_roles> sys_roles { get; set; }
        public virtual DbSet<sys_user> sys_user { get; set; }
        public virtual DbSet<sys_user_role_conn> sys_user_role_conn { get; set; }
        public virtual DbSet<tbl_bu> tbl_bu { get; set; }
        public virtual DbSet<tbl_HCData> tbl_HCData { get; set; }
        public virtual DbSet<tbl_MonthData> tbl_MonthData { get; set; }
        public virtual DbSet<tbl_WeekData> tbl_WeekData { get; set; }
        public virtual DbSet<V_Tbl_HCData> V_Tbl_HCData { get; set; }
        public virtual DbSet<V_Tbl_MonthData> V_Tbl_MonthData { get; set; }
        public virtual DbSet<V_Tbl_WeekData> V_Tbl_WeekData { get; set; }
        public virtual DbSet<V_USER_RIGHTS> V_USER_RIGHTS { get; set; }
    }
}

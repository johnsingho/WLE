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
    
    public partial class WarehouseLaborEffEntities : DbContext
    {
        public WarehouseLaborEffEntities()
            : base("name=WarehouseLaborEffEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sys_rights> sys_rights { get; set; }
        public virtual DbSet<sys_role_right_conn> sys_role_right_conn { get; set; }
        public virtual DbSet<sys_roles> sys_roles { get; set; }
        public virtual DbSet<sys_user> sys_user { get; set; }
        public virtual DbSet<sys_user_role_conn> sys_user_role_conn { get; set; }
        public virtual DbSet<tbl_bu> tbl_bu { get; set; }
        public virtual DbSet<tbl_hcdata> tbl_hcdata { get; set; }
        public virtual DbSet<tbl_monthdata> tbl_monthdata { get; set; }
        public virtual DbSet<tbl_weekdata> tbl_weekdata { get; set; }
        public virtual DbSet<v_tbl_hcdata> v_tbl_hcdata { get; set; }
        public virtual DbSet<v_tbl_hcdata_rate> v_tbl_hcdata_rate { get; set; }
        public virtual DbSet<v_tbl_monthdata> v_tbl_monthdata { get; set; }
        public virtual DbSet<v_tbl_weekdata> v_tbl_weekdata { get; set; }
        public virtual DbSet<v_user_rights> v_user_rights { get; set; }
    }
}

//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class v_tbl_hcdata
    {
        public int id { get; set; }
        public System.DateTime Date { get; set; }
        public string Warehouse { get; set; }
        public Nullable<int> Overall { get; set; }
        public Nullable<int> System_Clerk { get; set; }
        public Nullable<int> Inventory_Control { get; set; }
        public Nullable<int> RTV_Scrap { get; set; }
        public Nullable<int> Receiving { get; set; }
        public Nullable<int> Shipping { get; set; }
        public Nullable<int> Forklift_Driver { get; set; }
        public Nullable<int> Total { get; set; }
    }
}

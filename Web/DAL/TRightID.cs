using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseLaborEfficiencyWeb.DAL
{
    //对应 sys_rights 的ID
    public static class TRightID
    {
        public static readonly int ADMIN = 999;
        public static readonly int READONLY = 1;
        public static readonly int DOWNLOAD = 2;
        public static readonly int UPLOAD = 3;
    }
}
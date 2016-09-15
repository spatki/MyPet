using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_role_menu_access : Entity
    {
        public int MenuID { get; set; }
        public int AccessType { get; set; }
        public int RoleID { get; set; }
        public Nullable<bool> addAccess { get; set; }
        public Nullable<bool> deleteAccess { get; set; }
        public Nullable<bool> updateAccess { get; set; }
    }
}

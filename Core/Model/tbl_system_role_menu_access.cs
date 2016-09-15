using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_system_role_menu_access : Entity
    {
        public int MenuID { get; set; }
        public Nullable<int> AccessType { get; set; }
        public int RoleID { get; set; }
        public Nullable<bool> addAccess { get; set; }
        public Nullable<bool> deleteAccess { get; set; }
        public Nullable<bool> updateAccess { get; set; }

        [ForeignKey("MenuID")]
        public virtual mstr_access_menu mstr_access_menu { get; set; }
        [ForeignKey("RoleID")]
        public virtual webpages_Roles webpages_Roles { get; set; }
    }
}

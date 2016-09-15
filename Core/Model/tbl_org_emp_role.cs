using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_emp_role: Entity
    {
        public int tbl_Org_EmployeeID { get; set; }
        public int mstr_Org_RoleID { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
        public Nullable<bool> PrimaryRole { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }

        [ForeignKey("mstr_Org_RoleID")]
        public virtual mstr_org_role mstr_org_role { get; set; }
        [ForeignKey("tbl_Org_EmployeeID")]
        public virtual tbl_org_employee tbl_org_employee  { get; set; }
    }
}

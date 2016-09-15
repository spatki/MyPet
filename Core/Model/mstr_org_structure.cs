using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{   
    public partial class mstr_org_structure: Entity
    {
        public int mstr_Org_DesignationID { get; set; }
        public short Level { get; set; }
        public Nullable<int> mstr_Org_DesignationParentID { get; set; }
        public bool IsRoleSpecific { get; set; }
        public Nullable<int> mstr_Org_RoleID { get; set; }
        public Nullable<bool> IsEmployeeSpecific { get; set; }
        public Nullable<int> tbl_Org_EmployeeID { get; set; }
        public string StructPath { get; set; }
        public string Comments { get; set; }


        [ForeignKey("mstr_Org_DesignationID")]
        public virtual mstr_org_designation mstr_org_designation { get; set; }
        [ForeignKey("mstr_Org_RoleID")]
        public virtual mstr_org_role mstr_org_role { get; set; }
        [ForeignKey("tbl_Org_EmployeeID")]
        public virtual tbl_org_employee tbl_org_employee { get; set; }
    }
}

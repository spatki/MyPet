using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_audit_role : Entity
    {
        public int tbl_Audit_PlanID { get; set; }
        public byte Type { get; set; }
        public int tbl_Org_RoleID { get; set; }

        [ForeignKey("tbl_Audit_PlanID")]
        public tbl_audit_plan tbl_audit_plan { get; set; }
        [ForeignKey("tbl_Org_RoleID")]
        public mstr_org_role mstr_org_role { get; set; }
    }
}

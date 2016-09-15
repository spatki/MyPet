using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_audit_participant : Entity
    {
        public int tbl_Audit_ScheduleID { get; set; }
        public byte Type { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public int tbl_Org_RoleID { get; set; }

        [ForeignKey("tbl_Audit_ScheduleID")]
        public tbl_audit_schedule tbl_audit_schedule { get; set; }
        [ForeignKey("tbl_Org_EmployeeID")]
        public tbl_org_employee tbl_org_employee { get; set; }
        [ForeignKey("tbl_Org_RoleID")]
        public mstr_org_role mstr_org_role { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_audit_participant : Entity
    {
        public int tbl_Org_Audit_ScheduleID { get; set; }
        public byte Type { get; set; }
        public int tbl_Org_EmployeeID { get; set; }

        [ForeignKey("tbl_Org_EmployeeID")]
        public tbl_org_employee tbl_org_employee { get; set; }
        [ForeignKey("tbl_Org_Audit_ScheduleID")]
        public tbl_org_audit_schedule tbl_org_audit_schedule { get; set; }
    }
}

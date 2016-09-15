using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_audit_schedule : Entity
    {
        public int tbl_Org_Audit_PlanID { get; set; }
        public DateTime Planned_Start { get; set; }
        public DateTime Planned_Finish { get; set; }
        public Nullable<DateTime> Start { get; set; }
        public Nullable<DateTime> Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<DateTime> Audit_Date { get; set; }
        public Nullable<decimal> TimeSpentHrs { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }
        public string ClosureComments { get; set; }
        public Nullable<byte> ClosureStatus { get; set; }
        public Nullable<DateTime> NextAuditOn { get; set; }
        public string EvaluationCAPA { get; set; }
        public Nullable<int> ClosedBy { get; set; }

        [ForeignKey("tbl_Org_Audit_PlanID")]
        public tbl_org_audit_plan tbl_org_audit_plan { get; set; }
        public ICollection<tbl_org_audit_observation> tbl_org_audit_observation { get; set; }
        public ICollection<tbl_org_audit_participant> tbl_org_audit_participant { get; set; }
        public ICollection<tbl_org_audit_addln_obs> tbl_org_audit_addln_obs { get; set; }
    }
}

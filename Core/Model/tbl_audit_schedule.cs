using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_audit_schedule : Entity
    {
        public int tbl_Audit_PlanID { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> tbl_Mstr_Org_FunctionID { get; set; }
        public DateTime Planned_Start { get; set; }
        public DateTime Planned_Finish { get; set; }
        public Nullable<DateTime> Start { get; set; }
        public Nullable<DateTime> Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        public Nullable<DateTime> Review_Start { get; set; }
        public Nullable<DateTime> Review_Finish { get; set; }
        public byte Status { get; set; }
        public Nullable<DateTime> RecordingDate { get; set; }
        public Nullable<decimal> TimeSpentHrs { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }
        public string ClosureComments { get; set; }
        public Nullable<int> ClosedBy { get; set; }

        [ForeignKey("tbl_Audit_PlanID")]
        public tbl_audit_plan tbl_audit_plan { get; set; }
        public ICollection<tbl_audit_observation> tbl_audit_observation { get; set; }
        public ICollection<tbl_audit_checklist> tbl_audit_checklist { get; set; }
        public ICollection<tbl_audit_participant> tbl_audit_participant { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_audit_observation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int ID { get; set; }
        public int tbl_Audit_ScheduleID { get; set; }
        public string NC_Observation { get; set; }
        public string PossibleImpact_Attr { get; set; }
        public string CorrectiveAction { get; set; }
        public Nullable<int> Responsibility { get; set; }
        public Nullable<DateTime> EstimatedCloseDate { get; set; }
        public Nullable<DateTime> ActualCloseDate { get; set; }
        public short Status { get; set; }
        public Nullable<DateTime> StatusUpdateDate { get; set; }
        public string Comments { get; set; }

        [ForeignKey("tbl_Audit_ScheduleID")]
        public tbl_audit_schedule tbl_audit_proj_schedule { get; set; }
    }
}

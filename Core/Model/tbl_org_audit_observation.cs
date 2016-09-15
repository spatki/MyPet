using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_audit_observation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int ID { get; set; }
        public int tbl_Org_Audit_ScheduleID { get; set; }
        public string NCObservation { get; set; }
        public string EvidenceRefDocs { get; set; }
        public Nullable<byte> Category { get; set; }
        public string Clause { get; set; }
        public string Reference { get; set; }
        public Nullable<DateTime> StatusUpdateDate { get; set; }
        public Nullable<DateTime> ActualCloseDate { get; set; }
        public string CorrectiveAction { get; set; }
        public string RootCauseAnalysis { get; set; }
        public Nullable<DateTime> TargetDateCA { get; set; }
        public string PreventiveAction { get; set; }
        public Nullable<DateTime> TargetDatePA { get; set; }
        public Nullable<byte> StatusCA { get; set; }
        public Nullable<byte> StatusPA { get; set; }
        public Nullable<byte> Status { get; set; }

        [ForeignKey("tbl_Org_Audit_ScheduleID")]
        public tbl_org_audit_schedule tbl_org_audit_schedule { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_audit_checklist : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int ID { get; set; }
        public int tbl_Audit_ScheduleID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int PhaseID { get; set; }
        public short GroupSequenceNo { get; set; }
        public string GroupName { get; set; }
        public short ChkLstSequenceNo { get; set; }
        public string Task { get; set; }
        public bool Applicable { get; set; }
        public string Score { get; set; }
        public decimal PCI_Score { get; set; }
        public string Comments { get; set; }

        [ForeignKey("tbl_Audit_ScheduleID")]
        public tbl_audit_schedule tbl_audit_schedule { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; } 
    }
}

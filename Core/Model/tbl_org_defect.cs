using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_defect : Entity
    {
        public string Short_Description { get; set; }
        public string Details { get; set; }
        public int Type { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> PhaseID { get; set; }
        public Nullable<int> GroupID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> IdentifiedOn { get; set; }
        public string AffectedWP { get; set; }
        public Nullable<int> InjectedInPhaseID { get; set; }
        public string Cause { get; set; }
        public int SeverityID { get; set; }
        public string Impact { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public Nullable<DateTime> AssignedOn { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string ActionTaken { get; set; }
        public Nullable<DateTime> FixedOn { get; set; }
        public Nullable<int> VerifiedBy { get; set; }
        public Nullable<DateTime> VerifiedOn { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> RepeatIssue { get; set; }
        public Nullable<decimal> ResolutionEffortsPD { get; set; }

        public ICollection<tbl_org_defect_document> tbl_org_defect_document { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("mstr_Process_LC_StatusID")]
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("SeverityID")]
        public mstr_org_defect_severity mstr_org_defect_severity { get; set; }
        [ForeignKey("Type")]
        public mstr_org_defect_type mstr_org_defect_type { get; set; }
        [ForeignKey("PhaseID")]
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        [ForeignKey("GroupID, tbl_Org_ProjectID")]
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        [ForeignKey("TaskID")]
        public tbl_org_proj_plan tbl_org_proj_plan { get; set; }
        [ForeignKey("AssignedTo")]
        public UserProfile AssignedToUser { get; set; }
        [ForeignKey("VerifiedBy")]
        public UserProfile VerifiedByUser { get; set; }
    }
}

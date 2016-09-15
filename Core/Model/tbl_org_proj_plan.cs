using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_plan: Entity
    {
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 1)]
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 2)]
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public Nullable<int> PlanID { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int tbl_Mapped_Proj_ProcessID { get; set; }
        public Nullable<int> tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public System.DateTime PlannedStartDate { get; set; }
        public Nullable<System.DateTime> ActualStartDate { get; set; }
        public Nullable<System.DateTime> PlannedEndDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        public Nullable<System.DateTime> BaselineStart { get; set; }
        public Nullable<System.DateTime> BaselineEnd { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<bool> IsMilestone { get; set; }
        public Nullable<bool> AutoCreated { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public decimal PlannedDuration { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        public Nullable<byte> DurationUnit { get; set; }
        public Nullable<short> PercentComplete { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<int> NextTaskID { get; set; }
        public Nullable<bool> RuleExecutionPending { get; set; }
        public Nullable<bool> RuleApplicable { get; set; }
        public string Text1Caption { get; set; }
        public string Text2Caption { get; set; }
        public string Text3Caption { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }

        [ForeignKey("PlanID")]
        public tbl_org_proj_planname tbl_org_proj_planname { get; set; }
        [ForeignKey("tbl_Process_RepositoryID")]
        public tbl_process_repository tbl_process_repository { get; set; }
        [ForeignKey("tbl_Process_Rep_TaskID")]
        public tbl_process_rep_task tbl_process_rep_task { get; set; }
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        [ForeignKey("mstr_Org_Proj_PhaseID")]
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        [ForeignKey("mstr_Process_LC_StatusID")]
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("tbl_Mapped_Proj_ProcessID")]
        public tbl_org_project_process_mapping tbl_org_project_process_mapping { get; set; }
        public ICollection<tbl_org_plan_resource> tbl_org_plan_resource { get; set; }
        public ICollection<tbl_org_plan_document> tbl_org_plan_document { get; set; }
    }
}

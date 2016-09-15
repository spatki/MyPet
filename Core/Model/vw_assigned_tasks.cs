using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_assigned_tasks : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int Plan_ResourceID { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public DateTime res_PlannedStart { get; set; }
        public Nullable<DateTime> res_PlannedEnd { get; set; }
        public int PlanID { get; set; }
        public int tbl_Mapped_Proj_ProcessID { get; set; }
        public string Plan_TaskName { get; set; }
        public int RepositoryID { get; set; }
        public string RepositoryName { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public string Rep_TaskName { get; set; }
        public Nullable<int> GroupID { get; set; }
        public string GroupName { get; set; }
        public decimal res_PlannedDuration { get; set; }
        public Nullable<decimal> res_ActualDuration { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<bool> IsPublish { get; set; }
        public Nullable<bool> IsReview { get; set; }
        public Nullable<int> res_StatusID { get; set; }
        public string res_TaskStatus { get; set; }
        public Nullable<DateTime> res_ActualStart { get; set; }
        public Nullable<DateTime> res_ActualEnd { get; set; }
        public Nullable<DateTime> plan_ActualEnd { get; set; }
        public Nullable<DateTime> plan_ActualStart { get; set; }
    }
}

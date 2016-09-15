using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_timesheetEntry : Entity
    {
        public int tbl_Org_EmployeeID { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public string GroupName { get; set; }
        public string SubGroupName { get; set; }
        public string TaskName { get; set; }
        public Nullable<DateTime> PlannedStart { get; set; }
        public Nullable<DateTime> PlannedEnd { get; set; }
        public Nullable<DateTime> ActualStart { get; set; }
        public Nullable<DateTime> ActualEnd { get; set; }
        public Nullable<int> TaskStatus { get; set; }
        public Nullable<short> mstr_Org_FunctionID { get; set; }
        public Nullable<int> tbl_Org_Proj_PlanID { get; set; }
        public Nullable<int> tbl_Org_Plan_ResourceID { get; set; }
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        public Nullable<int> mstr_Org_Sub_FunctionID { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Mapped_Proj_ProcessID { get; set; }
        public Nullable<int> tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public Nullable<int> tbl_General_TaskID { get; set; }
        public decimal Duration { get; set; }
        public Nullable<decimal> PlannedDuration { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        public System.DateTime TSDate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<decimal> BillableDuration { get; set; }
        public Nullable<decimal> OvertimeDuration { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }
        public string Comments { get; set; }
    }
}

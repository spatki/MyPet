using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_timesheet: Entity
    {
        public int tbl_Org_EmployeeID { get; set; }
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 2)]
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Org_FunctionID { get; set; }
        public Nullable<int> tbl_Org_Proj_PlanID { get; set; }
        public Nullable<int> tbl_Org_Plan_ResourceID { get; set; }
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 1)]
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        public Nullable<int> mstr_Org_Sub_FunctionID { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Mapped_Proj_ProcessID { get; set; }
        public Nullable<int> tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public Nullable<int> tbl_General_TaskID { get; set; }
        public decimal Duration { get; set; }
        public System.DateTime TSDate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<decimal> BillableDuration { get; set; }
        public Nullable<decimal> OvertimeDuration { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> ApprovalID { get; set; }
        public string Comments { get; set; }

        [ForeignKey("tbl_Org_EmployeeID")]
        public tbl_org_employee tbl_org_employee { get; set; }
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        [ForeignKey("mstr_Org_FunctionID")]
        public mstr_org_function mstr_org_function { get; set; }
        [ForeignKey("mstr_Org_Sub_FunctionID")]
        public mstr_org_sub_function mstr_org_sub_function { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("tbl_Org_Proj_PlanID")]
        public tbl_org_proj_plan tbl_org_proj_plan { get; set; }
        [ForeignKey("tbl_Org_Plan_ResourceID")]
        public tbl_org_plan_resource tbl_org_plan_resource { get; set; }
    }
}

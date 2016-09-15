using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_plan_resource: Entity
    {
        public int tbl_Org_Proj_PlanID { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public byte AllocationPercent { get; set; }
        public System.DateTime PlannedStart { get; set; }
        public Nullable<System.DateTime> ActualStart { get; set; }
        public Nullable<System.DateTime> PlannedEnd { get; set; }
        public Nullable<System.DateTime> ActualEnd { get; set; }
        public Nullable<decimal> PlannedDuration { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        public Nullable<short> PercentComplete { get; set; }
        public Nullable<bool> Billable { get; set; }
        public int tbl_Process_Rep_ActivityID { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> OrgResource_AllocationID { get; set; }
        public Nullable<bool> AutoAssigned { get; set; }
        public Nullable<System.DateTime> AssignPickUpDate { get; set; }
        public string Comments { get; set; }

        [ForeignKey("tbl_Org_Proj_PlanID")]
        public tbl_org_proj_plan tbl_org_proj_plan { get; set; }
        [ForeignKey("tbl_Org_EmployeeID")]
        public tbl_org_employee tbl_org_employee { get; set; }
        [ForeignKey("mstr_Process_LC_StatusID")]
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
        public ICollection<tbl_org_plan_filled_document> tbl_org_plan_filled_document { get; set; }
        public ICollection<tbl_org_timesheet> tbl_org_timesheet { get; set; }
    }
}

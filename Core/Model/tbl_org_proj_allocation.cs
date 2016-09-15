using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_allocation: Entity
    {
        public int tbl_Org_ProjID { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        public Nullable<int> tbl_Org_Proj_LocationID { get; set; }
        public int mstr_Org_RoleID { get; set; }
        public System.DateTime PlannedStartDate { get; set; }
        public Nullable<System.DateTime> ActualStartDate { get; set; }
        public Nullable<System.DateTime> PlannedEndDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        public short Percent_Allocation { get; set; }
        public Nullable<int> ReportingTo { get; set; }
        public Nullable<bool> WorkingResource { get; set; }
        public Nullable<bool> ReviewResource { get; set; }
        public Nullable<bool> Stakeholder { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<bool> ManagementResource { get; set; }
        public Nullable<bool> DefectAdmin { get; set; }
        public Nullable<bool> IssueAdmin { get; set; }
        public Nullable<bool> HelpDeskAdmin { get; set; }
        public Nullable<bool> CRAdmin { get; set; }
        public Nullable<short> TimesheetHRS { get; set; }
        public string Comments { get; set; }
        public Nullable<int> RequisitionID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [ForeignKey("tbl_Org_EmployeeID")]
        public tbl_org_employee tbl_org_employee { get; set; }
        [ForeignKey("tbl_Org_ProjID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("tbl_Org_Proj_LocationID")]
        public tbl_org_proj_location tbl_org_proj_location { get; set; }

    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class vw_project_allocations
    {
        public int ID { get; set; }
        public int EmpID { get; set; }
        public string EmpCode { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public int CurrentDesignation { get; set; }
        public string desig_ShortName { get; set; }
        public string desig_LongName { get; set; }
        public int projectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime PlannedStart { get; set; }
        public Nullable<DateTime> ActualStart { get; set; }
        public Nullable<DateTime> PlannedEnd { get; set; }
        public Nullable<DateTime> ActualEnd { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public int RoleID { get; set; }
        public string role_ShortName { get; set; }
        public string role_LongName { get; set; }
        public Nullable<int> LocationID { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> GroupID { get; set; }
        public string GroupName { get; set; }
        public Nullable<short> groupLevel { get; set; }
        public Nullable<int> ManagerID { get; set; }
        public string ManagerGivenName { get; set; }
        public string ManagerFamilyName { get; set; }
        public string AllocationComments { get; set; }
    }
}

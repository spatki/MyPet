using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class vw_resourcewise_TaskStatus
    {
        public Nullable<int> ClientID { get; set; }
        public int ID { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public Nullable<short> alloc_GroupID { get; set; }
        public int RoleID { get; set; }
        public DateTime alloc_planned_start { get; set; }
        public Nullable<DateTime> alloc_planned_end { get; set; }
        public Nullable<DateTime> alloc_actual_start { get; set; }
        public Nullable<DateTime> alloc_actual_end { get; set; }
        public short alloc_percent { get; set; }
        public Nullable<int> alloc_reporting { get; set; }
        public Nullable<bool> alloc_billable { get; set; }
        public Nullable<int> planID { get; set; }
        public string TaskName { get; set; }
        public Nullable<DateTime> task_planned_start { get; set; }
        public Nullable<DateTime> task_planned_end { get; set; }
        public Nullable<byte> task_planned_duration { get; set; }
        public Nullable<DateTime> resource_planned_start { get; set; }
        public Nullable<DateTime> resource_planned_end { get; set; }
        public Nullable<byte> resource_planned_duration { get; set; }
        public Nullable<byte> resource_actual_duration { get; set; }
        public Nullable<byte> resource_alloc_percent { get; set; }
        public Nullable<byte> resource_percent_complete { get; set; }
        public Nullable<int> resource_task_statusID { get; set; }
        public string resource_task_status { get; set; }
    }
}

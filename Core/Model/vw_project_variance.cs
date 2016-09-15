using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class vw_project_variance : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public string Name { get; set; }
        public DateTime PlannedStartDate {get; set;}
        public Nullable<DateTime> PlannedEndDate {get; set;}
        public Nullable<Decimal> Duration { get; set; }
        public Nullable<DateTime> ActualStartDate {get; set;}
        public Nullable<DateTime> ActualEndDate { get; set; }
        public Nullable<Decimal> ActualDuration { get; set; }
        public Nullable<bool> IsComplete {get; set;}
        public Nullable<Decimal> EffortVariance { get; set; }
        public Nullable<int> ScheduleVariance { get; set; }
    }
}

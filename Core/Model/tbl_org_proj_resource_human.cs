using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_resource_human: Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public byte Type { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public System.DateTime PlannedStart { get; set; }
        public Nullable<System.DateTime> ActualStart { get; set; }
        public Nullable<System.DateTime> PlannedEnd { get; set; }
        public Nullable<System.DateTime> ActualEnd { get; set; }
        public Nullable<bool> OwnedByProject { get; set; }
        public Nullable<bool> AcquireNew { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public int RequestedBy { get; set; }
        public System.DateTime RequestDate { get; set; }

    }
}

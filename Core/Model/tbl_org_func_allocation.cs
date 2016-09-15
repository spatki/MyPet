using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_func_allocation: Entity
    {
        public short mstr_Org_FunctionID { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public System.DateTime AllocationDate { get; set; }
        public Nullable<System.DateTime> ConfirmationDate { get; set; }
        public Nullable<System.DateTime> PlannedReleaseDate { get; set; }
        public Nullable<System.DateTime> ReleaseDate { get; set; }
        public Nullable<byte> PercentAllocation { get; set; }
        public Nullable<bool> WorkingResource { get; set; }
        public Nullable<bool> ManagementResource { get; set; }
        public Nullable<bool> ReviewResource { get; set; }
        public Nullable<bool> Stakeholder { get; set; }
        public short mstr_Org_RoleID { get; set; }
        public Nullable<short> mstr_Org_Sub_FunctionID { get; set; }
        public Nullable<short> mstr_Process_LC_StatusID { get; set; }
    }
}

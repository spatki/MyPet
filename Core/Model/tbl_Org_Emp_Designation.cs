using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_Org_Emp_Designation: Entity
    {
        public int tbl_Org_EmployeeID { get; set; }
        public short mstr_Org_DesignationID { get; set; }
        public bool PrimaryDesignation { get; set; }
        public bool CurrentDesignation { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTill { get; set; }
        public string Comments { get; set; }
    }
}

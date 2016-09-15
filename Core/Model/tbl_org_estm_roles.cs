using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_estm_roles : Entity
    {
        public int tbl_Org_RoleID { get; set; }
        public decimal FP_Day {get; set;}
    }
}

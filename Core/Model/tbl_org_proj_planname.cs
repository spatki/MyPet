using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_planname : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<DateTime> PlannedStart { get; set; }
        public Nullable<DateTime> PlannedEnd { get; set; }

        public ICollection<tbl_org_proj_plan> tbl_org_proj_plan { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; } 
    }
}

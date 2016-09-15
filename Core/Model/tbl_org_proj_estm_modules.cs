using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_estm_modules : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Proj_EstimationID { get; set; }
        public string Name { get; set; }
        public Nullable<int> GroupMapping { get; set; }

        [ForeignKey("tbl_Proj_EstimationID")]
        public tbl_org_proj_estimation tbl_org_proj_estimation { get; set; }
    }
}

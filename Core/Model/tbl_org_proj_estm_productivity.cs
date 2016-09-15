using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_estm_productivity : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Proj_EstimationID { get; set; }
        public int tbl_Org_RoleID { get; set; }
        public short ResourceCount { get; set; }
        public Nullable<decimal> FP_Per_Day { get; set; }
        public Nullable<decimal> PercentAllocation { get; set; }

        [ForeignKey("tbl_Proj_EstimationID")]
        public tbl_org_proj_estimation tbl_org_proj_estimation { get; set; }
        [ForeignKey("tbl_Org_RoleID")]
        public mstr_org_role mstr_org_role { get; set; }
    }
}

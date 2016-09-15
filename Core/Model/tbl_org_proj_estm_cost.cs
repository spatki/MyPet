using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_estm_cost : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Estm_EffSchID { get; set; }
        public int tbl_Org_Proj_EstimationID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Org_RoleID { get; set; }
        public decimal SBR { get; set; }

        [ForeignKey("tbl_Org_RoleID")]
        public mstr_org_role mstr_org_role { get; set; }
    }
}

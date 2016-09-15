using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_estm_gsc : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Proj_EstimationID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Org_Estm_TDI_ID { get; set; }
        public Nullable<byte> Rating { get; set; }

        [ForeignKey("tbl_Proj_EstimationID")]
        public tbl_org_proj_estimation tbl_org_proj_estimation { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_estm_size : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 2)]
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Proj_EstimationID { get; set; }
        public int ParameterID { get; set; }
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 1)]
        public Nullable<int> ProjGroupID { get; set; }
        public Nullable<decimal> SimpleW { get; set; }
        public Nullable<decimal> MediumW { get; set; }
        public Nullable<decimal> ComplexW { get; set; }
        public Nullable<decimal> SimpleC { get; set; }
        public Nullable<decimal> MediumC { get; set; }
        public Nullable<decimal> ComplexC { get; set; }
        public Nullable<decimal> SimpleWP { get; set; }
        public Nullable<decimal> MediumWP { get; set; }
        public Nullable<decimal> ComplexWP { get; set; }

        [ForeignKey("tbl_Proj_EstimationID")]
        public tbl_org_proj_estimation tbl_org_proj_estimation { get; set; }
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        [ForeignKey("ParameterID")]
        public tbl_org_estm_parameters tbl_org_estm_parameters { get; set; }
    }
}

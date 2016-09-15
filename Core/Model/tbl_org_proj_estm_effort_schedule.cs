using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_estm_effort_schedule : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Proj_EstimationID { get; set; }
        public int tbl_Org_PhaseID { get; set; }
        public byte SequenceNo { get; set; }
        public decimal EffortPercent { get; set; }
        public Nullable<decimal> PDs { get; set; }
        public Nullable<decimal> Sch_People { get; set; }
        public Nullable<decimal> Sch_Days { get; set; }
        public Nullable<DateTime> Sch_StartDate { get; set; }
        public Nullable<DateTime> Sch_EndDate { get; set; }
        public Nullable<decimal> Sch_Holidays { get; set; }
        public bool IsCutPhase { get; set; }
        public Nullable<decimal> Defects { get; set; }
        public string Roles { get; set; }
        public string RoleIDs { get; set; }
        public Nullable<decimal> SBR { get; set; }
        public Nullable<decimal> PM_Loading { get; set; }
        public Nullable<decimal> PM_Effort { get; set; }
        public Nullable<decimal> PM_Cost { get; set; }
        public Nullable<decimal> Other_Costs { get; set; }

        [ForeignKey("tbl_Proj_EstimationID")]
        public tbl_org_proj_estimation tbl_org_proj_estimation { get; set; }
        [ForeignKey("tbl_Org_PhaseID")]
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
    }
}

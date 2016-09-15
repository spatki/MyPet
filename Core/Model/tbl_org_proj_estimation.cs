using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_estimation : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public short Version { get; set; }
        public Nullable<bool> CurrentVersion { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int tbl_Org_Proj_PhaseID { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Size { get; set; }
        public Nullable<decimal> SimpleEfforts { get; set; }
        public Nullable<decimal> MediumEfforts { get; set; }
        public Nullable<decimal> ComplexEfforts { get; set; }
        public Nullable<decimal> TotalEfforts { get; set; }
        public Nullable<decimal> ScheduleMonths { get; set; }
        public Nullable<decimal> TDI { get; set; }
        public Nullable<decimal> VAF { get; set; }
        public Nullable<decimal> CUT_Effort_FPs { get; set; }
        public Nullable<decimal> CUT_Effort_PDs { get; set; }
        public Nullable<decimal> CUT_EffortPercent { get; set; }
        public Nullable<decimal> CUT_TeamSize { get; set; }
        public Nullable<decimal> Overall_Project_PDs { get; set; }
        public Nullable<decimal> Team_Productivity { get; set; }
        public Nullable<decimal> DefectDensity { get; set; }
        public Nullable<decimal> TotalDefects { get; set; }
        public Nullable<decimal> TotalCutDefects { get; set; }
        public Nullable<decimal> PMSBR { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("tbl_Org_Proj_PhaseID")]
        public mstr_org_phase_in_proj mstr_org_phase_in_proj { get; set; }
        public ICollection<tbl_org_proj_estm_size> tbl_org_proj_estm_size { get; set; }
        public ICollection<tbl_org_proj_estm_gsc> tbl_org_proj_estm_gsc { get; set; }
        public ICollection<tbl_org_proj_estm_productivity> tbl_org_proj_estm_productivity { get; set; }
        public ICollection<tbl_org_proj_estm_effort_schedule> tbl_org_proj_estm_effort_schedule { get; set; }
    }
}

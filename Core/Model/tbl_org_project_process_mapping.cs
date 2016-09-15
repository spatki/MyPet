using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_project_process_mapping : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_TaskID { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TreatAsTask { get; set; }
        public Nullable<bool> TailorNew { get; set; }

        [ForeignKey("tbl_Process_RepositoryID")]
        public tbl_process_repository tbl_process_repository { get; set; }
        [ForeignKey("mstr_Org_Proj_PhaseID")]
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        [ForeignKey("tbl_Process_TaskID")]
        public tbl_process_rep_task tbl_process_rep_task { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
    }
}

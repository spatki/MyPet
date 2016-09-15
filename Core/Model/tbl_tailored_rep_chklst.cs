using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_tailored_rep_chklst: Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public int tbl_Process_ChecklistID { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TailorNew { get; set; }

        [ForeignKey("tbl_Process_ChecklistID")]
        public tbl_process_checklist tbl_process_checklist { get; set; } 
    }
}

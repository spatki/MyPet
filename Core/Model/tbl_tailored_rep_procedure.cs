using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_tailored_rep_procedure: Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public int tbl_Process_ProcedureID { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TailorNew { get; set; }

        [ForeignKey("tbl_Process_ProcedureID")]
        public tbl_process_procedure tbl_process_procedure { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_tailored_rep_document: Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public int tbl_Process_DocumentID { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TailorNew { get; set; }

        [ForeignKey("tbl_Process_DocumentID")]
        public tbl_process_document tbl_process_document { get; set; }
    }
}

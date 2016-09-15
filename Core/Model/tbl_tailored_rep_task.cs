using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_tailored_rep_task : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> Ref_Rep_TaskID { get; set; }
        public short SequenceNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short Type { get; set; }
        public Nullable<int> DefaultStatus { get; set; }
        public Nullable<int> CompletionStatus { get; set; }
        public Nullable<short> DefaultHrs { get; set; }
        public Nullable<short> DefaultMins { get; set; }
        public Nullable<short> DefaultDurationDays { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TailorNew { get; set; }

        public ICollection<tbl_tailored_rep_task_ref_docs> tbl_tailored_rep_task_ref_docs { get; set; }
        [ForeignKey("Ref_Rep_TaskID")]
        public tbl_process_rep_task tbl_process_rep_task { get; set; }
    }
}

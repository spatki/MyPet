using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_tailored_general_task: Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte SequenceNo { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public Nullable<int> Ref_General_TaskID { get; set; }
        public string mstr_Process_Role_Ids { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TailorNew { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_chklst_revision: Entity
    {
        public int tbl_Process_ChecklistID { get; set; }
        public string Comments { get; set; }
        public int RevisionUser { get; set; }
        public System.DateTime RevisionDate { get; set; }
        public Nullable<int> ReviewUser { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<int> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string version { get; set; }
    }
}

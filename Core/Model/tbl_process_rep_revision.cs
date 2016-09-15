using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_rep_revision: Entity
    {
        public string Comments { get; set; }
        public int Revision_User { get; set; }
        public System.DateTime Revision_Date { get; set; }
        public Nullable<int> Review_User { get; set; }
        public Nullable<System.DateTime> Review_Date { get; set; }
        public Nullable<System.DateTime> Publish_Date { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string version { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("tbl_Process_RepositoryID")]
        public virtual tbl_process_repository tbl_process_repository { get; set; }
    }
}

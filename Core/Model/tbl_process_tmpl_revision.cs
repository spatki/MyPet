using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_tmpl_revision: Entity
    {
        public int tbl_Process_TemplateID { get; set; }
        public string Comments { get; set; }
        public int RevisionUser { get; set; }
        public System.DateTime RevisionDate { get; set; }
        public Nullable<int> ReviewUser { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string version { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("tbl_Process_TemplateID")]
        public virtual tbl_process_template tbl_process_template { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_review_history : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Comments { get; set; }
        public int UserID { get; set; }
        public int StatusID { get; set; }

        [ForeignKey("StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public virtual tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("UserID")]
        public virtual UserProfile UserProfile { get; set; }
    }
}

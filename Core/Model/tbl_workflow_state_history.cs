using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_workflow_state_history : Entity
    {
        public int UserID { get; set; }
        public string FunctionID { get; set; }
        public int RefID { get; set; }
        public int Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string ReviewComments { get; set; }

        [ForeignKey("UserID")]
        public UserProfile UserProfile { get; set; }
        [ForeignKey("Status")]
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
    }
}

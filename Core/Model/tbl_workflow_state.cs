using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_workflow_state : Entity
    {
        public int RefID { get; set; }
        public string FromUserID { get; set; }
        public int UserID { get; set; }
        public string FunctionID { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}

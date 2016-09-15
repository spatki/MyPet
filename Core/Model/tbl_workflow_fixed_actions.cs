using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_workflow_fixed_actions : Entity
    {
        public string FunctionID { get; set; }
        public int PreStatusID { get; set; }
        public string ActionName { get; set; }
        public int StatusID { get; set; }
        public bool CascadeUpdate { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_workflow_functions
    {
        public int ID { get; set; }
        public string FunctionID { get; set; }
        public string FriendlyName { get; set; }
        public short StatusTypeID { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_workflow_implementation
    {
        public short ID { get; set; }
        public string FunctionID { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string ActionName { get; set; }
        public bool IsCreate { get; set; }
        public bool IsReview { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsReviewAccepted { get; set; }
        public bool IsResolved { get; set; }
    }
}

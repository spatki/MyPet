using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tailored_general_tasks
    {
        public int ID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

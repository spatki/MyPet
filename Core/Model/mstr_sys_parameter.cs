using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_sys_parameter: Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Nullable<int> Created_By { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<int> Updated_By { get; set; }
        public Nullable<System.DateTime> Update_Date { get; set; }
    }
}

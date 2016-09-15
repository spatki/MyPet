using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_org_client : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public short Type { get; set; }
        public string PrimaryContact { get; set; }
        public string PContactMailID { get; set; }
    }
}

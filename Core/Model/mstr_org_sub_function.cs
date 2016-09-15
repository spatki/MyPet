using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_org_sub_function: Entity
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<int> PrimaryResponsible { get; set; }

        public virtual ICollection<mstr_org_sub_in_function> mstr_org_sub_in_function { get; set; }
    }
}

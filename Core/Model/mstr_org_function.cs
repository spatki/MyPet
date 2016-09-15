using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{    
    public partial class mstr_org_function: Entity
    {
        public mstr_org_function()
        {
            this.mstr_org_function_level = new HashSet<mstr_org_function_level>();
            this.mstr_org_sub_in_function = new HashSet<mstr_org_sub_in_function>();
        }
    
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<int> PrimaryResponsible { get; set; }
    
        public virtual ICollection<mstr_org_function_level> mstr_org_function_level { get; set; }
        public virtual ICollection<mstr_org_sub_in_function> mstr_org_sub_in_function { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{    
    public partial class mstr_org_project_type: Entity
    {
        public mstr_org_project_type()
        {
            this.mstr_org_phase_in_proj = new HashSet<mstr_org_phase_in_proj>();
        }
    
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<mstr_org_phase_in_proj> mstr_org_phase_in_proj { get; set; }
    }
}
    
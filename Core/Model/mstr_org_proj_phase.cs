using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{    
    public partial class mstr_org_proj_phase : Entity
    {
        public mstr_org_proj_phase()
        {
            this.mstr_org_phase_in_proj = new HashSet<mstr_org_phase_in_proj>();
        }

        public short SequenceNo { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<short> PercentTime { get; set; }

        public virtual ICollection<mstr_org_phase_in_proj> mstr_org_phase_in_proj { get; set; }
    }
}

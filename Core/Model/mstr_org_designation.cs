using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model 
{
    using System;
    using System.Collections.Generic;
    
    public partial class mstr_org_designation: Entity
    {
        public mstr_org_designation()
        {
            this.mstr_org_structure = new HashSet<mstr_org_structure>();
        }
    
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        public virtual ICollection<mstr_org_structure> mstr_org_structure { get; set; }
    }
}

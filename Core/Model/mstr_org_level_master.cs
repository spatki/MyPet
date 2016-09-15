using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{    
    public partial class mstr_org_level_master: Entity
    {
        public mstr_org_level_master()
        {
            this.mstr_access_level_data = new HashSet<mstr_access_level_data>();
            this.mstr_org_function_level = new HashSet<mstr_org_function_level>();
        }
    
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<bool> DefaultAccessGrant { get; set; }
        public int mstr_Org_LevelID { get; set; }
    
        public virtual ICollection<mstr_access_level_data> mstr_access_level_data { get; set; }
        public virtual ICollection<mstr_org_function_level> mstr_org_function_level { get; set; }

        [ForeignKey("mstr_Org_LevelID")]
        public virtual mstr_org_level mstr_org_level { get; set; }
    }
}

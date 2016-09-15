using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{    
    public partial class mstr_org_function_level: Entity
    {
        public short mstr_Org_LevelID { get; set; }
        public int mstr_Org_Level_MasterID { get; set; }
        public short mstr_Org_FunctionID { get; set; }
    
        public virtual mstr_org_function mstr_org_function { get; set; }
        public virtual mstr_org_level mstr_org_level { get; set; }
        public virtual mstr_org_level_master mstr_org_level_master { get; set; }
    }
}

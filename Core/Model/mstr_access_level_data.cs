using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_access_level_data: Entity
    {
        public short mstr_Access_TypeID { get; set; }
        public short mstr_Org_LevelID { get; set; }
        public Nullable<int> mstr_Org_Level_MasterID { get; set; }
    
        public virtual mstr_access_type mstr_access_type { get; set; }
        public virtual mstr_org_level mstr_org_level { get; set; }
        public virtual mstr_org_level_master mstr_org_level_master { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_mapping_level : Entity
    {
        public short mstr_Process_LevelID { get; set; }
        public short mstr_Org_LevelID { get; set; }

        public virtual mstr_org_level mstr_org_level { get; set; }
        public virtual mstr_process_level mstr_process_level { get; set; }
    }
}

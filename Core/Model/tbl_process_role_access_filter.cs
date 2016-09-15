using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_role_access_filter: Entity
    {
        public Nullable<int> tbl_Process_Role_Level_AccessID { get; set; }
        public int mstr_Process_Level_MasterID { get; set; }

        [ForeignKey("mstr_Process_Level_MasterID")]
        public virtual mstr_process_level_master mstr_process_level_master { get; set; }
        [ForeignKey("tbl_Process_Role_Level_AccessID")]
        public virtual tbl_process_role_level_access tbl_process_role_level_access { get; set; }
    }
}

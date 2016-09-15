using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_role_level_access: Entity
    {
        public tbl_process_role_level_access()
        {
            this.tbl_process_role_access_filter = new HashSet<tbl_process_role_access_filter>();
        }
    
        public short mstr_Process_LevelID { get; set; }
        public short mstr_Process_RoleID { get; set; }
    
        public virtual mstr_process_level mstr_process_level { get; set; }
        public virtual mstr_process_role mstr_process_role { get; set; }
        public virtual ICollection<tbl_process_role_access_filter> tbl_process_role_access_filter { get; set; }
    }
}

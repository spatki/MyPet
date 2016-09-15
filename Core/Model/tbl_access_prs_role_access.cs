using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_access_prs_role_access: Entity
    {
        public short mstr_Access_MenuID { get; set; }
        public short mstr_Access_TypeID { get; set; }
        public short mstr_Process_RoleID { get; set; }
    
        public virtual mstr_access_menu mstr_access_menu { get; set; }
        public virtual mstr_access_type mstr_access_type { get; set; }
        public virtual mstr_process_role mstr_process_role { get; set; }
    }
}

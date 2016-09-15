using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_mapping_role: Entity
    {
        public int mstr_Process_RoleID { get; set; }
        public int mstr_Org_RoleID { get; set; }

        [ForeignKey("mstr_Process_RoleID")]        
        public virtual mstr_process_role mstr_process_role { get; set; }
        [ForeignKey("mstr_Org_RoleID")]
        public virtual mstr_org_role mstr_org_role { get; set; }
    }
}

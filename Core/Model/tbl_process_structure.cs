using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_structure: Entity
    {
        public int mstr_Process_RoleID { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public Nullable<int> ParentRoleID { get; set; }
        public short Sequence { get; set; }
        public string StructPath { get; set; }
        public string Comments { get; set; }

        [ForeignKey("mstr_Process_RoleID")]
        public mstr_process_role mstr_process_role { get; set; }
        [ForeignKey("ParentRoleID")]
        public virtual tbl_process_structure ParentRole { get; set; }
    }
}

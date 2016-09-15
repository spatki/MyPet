using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_gt_role: Entity
    {
        public short mstr_Process_RoleID { get; set; }
        public int tbl_Process_General_TasksID { get; set; }
    }
}

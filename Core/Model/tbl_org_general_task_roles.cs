using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_general_task_roles : Entity
    {
        public int tbl_Org_General_TaskID { get; set; }
        public int tbl_Org_RoleID { get; set; }

        [ForeignKey("tbl_Org_General_TaskID")]
        public mstr_org_general_tasks mstr_org_general_tasks { get; set; }
        [ForeignKey("tbl_Org_RoleID")]
        public mstr_org_role mstr_org_role { get; set; }
    }
}

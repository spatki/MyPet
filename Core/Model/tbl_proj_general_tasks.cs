using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_proj_general_tasks : Entity
    {
        public int tbl_Org_General_TaskID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<bool> Global { get; set; }
        public short Sequence { get; set; }
        public Nullable<bool> Tailored { get; set; }

        [ForeignKey("tbl_Org_General_TaskID")]
        public mstr_org_general_tasks mstr_org_general_tasks { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
        public ICollection<tbl_proj_general_task_roles> tbl_proj_general_task_roles { get; set; }
    }
}

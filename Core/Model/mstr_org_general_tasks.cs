using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class mstr_org_general_tasks : Entity
    {
        public string Task { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Global { get; set; }
        public Nullable<int> OwnedByProject { get; set; }
        public short Sequence { get; set; }

        public ICollection<tbl_org_general_task_roles> tbl_org_general_task_roles { get; set; }
        [ForeignKey("OwnedByProject")]
        public tbl_org_project tbl_org_project { get; set; }
    }
}

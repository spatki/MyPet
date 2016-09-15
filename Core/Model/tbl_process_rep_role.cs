using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_rep_role: Entity
    {
        public short mstr_Process_RoleID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string Access { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    
        public virtual mstr_process_role mstr_process_role { get; set; }
        public virtual tbl_process_repository tbl_process_repository { get; set; }
    }
}

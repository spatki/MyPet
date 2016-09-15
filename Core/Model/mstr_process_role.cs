using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_process_role: Entity
    {
        public mstr_process_role()
        {
            this.mstr_org_role = new HashSet<mstr_org_role>();
            this.tbl_access_prs_role_access = new HashSet<tbl_access_prs_role_access>();
            this.tbl_process_rep_role = new HashSet<tbl_process_rep_role>();
            this.tbl_process_role_level_access = new HashSet<tbl_process_role_level_access>();
            this.tbl_process_structure = new HashSet<tbl_process_structure>();
        }
    
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public short Type { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    
        public virtual ICollection<mstr_org_role> mstr_org_role { get; set; }
        public virtual ICollection<tbl_access_prs_role_access> tbl_access_prs_role_access { get; set; }
        public virtual ICollection<tbl_process_rep_role> tbl_process_rep_role { get; set; }
        public virtual ICollection<tbl_process_role_level_access> tbl_process_role_level_access { get; set; }
        public virtual ICollection<tbl_process_structure> tbl_process_structure { get; set; }
    }
}

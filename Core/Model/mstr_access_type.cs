using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{    
    public partial class mstr_access_type: Entity
    {
        public mstr_access_type()
        {
            this.mstr_access_level_data = new HashSet<mstr_access_level_data>();
            this.tbl_access_prs_role_access = new HashSet<tbl_access_prs_role_access>();
        }
    
        public string DisplayName { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool SelfData { get; set; }
        public bool LevelwiseDate { get; set; }
        public bool ReporteeData { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    
        public virtual ICollection<mstr_access_level_data> mstr_access_level_data { get; set; }
        public virtual ICollection<tbl_access_prs_role_access> tbl_access_prs_role_access { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_org_level : Entity
    {
        public mstr_org_level()
        {
            this.mstr_access_level_data = new HashSet<mstr_access_level_data>();
            this.mstr_org_function_level = new HashSet<mstr_org_function_level>();
            this.tbl_mapping_level = new HashSet<tbl_mapping_level>();
            this.mstr_org_level_master = new HashSet<mstr_org_level_master>();
        }

        public short LevelSequence { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        public virtual ICollection<mstr_access_level_data> mstr_access_level_data { get; set; }
        public virtual ICollection<mstr_org_function_level> mstr_org_function_level { get; set; }
        public virtual ICollection<tbl_mapping_level> tbl_mapping_level { get; set; }
        public virtual ICollection<mstr_org_level_master> mstr_org_level_master { get; set; }
    }
}

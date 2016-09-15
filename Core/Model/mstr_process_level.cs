using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_process_level : Entity
    {
        public mstr_process_level()
        {
            this.tbl_mapping_level = new HashSet<tbl_mapping_level>();
            this.mstr_process_level_master = new HashSet<mstr_process_level_master>();
            this.tbl_process_repository = new HashSet<tbl_process_repository>();
            this.tbl_process_role_level_access = new HashSet<tbl_process_role_level_access>();
        }
        public short LevelSequence { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        public virtual ICollection<tbl_mapping_level> tbl_mapping_level { get; set; }
        public virtual ICollection<mstr_process_level_master> mstr_process_level_master { get; set; }
        public virtual ICollection<tbl_process_repository> tbl_process_repository { get; set; }
        public virtual ICollection<tbl_process_role_level_access> tbl_process_role_level_access { get; set; }
    }
}

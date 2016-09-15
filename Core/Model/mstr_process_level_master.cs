using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_process_level_master : Entity
    {
        public mstr_process_level_master()
        {
            this.tbl_process_role_access_filter = new HashSet<tbl_process_role_access_filter>();
        }

        public int mstr_Process_level_ID { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [ForeignKey("mstr_Process_level_ID")]
        public virtual mstr_process_level mstr_process_level { get; set; }
        public virtual ICollection<tbl_process_role_access_filter> tbl_process_role_access_filter { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_procedure: Entity
    {
        public tbl_process_procedure()
        {
            this.tbl_process_proc_revision = new List<tbl_process_proc_revision>();
            this.tbl_process_proc_section = new List<tbl_process_proc_section>();
            this.tbl_process_rep_procedure = new List<tbl_process_rep_procedure>();
            this.tbl_process_proc_group = new List<tbl_process_proc_group>();
        }
    
        public string Name { get; set; }
        public int mstr_Process_TypeID { get; set; }
        public string Mission { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public string TailorComments { get; set; }
        public Nullable<bool> AvailableToAll { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual UserProfile UserProfile { get; set; }
        [ForeignKey("ReviewedBy")]
        public virtual UserProfile UserProfile1 { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual UserProfile UserProfile2 { get; set; }
        [ForeignKey("mstr_Process_TypeID")]
        public virtual mstr_process_type mstr_process_type { get; set; }
        public virtual ICollection<tbl_process_proc_revision> tbl_process_proc_revision { get; set; }
        public virtual ICollection<tbl_process_proc_section> tbl_process_proc_section { get; set; }
        public virtual ICollection<tbl_process_rep_procedure> tbl_process_rep_procedure { get; set; }
        public virtual ICollection<tbl_process_proc_group> tbl_process_proc_group { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_process_lc_status: Entity
    {
        public mstr_process_lc_status()
        {
            this.mstr_org_designation = new HashSet<mstr_org_designation>();
            this.tbl_process_checklist = new HashSet<tbl_process_checklist>();
            this.tbl_process_proc_revision = new HashSet<tbl_process_proc_revision>();
            this.tbl_process_procedure = new HashSet<tbl_process_procedure>();
            this.tbl_process_rep_revision = new HashSet<tbl_process_rep_revision>();
            this.tbl_process_template = new HashSet<tbl_process_template>();
            this.tbl_process_tmpl_revision = new HashSet<tbl_process_tmpl_revision>();
            this.tbl_org_employee = new HashSet<tbl_org_employee>();
        }
    
        public string Status { get; set; }
        public string Description { get; set; }
        public short Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsComplete { get; set; }
        public bool IsPublish { get; set; }
        public Nullable<bool> IsInactive { get; set; }
        public Nullable<bool> IsReview { get; set; }
        public byte SequenceNo { get; set; }
        public Nullable<int> Prev_Status { get; set; }
        public Nullable<int> Next_Status { get; set; }
        public Nullable<int> DefaultReviewRoleID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    
        public virtual ICollection<mstr_org_designation> mstr_org_designation { get; set; }
        public virtual ICollection<tbl_process_checklist> tbl_process_checklist { get; set; }
        public virtual ICollection<tbl_process_proc_revision> tbl_process_proc_revision { get; set; }
        public virtual ICollection<tbl_process_procedure> tbl_process_procedure { get; set; }
        public virtual ICollection<tbl_process_rep_revision> tbl_process_rep_revision { get; set; }
        public virtual ICollection<tbl_process_template> tbl_process_template { get; set; }
        public virtual ICollection<tbl_process_tmpl_revision> tbl_process_tmpl_revision { get; set; }
        public virtual ICollection<tbl_org_employee> tbl_org_employee { get; set; }
        public virtual ICollection<tbl_org_project> tbl_org_project { get; set; }
    }
}

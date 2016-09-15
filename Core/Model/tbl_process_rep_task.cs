using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_rep_task: Entity
    {
        public tbl_process_rep_task()
        {
            this.tbl_process_rep_rule = new HashSet<tbl_process_rep_rule>();
        }

        public int tbl_Process_RepositoryID { get; set; }
        public short SequenceNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Type { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> DefaultStatus { get; set; }
        public Nullable<int> CompletionStatus { get; set; }
        public Nullable<byte> DefaultHRS { get; set; }
        public Nullable<byte> DefaultMINS { get; set; }
        public Nullable<short> DefaultDurationDays { get; set; }
        public Nullable<bool> AssignmentMandatory { get; set; }
        public Nullable<bool> AsynExec { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> Tailored { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("DefaultStatus")]
        public virtual mstr_process_lc_status mstr_process_lc_status1 { get; set; }
        [ForeignKey("CompletionStatus")]
        public virtual mstr_process_lc_status mstr_process_lc_status2 { get; set; }
        [ForeignKey("tbl_Process_RepositoryID")]
        public virtual tbl_process_repository tbl_process_repository { get; set; }
        public virtual ICollection<tbl_process_rep_rule> tbl_process_rep_rule { get; set; }
        public virtual ICollection<tbl_process_rep_task_ref_docs> tbl_process_rep_task_ref_docs { get; set; }
        public virtual ICollection<tbl_mapping_org_process> tbl_mapping_org_process { get; set; }
        public virtual ICollection<tbl_org_proj_plan> tbl_org_proj_plan { get; set; }

    }
}

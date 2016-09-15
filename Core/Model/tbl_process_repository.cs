using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_repository :Entity
    {
        public tbl_process_repository()
        {
            this.tbl_process_general_task = new HashSet<tbl_process_general_task>();
            this.tbl_process_rep_document = new HashSet<tbl_process_rep_document>();
            this.tbl_process_rep_procedure = new HashSet<tbl_process_rep_procedure>();
            this.tbl_process_rep_revision = new HashSet<tbl_process_rep_revision>();
            this.tbl_process_rep_role = new HashSet<tbl_process_rep_role>();
            this.tbl_process_rep_template = new HashSet<tbl_process_rep_template>();
            this.tbl_process_rep_chklst = new HashSet<tbl_process_rep_chklst>();
            this.tbl_process_rep_task = new HashSet<tbl_process_rep_task>();
        }

        public Nullable<short> Sequence { get; set; }
        public string Name { get; set; }
        public bool IsParent { get; set; }
        public int Level { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string StructPath { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public Nullable<byte> TailorReference { get; set; }
        public Nullable<int> TailorID { get; set; }
        public string TailorComments { get; set; }
        public int mstr_Process_LevelID { get; set; }
        public Nullable<bool> TreatAsTask { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [ForeignKey("mstr_Process_LevelID")]
        public virtual mstr_process_level mstr_process_level { get; set; }
        public virtual ICollection<tbl_process_general_task> tbl_process_general_task { get; set; }
        public virtual ICollection<tbl_process_rep_document> tbl_process_rep_document { get; set; }
        public virtual ICollection<tbl_process_rep_procedure> tbl_process_rep_procedure { get; set; }
        public virtual ICollection<tbl_process_rep_revision> tbl_process_rep_revision { get; set; }
        public virtual ICollection<tbl_process_rep_role> tbl_process_rep_role { get; set; }
        public virtual ICollection<tbl_process_rep_template> tbl_process_rep_template { get; set; }
        public virtual ICollection<tbl_process_rep_chklst> tbl_process_rep_chklst { get; set; }
        public virtual ICollection<tbl_process_rep_task> tbl_process_rep_task { get; set; }
        public virtual ICollection<tbl_org_proj_plan> tbl_org_proj_plan { get; set; }
    }
}

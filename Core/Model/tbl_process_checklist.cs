using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_checklist: Entity
    {
        public tbl_process_checklist()
        {
            this.tbl_process_chklst_revision = new List<tbl_process_chklst_revision>();
            this.tbl_process_chklst_item = new List<tbl_process_chklst_item>();
            this.tbl_process_rep_chklst = new List<tbl_process_rep_chklst>();
            this.tbl_process_chklst_group = new List<tbl_process_chklst_group>();
        }
         public string Name { get; set; }
        public string Description { get; set; }
        public string C_SNO { get; set; }
        public Nullable<short> C_SNO_Len { get; set; }
        public string C_ChkPt { get; set; }
        public Nullable<short> C_ChkPt_Len { get; set; }
        public string C_Result { get; set; }
        public Nullable<short> C_Result_Len { get; set; }
        public string C_Remarks { get; set; }
        public Nullable<short> C_Remarks_Len { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public string TailorComments { get; set; }
        public Nullable<bool> AvailableToAll { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual UserProfile UserProfile { get; set; }
        [ForeignKey("ReviewedBy")]
        public virtual UserProfile UserProfile1 { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual UserProfile UserProfile2 { get; set; }
        public virtual ICollection<tbl_process_chklst_revision> tbl_process_chklst_revision { get; set; }
        public virtual ICollection<tbl_process_chklst_item> tbl_process_chklst_item { get; set; }
        public virtual ICollection<tbl_process_rep_chklst> tbl_process_rep_chklst { get; set; }
        public virtual ICollection<tbl_process_chklst_group> tbl_process_chklst_group { get; set; }

    }
}

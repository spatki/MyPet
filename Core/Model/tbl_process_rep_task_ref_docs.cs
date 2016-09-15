using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_rep_task_ref_docs: Entity
    {
        public int tbl_Process_Rep_TaskID { get; set; }
        public int tbl_Process_Repository_ID { get; set; }
        public short DocType { get; set; }

        public Nullable<int> tbl_Process_Rep_DocumentID { get; set; }
        public Nullable<int> tbl_Process_Rep_ChklstID { get; set; }
        public Nullable<int> tbl_Process_Rep_ProcID { get; set; }
        public Nullable<int> tbl_Process_Rep_TmplID { get; set; }
        public Nullable<short> Mandatory { get; set; }
        public string Remarks { get; set; }

        [ForeignKey("tbl_Process_Repository_ID")]
        public tbl_process_repository tbl_process_repository { get; set; }
        [ForeignKey("tbl_Process_Rep_DocumentID")]
        public tbl_process_rep_document tbl_process_rep_document { get; set; }
        [ForeignKey("tbl_Process_Rep_ProcID")]
        public tbl_process_rep_procedure tbl_process_rep_procedure { get; set; }
        [ForeignKey("tbl_Process_Rep_TmplID")]
        public tbl_process_rep_template tbl_process_rep_template { get; set; }
        [ForeignKey("tbl_Process_Rep_ChklstID")]
        public tbl_process_rep_chklst tbl_process_rep_chklst { get; set; }
        [ForeignKey("tbl_Process_Rep_TaskID")]
        public tbl_process_rep_task tbl_process_rep_task { get; set; }

    }
}

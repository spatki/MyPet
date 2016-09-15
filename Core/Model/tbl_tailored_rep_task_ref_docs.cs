using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_tailored_rep_task_ref_docs : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Tailored_Rep_TaskID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_Rep_Task_RefID { get; set; }
        public short DocType { get; set; }
        public string Name { get; set; }
        public Nullable<int> tbl_Process_DocumentID { get; set; }
        public Nullable<int> tbl_Process_ChecklistID { get; set; }
        public Nullable<int> tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_TemplateID { get; set; }
        public Nullable<short> Mandatory { get; set; }
        public string Remarks { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TailorNew { get; set; }
        public Nullable<bool> RefDeleted { get; set; }

        [ForeignKey("tbl_Tailored_Rep_TaskID")]
        public tbl_tailored_rep_task tbl_tailored_rep_task { get; set; }
        [ForeignKey("tbl_Process_DocumentID")]
        public tbl_process_document tbl_process_document { get; set; }
        [ForeignKey("tbl_Process_ProcedureID")]
        public tbl_process_procedure tbl_process_procedure { get; set; }
        [ForeignKey("tbl_Process_TemplateID")]
        public tbl_process_template tbl_process_template { get; set; }
        [ForeignKey("tbl_Process_ChecklistID")]
        public tbl_process_checklist tbl_process_checklist { get; set; } 
    }

}

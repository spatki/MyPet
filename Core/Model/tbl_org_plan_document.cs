using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_plan_document : Entity
    {
        public int tbl_Org_PlanID { get; set; }
        public string Name { get; set; }
        public byte DocType { get; set; }
        public Nullable<int> tbl_DocMgr_DocumentID { get; set; }
        public Nullable<int> tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_ChecklistID { get; set; }
        public Nullable<int> tbl_Process_TemplateID { get; set; }
        public byte Source { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<byte> ReferenceType { get; set; }

        [ForeignKey("tbl_Org_PlanID")]
        public tbl_org_proj_plan tbl_org_proj_plan { get; set; }
        [ForeignKey("tbl_DocMgr_DocumentID")]
        public tbl_docmgr_document tbl_docmgr_document { get; set; }
        [ForeignKey("tbl_Process_ProcedureID")]
        public tbl_process_procedure tbl_process_procedure { get; set; }
        [ForeignKey("tbl_Process_ChecklistID")]
        public tbl_process_checklist tbl_process_checklist { get; set; }
        [ForeignKey("tbl_Process_TemplateID")]
        public tbl_process_template tbl_process_template { get; set; }
        public ICollection<tbl_org_plan_filled_document> tbl_org_plan_filled_document { get; set; }
    }
}

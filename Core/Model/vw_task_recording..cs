using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_task_recording : Entity
    {
        public int projectID { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> groupID { get; set; }
        public string groupName { get; set; }
        public int planID { get; set; }
        public string TaskName { get; set; }
        public DateTime taskPlannedStart { get; set; }
        public Nullable<DateTime> taskPlannedEnd { get; set; }
        public decimal PlannedDuration { get; set; }
        public Nullable<DateTime> taskActualStart { get; set; }
        public Nullable<DateTime> taskActualEnd { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string TaskStatus { get; set; }
        public Nullable<int> documentID { get; set; }
        public Nullable<byte> DocType { get; set; }
        public Nullable<byte> ReferenceType { get; set; }
        public string Remarks { get; set; }
        public Nullable<byte> Source { get; set; }
        public Nullable<int> tbl_DocMgr_DocumentID { get; set; }
        public Nullable<int> tbl_Process_ChecklistID { get; set; }
        public Nullable<int> tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_TemplateID { get; set; }
        public Nullable<int> filled_DocID { get; set; }
        public Nullable<byte> filledType { get; set; }
        public Nullable<int> filled_Uploaded_Doc { get; set; }
        public Nullable<int> filled_UploadedBy { get; set; }
        public Nullable<DateTime> filled_UploadedDate { get; set; }
        public string docName { get; set; }
    }
}

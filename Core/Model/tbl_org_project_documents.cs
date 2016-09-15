using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_project_documents: Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Process_Doc_TypeID { get; set; }
        public Nullable<int> tbl_docmgr_documentID { get; set; }
        public string Name { get; set; }
        public Nullable<byte> SourceType { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<int> ProcessDoc_RefID { get; set; }
        public string Contents { get; set; }
        public string Owner { get; set; }
        public string Comments { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public virtual tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("mstr_Process_Doc_TypeID")]
        public virtual mstr_process_doc_type mstr_process_doc_type { get; set; }
        [ForeignKey("tbl_docmgr_documentID")]
        public virtual tbl_docmgr_document tbl_docmgr_document { get; set; }                
    }
}

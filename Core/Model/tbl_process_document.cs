using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_document: Entity
    {
        public tbl_process_document()
        {
            this.tbl_process_doc_revision = new HashSet<tbl_process_doc_revision>();
        }
    
        public Nullable<byte> mstr_Process_Doc_TypeID { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public Nullable<short> mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> tbl_DocMgr_DocumentID { get; set; }

        [ForeignKey("tbl_DocMgr_DocumentID")]
        public virtual tbl_docmgr_document tbl_docmgr_document { get; set; }
        public virtual ICollection<tbl_process_doc_revision> tbl_process_doc_revision { get; set; }
    }
}

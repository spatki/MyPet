using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_doc_revision: Entity
    {
        public string Comments { get; set; }
        public Nullable<int> RevisionUser { get; set; }
        public Nullable<System.DateTime> RevisionDate { get; set; }
        public Nullable<int> ReviewUser { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public Nullable<short> mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> tbl_Process_DocumentID { get; set; }
    
        public virtual tbl_process_document tbl_process_document { get; set; }
    }
}

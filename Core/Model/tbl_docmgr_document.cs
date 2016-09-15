using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_docmgr_document: Entity
    {
        public tbl_docmgr_document()
        {
            this.tbl_docmgr_version = new HashSet<tbl_docmgr_version>();
            this.tbl_process_document = new HashSet<tbl_process_document>();
        }
    
        public string Name { get; set; }
        public string FileLocation { get; set; }
        public Nullable<decimal> Version { get; set; }
        public Nullable<bool> ReadWriteMode { get; set; }
        public string UploadKey { get; set; }
        public string DownloadKey { get; set; }
        public string ViewKey { get; set; }
        public Nullable<byte> StorageMode { get; set; }
        public string Classification { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
    
        public virtual ICollection<tbl_docmgr_version> tbl_docmgr_version { get; set; }
        public virtual ICollection<tbl_process_document> tbl_process_document { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_docmgr_version: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public Nullable<decimal> version { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateUser { get; set; }
        public string Comments { get; set; }
        public string FileLocation { get; set; }
        public Nullable<bool> ReadWriteMode { get; set; }
        public Nullable<int> tbl_DocMgr_DocumentID { get; set; }
        public string UploadKey { get; set; }
        public string DownloadKey { get; set; }
        public string ViewKey { get; set; }
        public Nullable<byte> StorageMode { get; set; }
    
        public virtual tbl_docmgr_document tbl_docmgr_document { get; set; }
    }
}

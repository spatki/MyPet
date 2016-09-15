using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_rep_document: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public int tbl_Process_DocumentID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public string TailorComments { get; set; }
        public Nullable<bool> AvailableToAll { get; set; }
        public Nullable<byte> TailorReference { get; set; }
        public Nullable<int> TailorID { get; set; }

        [ForeignKey("tbl_Process_RepositoryID")]
        public virtual tbl_process_repository tbl_process_repository { get; set; }
        [ForeignKey("tbl_Process_DocumentID")]
        public virtual tbl_process_document tbl_process_document { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_general_task: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte SequenceNo { get; set; }
        public string mstr_Process_Role_Ids { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    
        public virtual tbl_process_repository tbl_process_repository { get; set; }
    }
}

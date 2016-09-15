using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_rep_procedure: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_Rep_ProcessID { get; set; }

        [ForeignKey("tbl_Process_ProcedureID")]
        public virtual tbl_process_procedure tbl_process_procedure { get; set; }
        [ForeignKey("tbl_Process_Rep_ProcessID")]
        public virtual tbl_process_rep_task tbl_process_rep_process { get; set; }
        [ForeignKey("tbl_Process_RepositoryID")]
        public virtual tbl_process_repository tbl_process_repository { get; set; }
    }
}

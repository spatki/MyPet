using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_proc_section: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public short SequenceNo { get; set; }
        [Required(ErrorMessage = "Enter the Title")]
        [StringLength(50)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Enter the Section Details")]
        public string Detail { get; set; }
        public int tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_Proc_GroupID { get; set; }

        [ForeignKey("tbl_Process_Proc_GroupID")]
        public virtual tbl_process_proc_group tbl_process_proc_group { get; set; }
        [ForeignKey("tbl_Process_ProcedureID")]
        public virtual tbl_process_procedure tbl_process_procedure { get; set; }
    }
}

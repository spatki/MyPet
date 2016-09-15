using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_chklst_item: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Process_ChecklistID { get; set; }
        [Required(ErrorMessage = "Enter the checklist description")]
        public string ItemDescription { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<int> tbl_Process_Chklst_GroupID { get; set; }
        [Required(ErrorMessage = "Enter the options")]
        public string Chklst_Options { get; set; }
        public string Remarks { get; set; }

        [ForeignKey("tbl_Process_Chklst_GroupID")]
        public virtual tbl_process_proc_group tbl_process_chklst_group { get; set; }
        [ForeignKey("tbl_Process_ChecklistID")]
        public virtual tbl_process_procedure tbl_process_checklist { get; set; }

    }
}

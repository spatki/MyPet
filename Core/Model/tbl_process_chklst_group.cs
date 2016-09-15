using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_chklst_group : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int tbl_Process_ChecklistID { get; set; }
        public string Name { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<short> ParentGroup { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_proc_group: Entity
    {
        public tbl_process_proc_group()
        {
            this.tbl_process_proc_section = new HashSet<tbl_process_proc_section>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Name { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<short> ParentGroup { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public int tbl_Process_ProcedureID { get; set; }
    
        public virtual ICollection<tbl_process_proc_section> tbl_process_proc_section { get; set; }
    }
}

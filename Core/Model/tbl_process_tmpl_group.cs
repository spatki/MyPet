using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_tmpl_group: Entity
    {
        public tbl_process_tmpl_group()
        {
            this.tbl_process_tmpl_section = new HashSet<tbl_process_tmpl_section>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Name { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<short> ParentGroup { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public int tbl_Process_TemplateID { get; set; }
    
        public virtual ICollection<tbl_process_tmpl_section> tbl_process_tmpl_section { get; set; }
    }
}

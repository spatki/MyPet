using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_tmpl_section: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public short SequenceNo { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public int tbl_Process_TemplateID { get; set; }
        public Nullable<int> tbl_Process_Tmpl_GroupID { get; set; }

        [ForeignKey("tbl_Process_TemplateID")]
        public virtual tbl_process_template tbl_process_template { get; set; }
        [ForeignKey("tbl_Process_Tmpl_GroupID")]
        public virtual tbl_process_tmpl_group tbl_process_tmpl_group { get; set; }
    }
}

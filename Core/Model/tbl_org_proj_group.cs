using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_group: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [Column(Order = 1)]
        public int ID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int tbl_Org_ProjectID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public Nullable<int> Parent_GroupID { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public virtual tbl_org_project tbl_org_project { get; set; }
    }
}

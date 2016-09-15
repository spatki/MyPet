using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_location: Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public string Name { get; set; }
        public byte SequenceNo { get; set; }
        public byte Type { get; set; }
        public Nullable<int> LevelMasterID { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; } 
    }
}

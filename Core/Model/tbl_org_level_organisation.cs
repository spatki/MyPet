using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_level_organisation : Entity
    {
        public int mstr_Org_LevelID { get; set; }
        public int mstr_Org_Level_MasterID { get; set; }
        public short Level { get; set; }
        public Nullable<int> mstr_Org_Level_ParentID { get; set; }
        public string Comments { get; set; }
        public string StructPath { get; set; }

        [ForeignKey("mstr_Org_LevelID")]
        public virtual mstr_org_level mstr_org_level { get; set; }
        [ForeignKey("mstr_Org_Level_MasterID")]
        public virtual mstr_org_level_master mstr_org_level_master { get; set; } 
    }
}

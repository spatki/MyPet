using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_org_level : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Org_LevelID { get; set; }
        public Nullable<int> tbl_Org_Level_MasterID { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("tbl_Org_LevelID")]
        public mstr_org_level mstr_org_level { get; set; }
        [ForeignKey("tbl_Org_Level_MasterID")]
        public mstr_org_level_master mstr_org_level_master { get; set; } 
    }
}

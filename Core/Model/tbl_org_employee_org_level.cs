using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_employee_org_level : Entity
    {
        public int tbl_Org_EmployeeID { get; set; }
        public int mstr_Org_LevelID { get; set; }
        public int mstr_Org_Level_MasterID { get; set; }

        [ForeignKey("tbl_Org_EmployeeID")]
        public virtual tbl_org_employee tbl_org_employee { get; set; }
        [ForeignKey("mstr_Org_LevelID")]
        public virtual mstr_org_level mstr_org_level { get; set; }
        [ForeignKey("mstr_Org_Level_MasterID")]
        public virtual mstr_org_level_master mstr_org_level_master { get; set; }
    }
}

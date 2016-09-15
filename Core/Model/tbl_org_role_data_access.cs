using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_role_data_access : Entity
    {
        public int RoleID { get; set; }
        public int OrgLevelID { get; set; }
        public int OrgMasterID { get; set; }
    }
}

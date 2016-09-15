using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_proj_lessons: Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public bool PublishToOrg { get; set; }
        public short mstr_Process_LC_StatusID { get; set; }
    }
}

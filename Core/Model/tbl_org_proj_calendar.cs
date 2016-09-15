using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_proj_calendar : Entity
    {
        public int tbl_Org_CalendarID { get; set; }
        public int tbl_Org_ProjectID { get; set; }

        [ForeignKey("tbl_Org_CalendarID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_calendar tbl_org_calendar { get; set; }
    }
}

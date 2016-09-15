using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_resourceplan_human : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Org_RoleID { get; set; }
        public string Skills { get; set; }
        public string JobDescription { get; set; }
        public DateTime PlannedStart { get; set; }
        public DateTime PlannedEnd { get; set; }
        public int Count { get; set; }
        public Decimal AllocationPercent { get; set; }
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> tbl_Org_Proj_LocationID { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("tbl_Org_Proj_GroupID,tbl_Org_ProjectID")]
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        [ForeignKey("tbl_Org_RoleID")]
        public mstr_org_role mstr_org_role { get; set; }
        [ForeignKey("tbl_Org_Proj_LocationID")]
        public tbl_org_proj_location tbl_org_proj_location { get; set; }
        [ForeignKey("mstr_Process_LC_StatusID")]
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
    }
}

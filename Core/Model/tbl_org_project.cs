using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_project: Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int mstr_Org_Project_TypeID { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> LocationOrgLevelID { get; set; }
        public int orgClientID { get; set; }
        public Nullable<System.DateTime> InitiationDate { get; set; }
        public System.DateTime PlannedStart { get; set; }
        public Nullable<System.DateTime> ActualStart { get; set; }
        public Nullable<System.DateTime> PlannedEnd { get; set; }
        public Nullable<System.DateTime> ActualEnd { get; set; }
        public bool ContractSigned { get; set; }
        public string Comments { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> SponseredBy { get; set; }
        public Nullable<System.DateTime> ContractSignDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<int> Status_User { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("mstr_Org_Project_TypeID")]
        public virtual mstr_org_project_type mstr_org_project_type { get; set; }
        [ForeignKey("orgClientID")]
        public virtual mstr_org_client mstr_org_client { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual UserProfile UserProfile { get; set; }
        [ForeignKey("Status_User")]
        public virtual UserProfile UserProfile_StatusUser { get; set; }
        public virtual ICollection<tbl_org_proj_review_history> tbl_org_proj_review_history { get; set; }
        public virtual ICollection<tbl_org_project_documents> tbl_org_project_documents { get; set; }
        public virtual ICollection<tbl_org_proj_group> tbl_org_proj_group { get; set; }
        public virtual ICollection<tbl_org_proj_location> tbl_org_proj_location { get; set; }
        public virtual ICollection<tbl_org_proj_org_level> tbl_org_proj_org_level { get; set; }
    }
}

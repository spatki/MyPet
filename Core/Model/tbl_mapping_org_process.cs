using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_mapping_org_process: Entity
    {
        public int? mstr_Org_Project_TypeID { get; set; }
        public int? mstr_Org_Proj_PhaseID { get; set; }
        public int? mstr_Org_FunctionID { get; set; }
        public int? mstr_Org_Sub_FunctionID { get; set; }
        public int? tbl_Process_Rep_TaskID { get; set; }
        public int? tbl_Process_RepositoryID { get; set; }
        public int? tbl_Org_Level_OrganisationID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [ForeignKey("mstr_Org_Project_TypeID")]
        public mstr_org_project_type mstr_org_project_type { get; set; }
        [ForeignKey("mstr_Org_Proj_PhaseID")]
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        [ForeignKey("mstr_Org_FunctionID")]
        public mstr_org_function mstr_org_function { get; set; }
        [ForeignKey("mstr_Org_Sub_FunctionID")]
        public mstr_org_sub_function mstr_org_sub_function { get; set; }
        [ForeignKey("tbl_Process_RepositoryID")]
        public tbl_process_repository tbl_process_repository { get; set; }
        [ForeignKey("tbl_Process_Rep_TaskID")]
        public tbl_process_rep_task tbl_process_rep_task { get; set; }
        [ForeignKey("tbl_Org_Level_OrganisationID")]
        public tbl_org_level_organisation tbl_org_level_organisation { get; set; }
    }
}

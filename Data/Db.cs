using System.Data.Entity;
using ProcessAccelerator.Core.Model;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;

namespace ProcessAccelerator.Data
{
    public class Db : DbContext
    {
        public Db() : base ("name=DefaultConnection")
        {
            Database.SetInitializer<Db>(null);
        }
        
        public DbSet<mstr_process_level> mstr_process_level { get; set; }
        public DbSet<mstr_process_role> mstr_process_role { get; set; }
        public DbSet<tbl_mapping_level> tbl_mapping_level { get; set; }
        public DbSet<mstr_process_level_master> mstr_process_level_master { get; set; }
        public DbSet<tbl_process_repository> tbl_process_repository { get; set; }
        public DbSet<tbl_process_role_level_access> tbl_process_role_level_access { get; set; }
        public DbSet<mstr_org_role> mstr_org_role { get; set; }
        public DbSet<tbl_process_structure> tbl_process_structure { get; set; }
        public DbSet<tbl_process_procedure> tbl_process_procedure { get; set; }
        public DbSet<tbl_process_proc_section> tbl_process_proc_section { get; set; }
        public DbSet<tbl_process_proc_revision> tbl_process_proc_revision { get; set; }
        public DbSet<tbl_process_proc_group> tbl_process_proc_group { get; set; }
        public DbSet<tbl_process_template> tbl_process_template { get; set; }
        public DbSet<tbl_process_tmpl_section> tbl_process_tmpl_section { get; set; }
        public DbSet<tbl_process_tmpl_revision> tbl_process_tmpl_revision { get; set; }
        public DbSet<tbl_process_rep_template> tbl_process_rep_template { get; set; }
        public DbSet<tbl_process_checklist> tbl_process_checklist { get; set; }
        public DbSet<tbl_process_chklst_group> tbl_process_chklst_group { get; set; }
        public DbSet<tbl_process_chklst_item> tbl_process_chklst_item { get; set; }
        public DbSet<tbl_process_chklst_revision> tbl_process_chklst_revision { get; set; }
        public DbSet<tbl_process_rep_chklst> tbl_process_rep_chklst { get; set; }
        public DbSet<vw_procedure> vw_procedure { get; set;}
        public DbSet<mstr_process_lc_status> mstr_process_lc_status { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
        public DbSet<tbl_process_general_task> tbl_process_general_task { get; set; }
        public DbSet<tbl_docmgr_document> tbl_docmgr_document { get; set; }
        public DbSet<tbl_docmgr_version> tbl_docmgr_version { get; set; }
        public DbSet<tbl_process_document> tbl_process_document { get; set; }
        public DbSet<tbl_process_rep_task> tbl_process_rep_task { get; set; }
        public DbSet<mstr_org_project_type> mstr_org_project_type { get; set; }
        public DbSet<mstr_org_proj_phase> mstr_org_proj_phase { get; set; }
        public DbSet<mstr_org_function> mstr_org_function { get; set; }
        public DbSet<mstr_org_sub_function> mstr_org_sub_function { get; set; }
        public DbSet<tbl_org_level_organisation> tbl_org_level_organisation { get; set; }
        public DbSet<mstr_org_phase_in_proj> mstr_org_phase_in_proj { get; set; }
        public DbSet<mstr_org_sub_in_function> mstr_org_sub_in_function { get; set; }
        public DbSet<tbl_mapping_org_process> tbl_mapping_org_process { get; set; }
        public DbSet<tbl_process_rep_task_ref_docs> tbl_process_rep_task_ref_docs { get; set; }
        public DbSet<tbl_mapping_role> tbl_mapping_role { get; set; }
        public DbSet<vw_role_mapping> vw_role_mapping { get; set; }
        public DbSet<mstr_client> mstr_client { get; set; }
        public DbSet<mstr_process_type> mstr_process_type { get; set; }
        public DbSet<mstr_access_menu> mstr_access_menu { get; set; }
        public DbSet<tbl_system_role_menu_access> tbl_system_role_menu_access { get; set; }
        public DbSet<vw_org_role_access> vw_org_role_access { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<tbl_org_emp_role> tbl_org_emp_role { get; set; }
        public DbSet<tbl_org_employee> tbl_org_employee { get; set; }
        public DbSet<mstr_org_level> mstr_org_level { get; set; }
        public DbSet<tbl_org_employee_org_level> tbl_org_employee_org_level { get; set; }
        public DbSet<tbl_org_role_menu_access> tbl_org_role_menu_access { get; set; }
        public DbSet<tbl_org_role_data_access> tbl_org_role_data_access { get; set; }
        public DbSet<tbl_org_project> tbl_org_project { get; set; }
        public DbSet<mstr_org_client> mstr_org_client { get; set; }
        public DbSet<tbl_workflow> tbl_workflow { get; set; }
        public DbSet<tbl_workflow_state_history> tbl_workflow_state_history { get; set; }
        public DbSet<tbl_workflow_state> tbl_workflow_state { get; set; }
        public DbSet<tbl_org_proj_review_history> tbl_org_proj_review_history { get; set; }
        public DbSet<tbl_org_project_documents> tbl_org_project_documents { get; set; }
        public DbSet<mstr_process_doc_type> mstr_process_doc_type { get; set; }
        public DbSet<tbl_org_proj_group> tbl_org_proj_group { get; set; }
        public DbSet<tbl_org_proj_plan> tbl_org_proj_plan { get; set; }
        public DbSet<tbl_org_plan_resource> tbl_org_plan_resource { get; set; }
        public DbSet<tbl_org_proj_planname> tbl_org_proj_planname { get; set; }
        public DbSet<tbl_org_project_process_mapping> tbl_org_project_process_mapping { get; set; }
        public DbSet<vw_prj_process_mapping> vw_prj_process_mapping { get; set; }
        public DbSet<tbl_tailored_general_task> tbl_tailored_general_task { get; set; }
        public DbSet<tbl_tailored_rep_chklst> tbl_tailored_rep_chklst { get; set; }
        public DbSet<tbl_tailored_rep_document> tbl_tailored_rep_document { get; set; }
        public DbSet<tbl_tailored_rep_procedure> tbl_tailored_rep_procedure { get; set; }
        public DbSet<tbl_tailored_rep_template> tbl_tailored_rep_template { get; set; }
        public DbSet<tbl_tailored_rep_task> tbl_tailored_rep_task { get; set; }
        public DbSet<tbl_org_proj_location> tbl_org_proj_location { get; set; }
        public DbSet<tbl_org_proj_allocation> tbl_org_proj_allocation { get; set; }
        public DbSet<vw_project_allocations> vw_project_allocations { get; set; }
        public DbSet<tbl_org_proj_calendar> tbl_org_proj_calendar { get; set; }
        public DbSet<tbl_org_calendar> tbl_org_calendar { get; set; }
        public DbSet<tbl_org_plan_document> tbl_org_plan_document { get; set; }
        public DbSet<tbl_workflow_fixed_actions> tbl_workflow_fixed_actions { get; set; }
        public DbSet<vw_resourcewise_TaskStatus> vw_resourcewise_TaskStatus { get; set; }
        public DbSet<vw_assigned_tasks> vw_assigned_tasks { get; set; }
        public DbSet<tbl_org_timesheet> tbl_org_timesheet { get; set; }
        public DbSet<vw_timesheetEntry> vw_timesheetEntry { get; set; }
        public DbSet<vw_task_recording> vw_task_recording { get; set; }
        public DbSet<vw_audit_pci_list> vw_audit_pci_list { get; set; }
        public DbSet<tbl_org_plan_filled_document> tbl_org_plan_filled_document { get; set; }
        public DbSet<vw_process_documents> vw_process_documents { get; set; }
        public DbSet<vw_process_document_master> vw_process_document_master { get; set; }
        public DbSet<tbl_process_rep_document> tbl_process_rep_document { get; set; }
        public DbSet<tbl_process_rep_procedure> tbl_process_rep_procedure { get; set; }
        public DbSet<tbl_org_proj_org_level> tbl_org_proj_org_level { get; set; }
        public DbSet<tbl_org_estm_parameters> tbl_org_estm_parameters { get; set; }
        public DbSet<tbl_org_proj_estimation> tbl_org_proj_estimation { get; set; }
        public DbSet<tbl_org_estm_roles> tbl_org_estm_roles { get; set; }
        public DbSet<tbl_org_proj_estm_effort_schedule> tbl_org_proj_estm_effort_schedule { get; set; }
        public DbSet<tbl_org_proj_estm_modules> tbl_org_proj_estm_modules { get; set; }
        public DbSet<tbl_org_proj_estm_size> tbl_org_proj_estm_size { get; set; }
        public DbSet<tbl_org_proj_estm_gsc> tbl_org_proj_estm_gsc { get; set; }
        public DbSet<tbl_org_estm_gsc_master> tbl_org_estm_gsc_master { get; set; }
        public DbSet<tbl_org_proj_estm_cost> tbl_org_proj_estm_cost { get; set; }
        public DbSet<tbl_org_proj_estm_productivity> tbl_org_proj_estm_productivity { get; set; }
        public DbSet<tbl_audit_participant> tbl_audit_participant { get; set; }
        public DbSet<tbl_audit_role> tbl_audit_role { get; set; }
        public DbSet<tbl_audit_plan> tbl_audit_plan { get; set; }
        public DbSet<tbl_audit_schedule> tbl_audit_schedule { get; set; }
        public DbSet<tbl_audit_observation> tbl_audit_observation { get; set; }
        public DbSet<tbl_org_audit_observation> tbl_org_audit_observation { get; set; }
        public DbSet<tbl_org_audit_plan> tbl_org_audit_plan { get; set; }
        public DbSet<tbl_org_audit_schedule> tbl_org_audit_schedule { get; set; }
        public DbSet<tbl_org_audit_role> tbl_org_audit_role { get; set; }
        public DbSet<tbl_org_audit_participant> tbl_org_audit_participant { get; set; }
        public DbSet<tbl_org_audit_addln_obs> tbl_org_audit_addln_obs { get; set; }
        public DbSet<mstr_org_general_tasks> mstr_org_general_tasks { get; set; }
        public DbSet<tbl_org_general_task_roles> tbl_org_general_task_roles { get; set; }
        public DbSet<tbl_proj_general_tasks> tbl_proj_general_tasks { get; set; }
        public DbSet<tbl_proj_general_task_roles> tbl_proj_general_task_roles { get; set; }
        public DbSet<tbl_tailored_rep_task_ref_docs> tbl_tailored_rep_task_ref_docs { get; set; }
        public DbSet<tbl_org_resourceplan_human> tbl_org_resourceplan_human { get; set; }
        public DbSet<tbl_org_resourceplan_hardware> tbl_org_resourceplan_hardware { get; set; }
        public DbSet<mstr_org_defect_type> mstr_org_defect_type { get; set; }
        public DbSet<mstr_org_defect_severity> mstr_org_defect_severity { get; set; }
        public DbSet<tbl_org_defect_document> tbl_org_defect_document { get; set; }
        public DbSet<tbl_org_defect> tbl_org_defect { get; set; }
        public DbSet<vw_project_variance> vw_project_variance { get; set; }
        public DbSet<tbl_workflow_functions> tbl_workflow_functions { get; set; }
        public DbSet<tbl_workflow_implementation> tbl_workflow_implementation { get; set; }
        public DbSet<vw_workflow> vw_workflow { get; set; }
        public DbSet<tbl_audit_checklist> tbl_audit_checklist { get; set; }
   
        public IEnumerable<prc_emp_by_location> EmployeesByLocation(int projectID, int? locationID)
        {
            IEnumerable<prc_emp_by_location> result = this.Database.SqlQuery<prc_emp_by_location>("Exec EmployeesByProjectLocation @projectID = {0}, @locationID = {1}",projectID,locationID);
            return result;
        }

        public IEnumerable<reportingEmp> ReportingManagers(int projectID, int employeeID, int roleID)
        {
            IEnumerable<reportingEmp> result = this.Database.SqlQuery<reportingEmp>("Exec sp_getReportingManagers @projectID = {0}, @employeeID = {1}, @roleID = {2}", projectID, employeeID, roleID);
            return result;
        }

        public int CreateEstimationCycle(int estmID, int currentVersion)
        {
            List<int> result = this.Database.SqlQuery<int> ("Exec sp_createEstimationCycle @estmID = {0}, @currentVersion = {1}", estmID, currentVersion).ToList<int>();
            return result.First();
        }

        public status TS_Status(int employeeID, System.DateTime startDate, System.DateTime endDate, int monthNumber)
        {
            IEnumerable<status> result = this.Database.SqlQuery<status>("Exec sp_TS_Status @employeeID = {0}, @TSstart = {1}, @TSEnd = {2}, @monthNumber = {3}", employeeID, startDate, endDate, monthNumber);
            if (result == null || !result.Any()) { return null; }
            else { return result.FirstOrDefault(); }
        }

        public IEnumerable<int> AccessibleProjects(int employeeID, int roleID)
        {
            IEnumerable<int> result = this.Database.SqlQuery<int>("Exec sp_getAccessibleProjects @employeeID = {0}, @roleID = {1}", employeeID, roleID);
            return result;
        }

        public IEnumerable<employee> UnAllocatedEmployees (int employeeID, int roleID)
        {
            IEnumerable<employee> result = this.Database.SqlQuery<employee>("Exec sp_unallocatedEmployees @employeeID = {0}, @roleID = {1}", employeeID, roleID);
            return result;
        }

        public IEnumerable<tailored_general_tasks> TS_GeneralTasks(int employeeID, System.DateTime startDate, System.DateTime endDate)
        {
            IEnumerable<tailored_general_tasks> result = this.Database.SqlQuery<tailored_general_tasks>("Exec sp_GeneralTasks @employeeID = {0}, @TSstart = {1}, @TSEnd = {2}", employeeID, startDate, endDate);
            return result;
        }

        public IEnumerable<review_timesheets> TS_Review (int approverID, int typeID)
        {
            IEnumerable<review_timesheets> result = this.Database.SqlQuery<review_timesheets>("Exec sp_getTSForApprover @approverID = {0}, @typeID = {1}",approverID,typeID);
            return result;
        }

        public IEnumerable<vw_timesheetEntry> GetApproverTSDetails (int approverID, int typeID, int empID, System.DateTime startDate)
        {
            IEnumerable<vw_timesheetEntry> result = this.Database.SqlQuery<vw_timesheetEntry>("Exec sp_getTSForApproverDetails @approverID = {0}, @typeID = {1}, @empID = {2}, @startDate = {3}", approverID, typeID, empID, startDate);
            return result;
        }

        public int reviewEmployeeTS(int approverID, int typeID, int empID, System.DateTime @startDate, System.DateTime endDate, int statusID)
        {
            IEnumerable<int> result = this.Database.SqlQuery<int>("Exec sp_ReviewTSForEmployee @approverID = {0}, @typeID = {1}, @empID = {2}, @startDate = {3}, @endDate = {4}, @statusID = {5} ", approverID, typeID, empID, startDate, endDate, statusID);
            return result.FirstOrDefault();
        }

        public int InitializeDataForNewClient(int clientID)
        {
            int result = this.Database.ExecuteSqlCommand("Exec sp_InitializeData_NewClient @clientID = {0}", clientID);
            return result;
        }
    }
}

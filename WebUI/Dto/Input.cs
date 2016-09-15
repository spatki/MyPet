using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;

namespace ProcessAccelerator.WebUI.Dto
{
    public class Input
    {
        public int ID { get; set; }
        [Display(Name="Client")]
        public Nullable<int> ClientID { get; set; }
        public virtual Nullable<bool> reSequence { get; set; }
        public virtual Nullable<bool> followWF { get; set; }
        public virtual Nullable<int> statusWF { get; set; }
        public int StatusType { get; set; }
        public int workflowUser { get; set; }
        public Nullable<bool> workflow { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        [StrLen(100)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel: Input
    {
        [Required]
        [StrLen(100)]
        [Display(Name="Name")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name="User Type")]
        public short UserType { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email Id (Used as login user name)")]
        public string UserName { get; set; }

        public Nullable<int> EmployeeID { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
   
    public class mstr_process_levelInput : Input
    {
        [Required(ErrorMessage = "Enter the Sequence Number")]
        [Range(1,99,ErrorMessage="Enter a value between 1 to 99")]
        [Display(Name = "Sequence")]
        public short LevelSequence { get; set; }

        [Required(ErrorMessage = "Short Name Required")]
        [StrLen(20)]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [StrLen(50)]
        [Display(Name = "Long Name")]
        public string LongName { get; set; }

        [DataType(DataType.MultilineText)]
        [StrLen(100)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class mstr_process_roleInput : Input
    {
        [Required(ErrorMessage = "Enter the Short Name")]
        [StrLen(20)]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Enter the Long Name")]
        [StrLen(50)]
        public string LongName { get; set; }

        [StrLen(100)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Select the Role Type")]
        [Range(1, 2, ErrorMessage = "Select the Role Type")]
        [Display(Name = "Role Type")]
        public short Type { get; set; }
    }

    public class tbl_process_structureInput : Input
    {
        [Required(ErrorMessage = "Select a role")]
        [Display(Name="Role")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a role")]
        public int mstr_Process_RoleID { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public Nullable<int> ParentRoleID { get; set; }
        public short Sequence { get; set; }
        public string StructPath { get; set; }
        [StringLength(200)]
        public string Comments { get; set; }
    }

    public class mstr_org_levelInput : Input
    {

        [Required(ErrorMessage = "Enter the Sequence Number")]
        [Range(1, 99, ErrorMessage = "Enter a value between 1 to 99")]
        [Display(Name = "Sequence")]
        public short LevelSequence { get; set; }

        [Required(ErrorMessage = "Short Name Required")]
        [StrLen(20)]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
        [StrLen(50)]
        [Display(Name = "Long Name")]
        public string LongName { get; set; }

        [DataType(DataType.MultilineText)]
        [StrLen(100)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class mstr_org_roleInput : Input
    {
        [Required(ErrorMessage = "Enter the Short Name")]
        [StrLen(20)]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Enter the Long Name")]
        [StrLen(50)]
        public string LongName { get; set; }

        [StrLen(100)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Select the Role Type")]
        [Range(1, 2, ErrorMessage = "Select the Role Type")]
        [Display(Name = "Role Type")]        
        public short Type { get; set; }

        [Display(Name = "Primary Role Mapping")]
        public Nullable<int> mstr_Primary_Process_RoleID { get; set; }

        [Display(Name = "Project Approver")]
        public Nullable<bool> Project_Approver { get; set; }

        [Display(Name = "Department Manager")]
        public Nullable<bool> Dept_Reporting { get; set; }

        [Display(Name = "HR Manager")]
        public Nullable<bool> HR_Reporting { get; set; }

        [Display(Name = "Project Reviewer")]
        public Nullable<bool> Project_Reviewer { get; set; }
    }

    public class mstr_org_level_masterInput : Input
    {
        [Required(ErrorMessage = "Enter the Short Name")]
        [StrLen(50)]
        public string ShortName { get; set; }
        [Required(ErrorMessage = "Enter the Long Name")]
        [StrLen(100)]
        public string LongName { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        public Nullable<bool> DefaultAccessGrant { get; set; }
        [Required(ErrorMessage = "Choose the Org Level")]
        public int mstr_Org_LevelID { get; set; }
    }
    
    public partial class mstr_org_designationInput : Input
    {
        [Required(ErrorMessage = "Enter the Short Name")]
        [StrLen(50)]
        public string ShortName { get; set; }
        [Required(ErrorMessage = "Enter the Long Name")]
        [StrLen(100)]
        public string LongName { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }
    }

    public partial class mstr_org_structureInput : Input
    {
        [Required(ErrorMessage = "Select the designation")]
        [Range(1, 99, ErrorMessage = "Designation not within range")]
        [Display(Name = "Designation")]
        public int mstr_Org_DesignationID { get; set; }
        public short Level { get; set; }
        public Nullable<int> mstr_Org_DesignationParentID { get; set; }
        public bool IsRoleSpecific { get; set; }
        public Nullable<int> mstr_Org_RoleID { get; set; }
        public Nullable<bool> IsEmployeeSpecific { get; set; }
        public Nullable<int> tbl_Org_EmployeeID { get; set; }
        public string StructPath { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
    }

    public partial class tbl_process_procedureInput : Input
    {
        public tbl_process_procedureInput()
        {
            this.tbl_process_proc_revision = new List<tbl_process_proc_revisionInput>();
            this.tbl_process_proc_section = new List<tbl_process_proc_sectionInput>();
            this.tbl_process_rep_procedure = new List<tbl_process_rep_procedureInput>();
            this.tbl_process_proc_group = new List<tbl_process_proc_groupInput>();
        }

        [Required(ErrorMessage = "Enter Procedure Name")]
        [StrLen(50)]
        public string Name { get; set; }
        [Required]
        [Display(Name="Process Type")]
        public int mstr_Process_TypeID { get; set; }
        [AllowHtml]
        public string Mission { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public string ReviewedByName { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public string TailorComments { get; set; }
        public Nullable<bool> AvailableToAll { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string companyName { get; set; }
        public string companyShortName { get; set; }
        public string companyLogo { get; set; }


        public virtual ICollection<tbl_process_proc_revisionInput> tbl_process_proc_revision { get; set; }
        public virtual ICollection<tbl_process_proc_sectionInput> tbl_process_proc_section { get; set; }
        public virtual ICollection<tbl_process_rep_procedureInput> tbl_process_rep_procedure { get; set; }
        public virtual ICollection<tbl_process_proc_groupInput> tbl_process_proc_group { get; set; }
    }

    public partial class tbl_process_proc_revisionInput : Input
    {
        public int tbl_Process_ProcedureID { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        public int RevisionUser { get; set; }
        public System.DateTime RevisionDate { get; set; }
        public Nullable<int> ReviewUser { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string version { get; set; }
    }

    public partial class mstr_process_lc_statusInput : Input
    {
        [Required(ErrorMessage = "Pl. enter the status name")]
        [StrLen(20)]
        public string Status { get; set; }
        [StrLen(50)]
        public string Description { get; set; }
        [Required]
        [Display(Name="Status Type")]
        public short Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsComplete { get; set; }
        public bool IsPublish { get; set; }
        public Nullable<bool> IsInactive { get; set; }
        public Nullable<bool> IsReview { get; set; }
        [Required]
        [Display(Name="Sequence")]
        [Range(1,99)]
        public byte SequenceNo { get; set; }
        public Nullable<int> Prev_Status { get; set; }
        public Nullable<int> Next_Status { get; set; }
        public Nullable<int> DefaultReviewRoleID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

    public partial class tbl_process_proc_sectionInput : Input
    {
        public short SequenceNo { get; set; }
        [Required(ErrorMessage = "Section Title is mandatory")]
        [StrLen(50)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Section Details is mandatory")]
        [AllowHtml]
        public string Detail { get; set; }
        public int tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_Proc_GroupID { get; set; }
    }

    public partial class seededSections
    {
        public Nullable<int> seed { get; set; }
        public bool isParent { get; set; }
        public string sequencePrefix { get; set; }
        public int sequence { get; set; }
        public string companyName { get; set; }
        public string companyShortName { get; set; }
        public string companyLogo { get; set; }
        public IEnumerable<tbl_process_proc_sectionInput> tbl_process_proc_section { get; set; }
        public IEnumerable<tbl_process_proc_groupInput> tbl_process_proc_group { get; set; }
    }

    public partial class tbl_process_proc_groupInput : Input
    {
        public tbl_process_proc_groupInput()
        {
            this.tbl_process_proc_section = new HashSet<tbl_process_proc_sectionInput>();
        }
        [StrLen(50)]
        public string Name { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<short> ParentGroup { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public int tbl_Process_ProcedureID { get; set; }

        public virtual ICollection<tbl_process_proc_sectionInput> tbl_process_proc_section { get; set; }
    }

    public partial class tbl_process_rep_procedureInput : Input
    {
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_Rep_ProcessID { get; set; }

        public virtual tbl_process_procedureInput tbl_process_procedure { get; set; }
        public virtual tbl_process_rep_processInput tbl_process_rep_process { get; set; }
        public virtual tbl_process_repositoryInput tbl_process_repository { get; set; }
    }

    public partial class tbl_process_rep_processInput : Input
    {
/*        public tbl_process_rep_processInput()
        {
            this.tbl_process_rep_taskInput = new HashSet<tbl_process_rep_taskInput>();
            this.tbl_process_rep_procedureInput = new HashSet<tbl_process_rep_procedureInput>();
            this.tbl_process_rep_templateInput = new HashSet<tbl_process_rep_templateInput>();
        } */

        public int tbl_Process_RepositoryID { get; set; }
        [Required]
        [StrLen(50)]
        public string Name { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        public short SequenceNo { get; set; }
        public string ShortCode { get; set; }
        public short mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
/*
        public virtual mstr_process_lc_statusInput mstr_process_lc_statusInput { get; set; }
        public virtual tbl_process_repositoryInput tbl_process_repositoryInput { get; set; }
        public virtual ICollection<tbl_process_rep_taskInput> tbl_process_rep_taskInput { get; set; }
        public virtual ICollection<tbl_process_rep_procedureInput> tbl_process_rep_procedureInput { get; set; }
        public virtual ICollection<tbl_process_rep_templateInput> tbl_process_rep_templateInput { get; set; }
 */
    }

    public partial class UserProfileInput : Input
    {
        public UserProfileInput()
        {
            this.webpages_UsersInRolesInput = new HashSet<webpages_UsersInRoles>();
            this.Roles = new List<int>();
        }
        [Required]
        [StrLen(200)]
        [Display(Name="Email (User ID)")]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        [Display(Name="User Type")]
        public Nullable<short> UserType { get; set; }
        public Nullable<short> AccessType { get; set; }
        public string RefCode { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> ClientID { get; set; }
        [Required]
        [Display(Name="Name")]
        public string DisplayName { get; set; }
        [Display(Name="Email ID")]
        [EmailAddress]
        public string EMailID { get; set; }
        public string MobileContact { get; set; }
        public Nullable<bool> IsAdministrator { get; set; }
        public Nullable<bool> IsGuest { get; set; }
        public Nullable<int> ProfilePicture { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public List<int> Roles { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }

        public virtual ICollection<webpages_UsersInRoles> webpages_UsersInRolesInput { get; set; }
        public virtual tbl_org_employeeInput tbl_org_employeeInput { get; set; }
        public virtual mstr_client mstr_client { get; set; }
    }

    public class EditUserDetailsInput : Input
    {
        [Required]
        [StrLen(200)]
        [Display(Name="User ID")]
        public string UserName { get; set; }
        [Required]
        [Display(Name="User Type")]
        public Nullable<short> UserType { get; set; }
        public Nullable<short> AccessType { get; set; }
        public string RefCode { get; set; }
        public Nullable<int> ClientID { get; set; }
        [Required]
        [Display(Name="Name")]
        [StrLen(100)]
        public string DisplayName { get; set; }
        [Required]
        [Display(Name = "Email ID")]
        [EmailAddress]
        [StrLen(100)]
        public string EMailID { get; set; }
        [StrLen(20)]
        public string MobileContact { get; set; }
        public Nullable<bool> IsAdministrator { get; set; }
        public Nullable<bool> IsGuest { get; set; }
    }

    public partial class webpages_UsersInRolesInput
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual UserProfileInput UserProfileInput { get; set; }
        public virtual webpages_RolesInput webpages_RolesInput { get; set; }
    }

    public class PasswordResetInput
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public partial class tbl_org_employeeInput : Input
    {

        public tbl_org_employeeInput()
        {
            this.employee_org_level = new HashSet<employee_org_level>();
        }
        [Required]
        [StrLen(50)]
        public string EmpCode { get; set; }
        [Required]
        [StrLen(50)]
        public string GivenName { get; set; }
        [StrLen(50)]
        public string FamilyName { get; set; }
        [Required]
        public short Gender { get; set; }
        [Display(Name="Date of Birth")]
        [DataType(DataType.DateTime,ErrorMessage="Provide valid date input")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        [Required]
        [Display(Name = "Date of Joining")]
        [DataType(DataType.DateTime, ErrorMessage = "Provide valid date input")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime DateOfJoining { get; set; }
        [Required]
        [Display(Name = "Electronic Mail Address")]
        [StrLen(100)]
        [EmailAddress]
        public string EmailID { get; set; }
        [DataType(DataType.DateTime, ErrorMessage = "Provide valid date input")]
        public Nullable<System.DateTime> ConfirmationDate { get; set; }
        [DataType(DataType.DateTime, ErrorMessage = "Provide valid date input")]
        public Nullable<System.DateTime> DateOfResignation { get; set; }
        [DataType(DataType.DateTime, ErrorMessage = "Provide valid date input")]
        public Nullable<System.DateTime> DateRelieved { get; set; }
        [Display(Name="Employment Status")]
        public int mstr_Process_LC_StatusID { get; set; }
        [Required]
        public DateTime StatusDate { get; set; }
        [Required]
        [Display(Name="Designation")]
        public int CurrentDesignation { get; set; }
        [Required]
        [Display(Name="Designation effective from")]
        public System.DateTime DesignationEffectiveFrom { get; set; }
        public Nullable<int> HR_Reporting { get; set; }
        public Nullable<int> Dept_Reporting { get; set; }
        public Nullable<int> UserID { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        [Required]
        [Display(Name="Role")]
        public string[] Roles { get; set; }
        [Required]
        [StrLen(200)]
        public string UserName { get; set; }
        [Required]
        [StrLen(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string projectLocation { get; set; }

        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public virtual ICollection<employee_org_level> employee_org_level { get; set; }
    }

    public partial class UserRoles
    {
        public int userid { get; set; }
        public string selectRoles { get; set; }
        public string type { get; set; }
        [Required]
        [Display(Name="Role")]
        public int Role { get; set; }
    }

    public partial class webpages_RolesInput : Input
    {
        public webpages_RolesInput()
        {
            this.webpages_UsersInRoles = new HashSet<webpages_UsersInRoles>();
        }

        public string RoleName { get; set; }

        public virtual ICollection<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
    }

    public class employee_org_level
    {
        public int ID { get; set; }
        public int org_levelID { get; set; }
        public string org_level_name { get; set; }
        public Nullable<int> org_level_masterID { get; set; }
    }

    public partial class tbl_process_templateInput : Input
    {
        public tbl_process_templateInput()
        {
            this.tbl_process_rep_template = new HashSet<tbl_process_rep_templateInput>();
            this.tbl_process_tmpl_group = new HashSet<tbl_process_tmpl_groupInput>();
            this.tbl_process_tmpl_section = new HashSet<tbl_process_tmpl_sectionInput>();
            this.tbl_process_tmpl_revision = new HashSet<tbl_process_tmpl_revisionInput>();
        }
        [Required(ErrorMessage = "Template name is mandatory")]
        [StrLen(50)]
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public string ReviewedByName { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public string TailorComments { get; set; }
        public Nullable<bool> AvailableToAll { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        public virtual mstr_process_lc_statusInput mstr_process_lc_status { get; set; }
        public virtual ICollection<tbl_process_rep_templateInput> tbl_process_rep_template { get; set; }
        public virtual ICollection<tbl_process_tmpl_revisionInput> tbl_process_tmpl_revision { get; set; }
        public virtual ICollection<tbl_process_tmpl_sectionInput> tbl_process_tmpl_section { get; set; }
        public virtual ICollection<tbl_process_tmpl_groupInput> tbl_process_tmpl_group { get; set; }
    }

    public partial class tbl_process_tmpl_groupInput : Input
    {
        public tbl_process_tmpl_groupInput()
        {
            this.tbl_process_tmpl_section = new HashSet<tbl_process_tmpl_sectionInput>();
        }
        [Required]
        [StrLen(50)]
        public string Name { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<short> ParentGroup { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public int tbl_Process_TemplateID { get; set; }


        public virtual ICollection<tbl_process_tmpl_sectionInput> tbl_process_tmpl_section { get; set; }
    }

    public partial class tbl_process_tmpl_sectionInput : Input
    {
        public short SequenceNo { get; set; }
        [Required(ErrorMessage = "Section Title is mandatory")]
        [StrLen(50)]
        public string Title { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Section Details is mandatory")]
        public string Detail { get; set; }
        public int tbl_Process_TemplateID { get; set; }
        public Nullable<int> tbl_Process_Tmpl_GroupID { get; set; }

        public virtual tbl_process_templateInput tbl_process_template { get; set; }
        public virtual tbl_process_tmpl_groupInput tbl_process_tmpl_group { get; set; }
    }

    public partial class tbl_process_tmpl_revisionInput : Input
    {
        public int tbl_Process_TemplateID { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        public int RevisionUser { get; set; }
        public System.DateTime RevisionDate { get; set; }
        public Nullable<int> ReviewUser { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string version { get; set; }

        public virtual mstr_process_lc_statusInput mstr_process_lc_status { get; set; }
        public virtual tbl_process_templateInput tbl_process_template { get; set; }
    }

    public partial class tbl_process_rep_templateInput : Input
    {
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_TemplateID { get; set; }
        public Nullable<int> tbl_Process_Rep_ProcessID { get; set; }

        public virtual tbl_process_rep_processInput tbl_process_rep_process { get; set; }
        public virtual tbl_process_repositoryInput tbl_process_repository { get; set; }
        public virtual tbl_process_templateInput tbl_process_template { get; set; }
    }

    public partial class tbl_tailored_rep_templateInput : Input
    {
        public int tbl_Mapping_Org_ProcessID { get; set; }
        public short TailorType { get; set; }
        public int tbl_Process_TemplateID { get; set; }
    }

    public partial class seededTemplateSections
    {
        public Nullable<int> seed { get; set; }
        public bool isParent { get; set; }
        public string sequencePrefix { get; set; }
        public int sequence { get; set; }
        public IEnumerable<tbl_process_tmpl_sectionInput> tbl_process_tmpl_section { get; set; }
        public IEnumerable<tbl_process_tmpl_groupInput> tbl_process_tmpl_group { get; set; }
    }

    public partial class tbl_process_checklistInput : Input
    {
        public tbl_process_checklistInput()
        {
            this.tbl_process_chklst_revision = new List<tbl_process_chklst_revisionInput>();
            this.tbl_process_chklst_item = new List<tbl_process_chklst_itemInput>();
            this.tbl_process_rep_chklst = new List<tbl_process_rep_chklstInput>();
            this.tbl_process_chklst_group = new List<tbl_process_chklst_groupInput>();
        }

        [Required(ErrorMessage="Enter Checklist Name")]
        [StrLen(50)]
        public string Name { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        [Required]
        [StrLen(20)]
        [Display(Name = "SNO Caption/Title")]
        public string C_SNO { get; set; }
        [Required]
        [Display(Name = "SNO Column Width")]
        [Range(1, 100)]
        public Nullable<short> C_SNO_Len { get; set; }
        [Required]
        [StrLen(20)]
        [Display(Name = "Checkpoint Caption/Title")]
        public string C_ChkPt { get; set; }
        [Required]
        [Display(Name = "Checkpoint Column Width")]
        [Range(1, 100)]
        public Nullable<short> C_ChkPt_Len { get; set; }
        [Required]
        [StrLen(20)]
        [Display(Name = "Result Caption/Title")]
        public string C_Result { get; set; }
        [Required]
        [Display(Name = "Result Column Width")]
        [Range(1, 100)]
        public Nullable<short> C_Result_Len { get; set; }
        [Required]
        [StrLen(20)]
        [Display(Name = "Remarks Caption/Title")]
        public string C_Remarks { get; set; }
        [Required]
        [Display(Name = "Remarks Column Width")]
        [Range(1, 100)]
        public Nullable<short> C_Remarks_Len { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public string ReviewedByName { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public string TailorComments { get; set; }
        public Nullable<bool> AvailableToAll { get; set; }

        public virtual ICollection<tbl_process_chklst_revisionInput> tbl_process_chklst_revision { get; set; }
        public virtual ICollection<tbl_process_chklst_itemInput> tbl_process_chklst_item { get; set; }
        public virtual ICollection<tbl_process_rep_chklstInput> tbl_process_rep_chklst { get; set; }
        public virtual ICollection<tbl_process_chklst_groupInput> tbl_process_chklst_group { get; set; }
    }

    public partial class tbl_process_chklst_itemInput : Input
    {
        public int tbl_Process_ChecklistID { get; set; }
        public Nullable<bool> IsGroup { get; set; }
        [Required(ErrorMessage = "Enter the checklist description")]
        [AllowHtml]
        public string ItemDescription { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<int> tbl_Process_Chklst_GroupID { get; set; }
        [Required(ErrorMessage = "Enter the options")]
        public List<string> ChklstOptionList { get; set; }
        [AllowHtml]
        public string Remarks { get; set; }
   }

    public partial class tbl_process_chklst_groupInput : Input
    {
        public int tbl_Process_ChecklistID { get; set; }
        [Required]
        [StrLen(50)]
        public string Name { get; set; }
        public short SequenceNo { get; set; }
        public Nullable<short> ParentGroup { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
    }

    public partial class tbl_process_chklst_revisionInput : Input
    {
        public int tbl_Process_ChecklistID { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        public int RevisionUser { get; set; }
        public System.DateTime RevisionDate { get; set; }
        public Nullable<int> ReviewUser { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<int> PublishDate { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string version { get; set; }
    }

    public partial class tbl_process_rep_chklstInput : Input
    {
        public int tbl_Process_RepositoryID { get; set; }
        public int tbl_Process_ChecklistID { get; set; }
        public Nullable<int> tbl_Process_Rep_ProcessID { get; set; }
    }

    public partial class seededChklstItems
    {
        public Nullable<int> seed { get; set; }
        public bool isParent { get; set; }
        public string sequencePrefix { get; set; }
        public int sequence { get; set; }
        public IEnumerable<tbl_process_chklst_itemInput> tbl_process_chklst_item { get; set; }
        public IEnumerable<tbl_process_chklst_groupInput> tbl_process_chklst_group { get; set; }
    }

    public partial class tbl_process_repositoryInput : Input
    {
        public tbl_process_repositoryInput()
        {
            this.tbl_process_general_task = new HashSet<tbl_process_general_taskInput>();
            this.tbl_process_rep_document = new HashSet<tbl_process_rep_documentInput>();
            this.tbl_process_rep_procedure = new HashSet<tbl_process_rep_procedureInput>();
            this.tbl_process_rep_revision = new HashSet<tbl_process_rep_revisionInput>();
            this.tbl_process_rep_role = new HashSet<tbl_process_rep_roleInput>();
            this.tbl_process_rep_template = new HashSet<tbl_process_rep_templateInput>();
            this.tbl_process_rep_chklst = new HashSet<tbl_process_rep_chklstInput>();
        }

        public Nullable<short> Sequence { get; set; }
        [Required(ErrorMessage = "Enter the Name")]
        [StrLen(50)]
        [Display(Name="Name")]
        public string Name { get; set; }
        public bool IsParent { get; set; }
        public int Level { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string StructPath { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public Nullable<byte> TailorReference { get; set; }
        public Nullable<int> TailorID { get; set; }
        public string TailorComments { get; set; }
        [Required]
        [Display(Name="Level")]
        public int mstr_Process_LevelID { get; set; }
        public Nullable<bool> TreatAsTask { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [ForeignKey("mstr_Process_LevelID")]
        public virtual mstr_process_levelInput mstr_process_level { get; set; }
        public virtual ICollection<tbl_process_general_taskInput> tbl_process_general_task { get; set; }
        public virtual ICollection<tbl_process_rep_documentInput> tbl_process_rep_document { get; set; }
        public virtual ICollection<tbl_process_rep_procedureInput> tbl_process_rep_procedure { get; set; }
        public virtual ICollection<tbl_process_rep_revisionInput> tbl_process_rep_revision { get; set; }
        public virtual ICollection<tbl_process_rep_roleInput> tbl_process_rep_role { get; set; }
        public virtual ICollection<tbl_process_rep_templateInput> tbl_process_rep_template { get; set; }
        public virtual ICollection<tbl_process_rep_chklstInput> tbl_process_rep_chklst { get; set; }
    }

    public partial class tbl_process_rep_generaltasks 
    {
        public tbl_process_rep_generaltasks()
        {
            this.tbl_process_general_task = new List<tbl_process_general_taskInput>();
        }

        public int ID { get; set; }
        public int repoID { get; set; }
        public Nullable<int> phaseID { get; set; }
        public string key { get; set; }

        public virtual ICollection<tbl_process_general_taskInput> tbl_process_general_task { get; set; }
    }

    public partial class tbl_process_rep_details
    {
        public tbl_process_rep_details()
        {
            this.selectedOptions = new List<int>();
        }

        public int ID { get; set; }
        public int repoID { get; set; }
        public Nullable<int> phaseID { get; set; }
        public string key { get; set; }

        public virtual ICollection<int> selectedOptions { get; set; }
    }

    public partial class tbl_process_rep_taskdetails
    {
        public tbl_process_rep_taskdetails()
        {
            this.selectedOptions = new List<tbl_process_rep_taskInput>();
            this.position = new List<string>();
        }

        public int ID { get; set; }
        public List<string> position { get; set; }

        public virtual ICollection<tbl_process_rep_taskInput> selectedOptions { get; set; }
    }

    public partial class tbl_org_config_details
    {
        public tbl_org_config_details()
        {
            this.selectedOptions = new List<int>();
        }

        public int ID { get; set; }
        public string Name {get; set;}

        public virtual ICollection<int> selectedOptions { get; set; }
    }

    public partial class tbl_process_general_taskInput : Input
    {

        [Required]
        [Display(Name="Task Name")]
        [StrLen(50)]
        public string Name { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        public byte SequenceNo { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public List<string> roleIDs { get; set; }

        public virtual tbl_process_repositoryInput tbl_process_repository { get; set; }
    }

    public partial class tbl_process_rep_documentInput : Input
    {
        public int tbl_Process_RepositoryID { get; set; }
        public int tbl_Process_DocumentID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public string TailorComments { get; set; }
        public Nullable<bool> AvailableToAll { get; set; }
        public Nullable<byte> TailorReference { get; set; }
        public Nullable<int> TailorID { get; set; }

        [ForeignKey("tbl_Process_RepositoryID")]
        public virtual tbl_process_repositoryInput tbl_process_repository { get; set; }
    }

    public partial class tbl_process_rep_revisionInput : Input
    {
        public string Comments { get; set; }
        public int Revision_User { get; set; }
        public System.DateTime Revision_Date { get; set; }
        public Nullable<int> Review_User { get; set; }
        public Nullable<System.DateTime> Review_Date { get; set; }
        public Nullable<System.DateTime> Publish_Date { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string version { get; set; }
        public string LC_Status { get; set; }
    }

    public partial class tbl_process_rep_roleInput : Input
    {
        public short mstr_Process_RoleID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string Access { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

    public class repoData
    {
        public int ID { get; set; }
        public bool isHeader { get; set; }
        public string repoName { get; set; }
        public int levelID { get; set; }
        public string levelName { get; set; }
        public int Level { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string itemName { get; set; }
        public string className { get; set; }
        public int itemID { get; set; }
        public string itemKey { get; set; }
    }

    public partial class tbl_docmgr_documentInput : Input
    {
        public tbl_docmgr_documentInput()
        {
            this.tbl_docmgr_version = new HashSet<tbl_docmgr_version>();
            this.tbl_process_document = new HashSet<tbl_process_document>();
        }

        [Display(Name="Document Name")]
        [Required]
        [StrLen(50)]
        public string Name { get; set; }
        public string FileLocation { get; set; }
        [Required]
        [Display(Name="File to be uploaded")]
        public HttpPostedFileBase fileName { get; set; }
        public Nullable<decimal> Version { get; set; }
        public Nullable<bool> ReadWriteMode { get; set; }
        public string UploadKey { get; set; }
        public string DownloadKey { get; set; }
        public string ViewKey { get; set; }
        public Nullable<byte> StorageMode { get; set; }
        [StrLen(50)]
        public string Classification { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }

        public virtual ICollection<tbl_docmgr_version> tbl_docmgr_version { get; set; }
        public virtual ICollection<tbl_process_document> tbl_process_document { get; set; }
    }

    public partial class tbl_org_projectInput : Input
    {
        public string Code { get; set; }
        [Required]
        [StrLen(50)]
        public string Name { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        [Required]
        [Display(Name="Project Type")]
        public int mstr_Org_Project_TypeID { get; set; }
        [Display(Name = "Status")]
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> LocationOrgLevelID { get; set; }
        [Display(Name = "Initiation Date")]
        public Nullable<System.DateTime> InitiationDate { get; set; }
        [Required]
        [Display(Name = "Planned Start")]
        public System.DateTime PlannedStart { get; set; }
        [Display(Name = "Actual Start")]
        public Nullable<System.DateTime> ActualStart { get; set; }
        [Display(Name = "Planned End")]
        public Nullable<System.DateTime> PlannedEnd { get; set; }
        [Display(Name = "Actual End")]
        public Nullable<System.DateTime> ActualEnd { get; set; }
        public bool ContractSigned { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Client")]
        public Nullable<int> orgClientID { get; set; }
        [Required]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }
        [Required]
        [Display(Name = "Client Type")]
        public short ClientType { get; set; }
        [StrLen(200)]
        [Display(Name = "Contact Person")]
        public string ClientContactPerson { get; set; }
        [StrLen(200)]
        [Display(Name = "Contact Details")]
        public string ClientContactDetail { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> SponseredBy { get; set; }
        public Nullable<int> Status_User { get; set; }
        public Nullable<System.DateTime> ContractSignDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string projectStatus { get; set; }

        public virtual ICollection<tbl_org_proj_review_history> tbl_org_proj_review_history { get; set; }
        public virtual ICollection<tbl_org_proj_org_levelInput> tbl_org_proj_org_level { get; set; }
    }

    public partial class tbl_org_proj_org_levelInput : Input
    {
        public int tbl_Org_LevelID { get; set; }
        public string org_level_name { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> tbl_Org_Level_MasterID { get; set; }
        public bool NewEntry { get; set; }
    }

    public partial class tbl_process_documentInput : Input
    {
        public tbl_process_documentInput()
        {
            this.tbl_process_doc_revision = new HashSet<tbl_process_doc_revision>();
        }

        public Nullable<byte> mstr_Process_Doc_TypeID { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public Nullable<int> ReviewedBy { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public Nullable<short> mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> tbl_DocMgr_DocumentID { get; set; }

        public virtual tbl_docmgr_documentInput tbl_docmgr_document { get; set; }
        public virtual ICollection<tbl_process_doc_revision> tbl_process_doc_revision { get; set; }
    }

    public partial class tbl_process_rep_taskInput : Input
    {
        public tbl_process_rep_taskInput()
        {
            this.tbl_process_rep_rule = new HashSet<tbl_process_rep_rule>();
        }
        [Required]
        [Display(Name="Repository")]
        public int tbl_Process_RepositoryID { get; set; }
        [Required]
        [Display(Name="Sequence")]
        [Range(1,9999)]
        public short SequenceNo { get; set; }
        [Required]
        [StrLen(50)]
        public string Name { get; set; }
        [StrLen(100)]
        public string Description { get; set; }
        public byte Type { get; set; }
        [Display(Name="Status")]
        public Nullable<int> mstr_Process_LC_StatusID { get; set; } 
        [Display(Name="Default Status")]
        public Nullable<int> DefaultStatus { get; set; }
        [Display(Name="Completion Status")]
        public Nullable<int> CompletionStatus { get; set; }
        [Display(Name="Default Hours")]
        public Nullable<byte> DefaultHRS { get; set; }
        public Nullable<byte> DefaultMINS { get; set; }
        [Display(Name="Default Duration (Days)")]
        public Nullable<short> DefaultDurationDays { get; set; }
        public Nullable<bool> AssignmentMandatory { get; set; }
        public Nullable<bool> AsynExec { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> Tailored { get; set; }

        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; }
        [ForeignKey("DefaultStatus")]
        public virtual mstr_process_lc_status mstr_process_lc_status1 { get; set; }
        [ForeignKey("CompletionStatus")]
        public virtual mstr_process_lc_status mstr_process_lc_status2 { get; set; }
        public virtual ICollection<tbl_process_rep_rule> tbl_process_rep_rule { get; set; }
    }

    public partial class mstr_org_project_typeInput : Input
    {
        public mstr_org_project_typeInput()
        {
            this.mstr_org_phase_in_proj = new HashSet<mstr_org_phase_in_proj>();
        }
        [Required]
        [StrLen(50)]
        [Display(Name="Short Name")]
        public string ShortName { get; set; }
        [Required]
        [StrLen(100)]
        [Display(Name = "Long Name")]
        public string LongName { get; set; }
        [StrLen(200)]
        public string Description { get; set; }

        public virtual ICollection<mstr_org_phase_in_proj> mstr_org_phase_in_proj { get; set; }
    }

    public partial class mstr_org_proj_phaseInput : Input
    {
        public mstr_org_proj_phaseInput()
        {
            this.mstr_org_phase_in_proj = new HashSet<mstr_org_phase_in_proj>();
        }
        [Required]
        public short SequenceNo { get; set; }
        [Required]
        [StrLen(50)]
        [Display(Name="Short Name")]
        public string ShortName { get; set; }
        [Required]
        [StrLen(100)]
        [Display(Name = "Long Name")]
        public string LongName { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        [Range(0,100)]
        [Display(Name="Percentage Distribution")]
        public Nullable<short> PercentTime { get; set; }

        public virtual ICollection<mstr_org_phase_in_proj> mstr_org_phase_in_proj { get; set; }
    }

    public partial class mstr_org_functionInput : Input
    {
        [Required]
        [StrLen(50)]
        [Display(Name="Short Name")]
        public string ShortName { get; set; }
        [Required]
        [StrLen(100)]
        [Display(Name = "Long Name")]
        public string LongName { get; set; }
        [StrLen(100)]
        public string Description { get; set; }
        public Nullable<int> PrimaryResponsible { get; set; }

        public virtual ICollection<mstr_org_function_level> mstr_org_function_level { get; set; }
        public virtual ICollection<mstr_org_sub_function> mstr_org_sub_function { get; set; }
    }

    public class mstr_org_sub_functionInput : Input
    {
        [Required]
        [StrLen(50)]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
        [Required]
        [StrLen(100)]
        [Display(Name = "Long Name")]
        public string LongName { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        public Nullable<int> PrimaryResponsible { get; set; }
    }

    public class tbl_org_level_organisationInput : Input
    {
        [Required]
        [Display(Name="Organisation Level")]
        public int mstr_Org_LevelID { get; set; }
        public string levelName { get; set; }
        [Required]
        [Display(Name = "Level Details")]
        public int mstr_Org_Level_MasterID { get; set; }
        public string masterDataName { get; set; }
        public short Level { get; set; }
        public Nullable<int> mstr_Org_Level_ParentID { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        public string StructPath { get; set; }

        public virtual mstr_org_level mstr_org_level { get; set; }
        public virtual mstr_org_level_master mstr_org_level_master { get; set; }
    }

    public partial class mstr_org_phase_in_projInput : Input
    {
        [Required]
        [Display(Name="Project Phase")]
        public int mstr_Org_Proj_PhaseID { get; set; }
        public string PhaseName { get; set; }
        [Required]
        [Display(Name="Project Type")]
        public int mstr_Org_Project_TypeID { get; set; }
        public string ProjectTypeName { get; set; }

        [ForeignKey("mstr_Org_Proj_PhaseID")]
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        [ForeignKey("mstr_Org_Project_TypeID")]
        public mstr_org_project_type mstr_org_project_type { get; set; }
    }

    public partial class mstr_org_sub_in_functionInput : Input
    {
        [Required]
        [Display(Name = "Function")]
        public int mstr_Org_FunctionID { get; set; }
        public string FunctionName { get; set; }
        [Required]
        [Display(Name = "Sub Function")]
        public int mstr_Org_Sub_FunctionID { get; set; }
        public string SubFunctionName { get; set; }

        [ForeignKey("mstr_Org_FunctionID")]
        public mstr_org_function mstr_org_function { get; set; }
        [ForeignKey("mstr_Org_Sub_FunctionID")]
        public mstr_org_sub_function mstr_org_sub_function { get; set; }
    }

    public partial class tbl_mapping_org_processInput : Input
    {
        public Nullable<int> mstr_Org_Project_TypeID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public Nullable<int> mstr_Org_FunctionID { get; set; }
        public Nullable<int> mstr_Org_Sub_FunctionID { get; set; }
        public Nullable<int> tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public Nullable<int> tbl_Org_Level_OrganisationID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        public mstr_org_project_type mstr_org_project_type { get; set; }
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        public mstr_org_function mstr_org_function { get; set; }
        public mstr_org_sub_function mstr_org_sub_function { get; set; }
        public tbl_process_repository tbl_process_repository { get; set; }
        public tbl_process_rep_task tbl_process_rep_task { get; set; }
        public tbl_org_level_organisation tbl_org_level_organisation { get; set; }
    }

    public class returnData
    {
        public int ID { get; set; }
        public string IsActivity { get; set; }
        public int levelID;
        public string levelName;
        public string nodeName;
        public int Level;
        public Nullable<int> ParentID;
        public string accessID;
        public string paramName;
        public int paramValue;
        public string Type { get; set; }
        public string previewURL { get; set; }
        public string previewClass { get; set; }
        public bool Exclude { get; set; }   
    }

    public class tbl_process_rep_task_ref_docsInput : Input
    {
        [Display(Name="Original Name")]
        public string Name { get; set; }
        [Display(Name = "Name")]
        [StrLen(50)]
        [Required]
        public string TailorName { get; set; }
        [Display(Name = "Description")]
        [StrLen(100)]
        public string Description { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        [Required]
        public int tbl_Process_Repository_ID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        [Required]
        public int tbl_Org_ProjectID { get; set; }
        public List<refDoc> refDocs { get; set; }
    }

    public class refDoc : Input
    {
        [Required]
        public short DocType { get; set; }
        [Required]
        [Display(Name="Reference Document")]
        public int referenceID { get; set; }
        public string refKey { get; set; }
        public string DocumentName { get; set; }
        [Required]
        public Nullable<short> Mandatory { get; set; }
        [StrLen(200)]
        public string Remarks { get; set; }
    }

    public partial class tbl_mapping_roleInput : Input
    {
        [Required]
        [Display(Name="Process Role")]
        public int mstr_Process_RoleID { get; set; }
        public int mstr_Org_RoleID { get; set; }

        [ForeignKey("mstr_Process_RoleID")]
        public virtual mstr_process_role mstr_process_role { get; set; }
        [ForeignKey("mstr_Org_RoleID")]
        public virtual mstr_org_role mstr_org_role { get; set; }
    }

    public class mapRolesInput
    {
        public int ClientID {get; set;}

        public tbl_mapping_role tbl_mapping_role { get; set; }
    }

    public partial class mstr_clientInput : Input
    {
        [Required]
        [Display(Name="Client Name")]
        [StrLen(100)]
        public string ClientName { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        [StrLen(200)]
        public string Logo { get; set; }
        [StrLen(100)]
        public string ShortName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

    public class mstr_process_typeInput : Input
    {
        [Required]
        [Display(Name="Process Type")]
        [StrLen(50)]
        public string ShortName { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
    }

    public class mstr_access_menuInput : Input
    {
        public mstr_access_menuInput()
        {
            this.tbl_system_role_menu_access = new HashSet<tbl_system_role_menu_access>();
        }

        public string FunctionID { get; set; }
        public string FunctionGroup { get; set; }
        public short NestingLevel { get; set; }
        public bool IsMain { get; set; }
        public string ParentFunctionID { get; set; }
        public Nullable<short> DisplaySequence { get; set; }
        public string IconName { get; set; }
        public string ToolTip { get; set; }
        public string HelpText { get; set; }
        public string FriendlyName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string MainTable { get; set; }

        public virtual ICollection<tbl_system_role_menu_access> tbl_system_role_menu_access { get; set; }
    }

    public partial class vw_org_role_accessInput : Input
    {
        public string UserName { get; set; }
        public Nullable<int> Sys_Role { get; set; }
        public Nullable<int> mstr_Org_RoleID { get; set; }
        public Nullable<bool> PrimaryRole { get; set; }
        public int Menu_ID { get; set; }
        public int RefID { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public Nullable<short> DisplaySequence { get; set; }
        public string FriendlyName { get; set; }
        public string FunctionID { get; set; }
        public string HelpText { get; set; }
        public string IconName { get; set; }
        public bool IsMain { get; set; }
        public string MainTable { get; set; }
        public int NestingLevel { get; set; }
        public string ParentFunctionID { get; set; }
        public string ToolTip { get; set; }
        public string AccessType { get; set; }
        public Nullable<bool> addAccess { get; set; }
        public Nullable<bool> deleteAccess { get; set; }
        public Nullable<bool> updateAccess { get; set; }
    }

    public class seededMenu
    {
        public string functionID { get; set; }
        public IEnumerable<vw_org_role_access> vw_org_role_access { get; set; }
    }

    public class subMenuOptions
    {
        public string parentFunctionID { get; set; }

        public IOrderedEnumerable<vw_org_role_access> vw_org_role_access { get; set; }
    }

    public class searchEmployees
    {
        public int searchCode { get; set; }
        public string searchText { get; set; }
        public Nullable<int> projectID { get; set; }
        public Nullable<int> locationID { get; set; }
    }

    public class restrictAccessInput : Input
    {
        public int MenuID { get; set; }
        public string accessType { get; set; }
        public Nullable<bool> addAccess { get; set; }
        public Nullable<bool> deleteAccess { get; set; }
        public Nullable<bool> updateAccess { get; set; }
    }

    public class MenuAccessInfo
    {
        public string MenuID { get; set; }
        public Nullable<int> RefID { get; set; }
        public Nullable<int> RoleID { get; set; }
        public string RoleType { get; set; }
        public string RestrictType { get; set; }
    }

    public partial class mstr_org_clientInput : Input
    {
        [Required]
        [StrLen(50)]
        public string Name { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        [StrLen(200)]
        public string Address { get; set; }
        [StrLen(50)]
        public string Logo { get; set; }
        public short Type { get; set; }
        [StrLen(200)]
        [Display(Name="Primary Contact Person")]
        public string PrimaryContact { get; set; }
        [StrLen(200)]
        [Display(Name = "Primary Contact Details")]
        public string PContactMailID { get; set; }
    }

    public partial class tbl_org_proj_review_historyInput : Input
    {
        [Required]
        public int tbl_Org_ProjectID { get; set; }
        public string projectName { get; set; }
        [Required]
        public DateTime ReviewDate { get; set; }
        [Required]
        [StrLen(200)]
        public string Comments { get; set; }
        [Required]
        public int UserID { get; set; }
        public string userName { get; set; }
        public string CreaterName { get; set; }
        [Required]
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> prevUser { get; set; }

    }

    public partial class tbl_org_project_documentsInput : Input
    {
        [Display(Name = "Project")]
        public int tbl_Org_ProjectID { get; set; }
        [Display(Name="Document Type")]
        [Required]
        public Nullable<int> mstr_Process_Doc_TypeID { get; set; }
        public Nullable<int> tbl_docmgr_documentID { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        [StrLen(100)]
        public string Owner { get; set; }
        [Required]
        [Display(Name = "File to be uploaded")]
        public HttpPostedFileBase fileName { get; set; }

        public virtual tbl_org_project tbl_org_project { get; set; }
        public virtual mstr_process_doc_type mstr_process_doc_type { get; set; }
    }

    public partial class project_process_documents : Input
    {
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Process_Doc_TypeID { get; set; }
        public Nullable<int> tbl_docmgr_documentID { get; set; }
        public string Name { get; set; }
        public Nullable<byte> SourceType { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<int> ProcessDoc_RefID { get; set; }
        [Required]
        [AllowHtml]
        public string Contents { get; set; }
        [StrLen(100)]
        public string Owner { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
    }

    public partial class tbl_org_proj_doc_versionInput : Input
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_docmgr_documentID { get; set; }
        public string Comments { get; set; }
        public string Owner { get; set; }
        public string DocTypeName { get; set; }
        public string DocumentName { get; set; }
        public string RevisedBy { get; set; }
        public Nullable<decimal> Version { get; set; }
        [Required]
        [Display(Name = "File to be uploaded")]
        public HttpPostedFileBase fileName { get; set; }
    }

    public partial class mstr_process_doc_typeInput : Input
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValidExtensions { get; set; }
        public int MaxSizeMB { get; set; }
        public bool ImposeSizeLimit { get; set; }
        public string NameFormat { get; set; }
    }

    public partial class tbl_org_proj_groupInput : Input
    {
        [StrLen(50)]
        public string Name { get; set; }
        [StrLen(100)]
        public string Description { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public bool IsParent { get; set; }
        public short Level { get; set; }
        public Nullable<int> Parent_GroupID { get; set; }
    }

    public class proj_estm_group 
    {
        public int Proj_EstimationID { get; set; }
        public int ProjectID { get; set; }
        [Required]
        [Display(Name="Module")]
        public Nullable<int> GroupID { get; set; }
        [Required]
        [StrLen(50)]
        [Display(Name="Module Name")]
        public string Name { get; set; }
        public Nullable<int> OldGroupID { get; set; }
        public string ExcludeIDs { get; set; }
    }

    public class proj_estm_size_parameter : Input
    {
        public int Proj_EstimationID { get; set; }
        public int ProjectID { get; set; }
        public int GroupID { get; set; }
        [Required]
        public Nullable<int> ParameterID { get; set; }
        [Required]
        [StrLen(50)]
        [Display(Name = "Parameter Name")]
        public string Name { get; set; }
        public decimal Simple { get; set; }
        public decimal Medium { get; set; }
        public decimal Complex { get; set; }
        public string ExcludeIDs { get; set; }
        public bool IsCutPhase { get; set; }
    }

    public class proj_estm_GSC : Input
    {
        public int Proj_EstimationID { get; set; }
        public int ProjectID { get; set; }
        [Required]
        [Display(Name="GSC")]
        public Nullable<int> GSCID { get; set; }
        [Required]
        [StrLen(50)]
        [Display(Name = "GSC Name")]
        public string Name { get; set; }
        public string ExcludeIDs { get; set; }
        public string Helptext { get; set; }
    }

    public class seededProjGroup
    {
        public int ProjectID { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public short Level { get; set; }
        public IEnumerable<tbl_org_proj_group> tbl_org_proj_group { get; set; }
    }

    public class tbl_org_project_process_mappingInput : Input
    {
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_TaskID { get; set; }
        public bool Exclude { get; set; }
        public string TailorName { get; set; }
        public Nullable<bool> TreatAsTask { get; set; }
        public Nullable<bool> TailorNew { get; set; }

        public tbl_process_repository tbl_process_repository { get; set; }
        public tbl_process_rep_task tbl_process_rep_task { get; set; }
        public tbl_org_project tbl_org_project { get; set; }
    }

    public partial class tbl_org_proj_locationInput : Input
    {
        public int tbl_Org_ProjectID { get; set; }
        [Required]
        [StrLen(100)]
        public string Name { get; set; }
        [Required]
        [Display(Name="Sequence No")]
        public byte SequenceNo { get; set; }
        [Required]
        public byte Type { get; set; }
        public Nullable<int> LevelMasterID { get; set; }
        public Nullable<int> LevelID { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
    }

    public class ProjOrgLevelMapping : Input
    {
        public string ProjectName { get; set; }
        [Required]
        [Display(Name="Organisation Level")]
        public int LevelID { get; set; }
    }

    public partial class tbl_org_proj_allocationInput : Input
    {
        public int tbl_Org_ProjID { get; set; }
        public string ProjectName { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Display(Name = "Group")]
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        [Display(Name = "Location")]
        public Nullable<int> tbl_Org_Proj_LocationID { get; set; }
        [Required]
        [Display(Name = "Role")]
        public int mstr_Org_RoleID { get; set; }
        [Required]
        [Display(Name = "Planned Start")]
        public System.DateTime PlannedStartDate { get; set; }
        public Nullable<System.DateTime> ActualStartDate { get; set; }
        [Required]
        [Display(Name = "Planned End")]
        public Nullable<System.DateTime> PlannedEndDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        [Required]
        [Display(Name = "Allocation Percent")]
        [Range(1,100)]
        public short Percent_Allocation { get; set; }
        public Nullable<int> ReportingTo { get; set; }
        public Nullable<bool> WorkingResource { get; set; }
        public Nullable<bool> ReviewResource { get; set; }
        public Nullable<bool> Stakeholder { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<bool> ManagementResource { get; set; }
        public Nullable<bool> DefectAdmin { get; set; }
        public Nullable<bool> IssueAdmin { get; set; }
        public Nullable<bool> HelpDeskAdmin { get; set; }
        public Nullable<bool> CRAdmin { get; set; }
        public Nullable<short> TimesheetHRS { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        public Nullable<int> RequisitionID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

    public partial class tbl_org_proj_planInput : Input
    {
        public tbl_org_proj_planInput()
        {
            this.tbl_org_plan_resource = new List<tbl_org_plan_resourceInput>();
            this.tbl_org_plan_document = new List<tbl_org_plan_documentInput>();
        }
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> ProjectType { get; set; }
        [Display(Name="Group")]
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        [Display(Name = "Phase")]
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        [Display(Name = "Plan")]
        public Nullable<int> PlanID { get; set; }
        public string TaskName { get; set; }
        [Display(Name = "Description")]
        public string TaskDescription { get; set; }
        [Required]
        [Display(Name="Task")]
        public int tbl_Mapped_Proj_ProcessID { get; set; }
        public Nullable<int> tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        [Required]
        [Display(Name = "Status")]
        public int mstr_Process_LC_StatusID { get; set; }
        [Required]
        [Display(Name="Planned Start")]
        public System.DateTime PlannedStartDate { get; set; }
        public Nullable<System.DateTime> ActualStartDate { get; set; }
        [Required]
        [Display(Name = "Planned End")]
        public Nullable<System.DateTime> PlannedEndDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        public Nullable<System.DateTime> BaselineStart { get; set; }
        public Nullable<System.DateTime> BaselineEnd { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<bool> IsMilestone { get; set; }
        public Nullable<bool> AutoCreated { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        [Required]
        [Display(Name = "Planned Duration")]
        [Range(1,999)]
        public decimal PlannedDuration { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        [Required]
        [Display(Name="Unit")]
        public Nullable<byte> DurationUnit { get; set; }
        public Nullable<short> PercentComplete { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<int> NextTaskID { get; set; }
        public Nullable<bool> RuleExecutionPending { get; set; }
        public Nullable<bool> RuleApplicable { get; set; }
        public string Text1Caption { get; set; }
        public string Text2Caption { get; set; }
        public string Text3Caption { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string FunctionID { get; set; }

        public tbl_org_proj_planname tbl_org_proj_planname { get; set; }
        public tbl_process_repository tbl_process_repository { get; set; }
        public tbl_process_rep_task tbl_process_rep_task { get; set; }
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
        public ICollection<tbl_org_plan_resourceInput> tbl_org_plan_resource { get; set; }
        public ICollection<tbl_org_plan_documentInput> tbl_org_plan_document { get; set; }
        public ICollection<uploaded_doc> uploaded_doc { get; set; }

        public bool validate(Db ctx, int ClientID, int currentUser, out List<ValidationMessage> messages)
        {
            bool result = false;
            messages = new List<ValidationMessage>();

            var selectedTask = ctx.tbl_org_project_process_mapping.Where(o => o.ID == tbl_Mapped_Proj_ProcessID).SingleOrDefault();
            if (selectedTask == null)
            {
                messages.Add(new ValidationMessage() {
                        key = "tbl_Mapped_Proj_ProcessID",
                        message = "Task mapping not found"
                });
                result = true;
            }

            tbl_Process_RepositoryID = selectedTask.tbl_Process_RepositoryID;
            tbl_Process_Rep_TaskID = selectedTask.tbl_Process_TaskID;
            this.ClientID = ClientID;
            CreateDate = System.DateTime.Now;
            CreatedBy = currentUser;

            bool test1 = false, test2 = false, test3 = false, test4 = false;
            decimal totalDuration = 0;

            var entity = ctx.tbl_org_proj_plan.Where(o => ((o.tbl_Process_Rep_TaskID == null && tbl_Process_Rep_TaskID == null) || o.tbl_Process_Rep_TaskID == tbl_Process_Rep_TaskID)
                                       && ((o.tbl_Process_RepositoryID == null && tbl_Process_RepositoryID == null) || o.tbl_Process_RepositoryID == tbl_Process_RepositoryID)
                                       && ((o.PlanID == null && PlanID == null) || o.PlanID == PlanID)
                                       && ((o.tbl_Org_Proj_GroupID == null && tbl_Org_Proj_GroupID == null) || o.tbl_Org_Proj_GroupID == tbl_Org_Proj_GroupID)
                                       && o.PlannedStartDate == PlannedStartDate && o.PlannedEndDate == PlannedEndDate
                                       && o.PlannedDuration == PlannedDuration && o.ID != ID);
            if (entity.Any())
            {
                result = true;
                messages.Add(new ValidationMessage()
                {
                    key = "",
                    message = "Duplicate Entry Found"
                });
            }
            if (PlannedEndDate < PlannedStartDate)
            {
                result = true;
                messages.Add(new ValidationMessage()
                {
                    key = "",
                    message = "Task start date and end dates cannot be out of sequence"
                });
            }

            var duplicateResources = from r in tbl_org_plan_resource
                                     group r by r.tbl_Org_EmployeeID into grp
                                     where grp.Count() > 1
                                     select new { key = grp.Key, cnt = grp.Count() };
            if (duplicateResources.Count() > 0)
            {
                foreach (var emp in duplicateResources)
                {
                    result = true;
                    messages.Add(new ValidationMessage()
                    {
                        key = "",
                        message = "Resource has been allocated more than once"
                    });
                }
            }
            if (tbl_org_plan_resource != null && tbl_org_plan_resource.Any())
            {
                foreach (var r in tbl_org_plan_resource)
                {
                    if (r.PlannedStart < r.AllocationStart || r.PlannedStart > r.AllocationEnd)
                    {
                        result = true;
                        messages.Add(new ValidationMessage()
                        {
                            key = "tbl_org_plan_resource[" + r.ID + "].PlannedStart",
                            message = "*"
                        }); 
                        if (!test1)
                        {
                            messages.Add(new ValidationMessage()
                            {
                                key = "",
                                message = "Planned Start cannot be outside resource allocation dates"
                            });
                            test1 = true;
                        }
                    }
                    if (r.PlannedEnd < r.AllocationStart || r.PlannedEnd > r.AllocationEnd)
                    {
                        result = true;
                        messages.Add(new ValidationMessage()
                        {
                            key = "tbl_org_plan_resource[" + r.ID + "].PlannedEnd",
                            message = "*"
                        });
                        if (!test2)
                        {
                            messages.Add(new ValidationMessage()
                            {
                                key = "",
                                message = "Planned End cannot be outside resource allocation dates"
                            });
                            test2 = true;
                        }
                    }
                    if (r.PlannedEnd < r.PlannedStart)
                    {
                        result = true;
                        messages.Add(new ValidationMessage()
                        {
                            key = "tbl_org_plan_resource[" + r.ID + "].PlannedEnd",
                            message = "*"
                        });
                        messages.Add(new ValidationMessage()
                        {
                            key = "tbl_org_plan_resource[" + r.ID + "].PlannedStart",
                            message = "*"
                        });
                        if (!test3)
                        {
                            messages.Add(new ValidationMessage()
                            {
                                key = "",
                                message = "Planned start and end dates cannot be out of sequence"
                            });
                            test3 = true;
                        }
                    }
                    if (r.PlannedDuration > PlannedDuration)
                    {
                        result = true;
                        messages.Add(new ValidationMessage()
                        {
                            key = "tbl_org_plan_resource[" + r.ID + "].PlannedDuration",
                            message = "*"
                        });
                        if (!test4)
                        {
                            messages.Add(new ValidationMessage()
                            {
                                key = "",
                                message = "Resource duration is greater than the task duration"
                            });
                            test4 = true;
                        }
                        totalDuration += r.PlannedDuration.GetValueOrDefault();
                    }
                    if (totalDuration > PlannedDuration)
                    {
                        result = true;
                        messages.Add(new ValidationMessage()
                        {
                            key = "",
                            message = "Total duration of individual resources cannot be greater than the task duration"
                        });
                    }
                }
            }
            return result;
        }
    }

    public class planSupportingDocuments    // used for selecting more documents for a planned task.
    {
        public int PlanID { get; set; }
        public int MapID { get; set; }
        public string excludeIDs { get; set; }
        public int key { get; set; }
        public ICollection<tbl_org_plan_documentInput> tbl_org_plan_document { get; set; }
    }

    public class tbl_org_plan_resourceInput : Input
    {
        public int tbl_Org_Proj_PlanID { get; set; }
        public string ProjectName { get; set; }
        [Required]
        [Display(Name="Team Member")]
        public int tbl_Org_EmployeeID { get; set; }
        public string EmpName { get; set; }
        [Required]
        [Range(1,100)]
        [Display(Name = "Allocation Percent")]
        public byte AllocationPercent { get; set; }
        [Display(Name = "Planned Start")]
        [Required]
        public System.DateTime PlannedStart { get; set; }
        public Nullable<DateTime> AllocationStart { get; set; }
        public Nullable<DateTime> AllocationEnd { get; set; }
        public Nullable<System.DateTime> ActualStart { get; set; }
        [Display(Name = "Planned End")]
        [Required]
        public Nullable<System.DateTime> PlannedEnd { get; set; }
        public Nullable<System.DateTime> ActualEnd { get; set; }
        [Display(Name = "Planned Duration")]
        [Required]
        public Nullable<decimal> PlannedDuration { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        public Nullable<short> PercentComplete { get; set; }
        public Nullable<bool> Billable { get; set; }
        public int tbl_Process_Rep_ActivityID { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<int> OrgResource_AllocationID { get; set; }
        public Nullable<bool> AutoAssigned { get; set; }
        public Nullable<System.DateTime> AssignPickUpDate { get; set; }
        public string Comments { get; set; }
    }

    public class ProjectPlanFilter
    {
        public int SearchProjectID { get; set; }
        public Nullable<int> SearchTaskID { get; set; }
        public Nullable<DateTime> SearchFromDate { get; set; }
        public Nullable<DateTime> SearchToDate { get; set; }
        public Nullable<int> SearchPlanID { get; set; }
        public Nullable<int> SearchGroupID { get; set; }
        public Nullable<int> SearchStatusID { get; set; }
        public Nullable<int> SearchTrackID { get; set; }
        
    }

    public class TaskRecordingFilter
    {
        public int SearchProjectID { get; set; }
        public Nullable<DateTime> SearchFromDate { get; set; }
        public Nullable<DateTime> SearchToDate { get; set; }
        public Nullable<int> SearchGroupID { get; set; }
        public Nullable<int> SearchStatusID { get; set; }
        public Nullable<int> SearchTaskID { get; set; }
        public Nullable<int> SearchRecordTypeID { get; set; }
    }

    public class tbl_org_proj_plannameInput : Input
    {
        [Required]
        [StrLen(100)]
        public string Name { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<DateTime> PlannedStart { get; set; }
        public Nullable<DateTime> PlannedEnd { get; set; }
    }

    public class GroupListItem
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public string DisplayText { get; set; }
    }

    public class KeyListItem
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public string Key { get; set; }
        public string DisplayText { get; set; }
    }

    public class tbl_org_plan_documentInput : Input
    {
        public bool Include { get; set; }
        public int tbl_Org_PlanID { get; set; }
        public byte DocType { get; set; }
        public string Name { get; set; }
        public Nullable<int> tbl_DocMgr_DocumentID { get; set; }
        public Nullable<int> tbl_Process_ProcedureID { get; set; }
        public Nullable<int> tbl_Process_ChecklistID { get; set; }
        public Nullable<int> tbl_Process_TemplateID { get; set; }
        public byte Source { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<byte> ReferenceType { get; set; }
    }
    public class resourceWiseTasks : Input
    {
        public int tbl_Org_ProjID { get; set; }
        public string ProjectName { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Display(Name = "Group")]
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        [Display(Name = "Location")]
        public Nullable<int> tbl_Org_Proj_LocationID { get; set; }
        [Required]
        [Display(Name = "Role")]
        public int mstr_Org_RoleID { get; set; }
        [Required]
        [Display(Name = "Planned Start")]
        public System.DateTime PlannedStartDate { get; set; }
        public Nullable<System.DateTime> ActualStartDate { get; set; }
        [Required]
        [Display(Name = "Planned End")]
        public Nullable<System.DateTime> PlannedEndDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        [Required]
        [Display(Name = "Allocation Percent")]
        [Range(1, 100)]
        public short Percent_Allocation { get; set; }
        public Nullable<int> ReportingTo { get; set; }
        public Nullable<bool> WorkingResource { get; set; }
        public Nullable<bool> ReviewResource { get; set; }
        public Nullable<bool> Stakeholder { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<bool> ManagementResource { get; set; }
        public Nullable<short> TimesheetHRS { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
        public Nullable<int> RequisitionID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public decimal PlannedDuration { get; set; } 
        public decimal ActualDuration { get; set; }
        public int ActualPercentAllocation { get; set; }

        public IEnumerable<tbl_org_plan_resource> tbl_org_plan_resource { get; set; } 
    }
    public class uploaded_doc : Input
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Org_PlanID { get; set; }
        [Required]
        [Display(Name = "File to be uploaded")]
        public HttpPostedFileBase fileName { get; set; }
        [StrLen(200)]
        public string Remarks { get; set; }
        public byte ReferenceType { get; set; }
    }

    public partial class tbl_org_timesheetInput : Input
    {
        public int tbl_Org_EmployeeID { get; set; }
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 2)]
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public string GroupName { get; set; }
        public string SubGroupName { get; set; }
        public string TaskName { get; set; }
        public Nullable<DateTime> PlannedStart { get; set; }
        public Nullable<DateTime> PlannedEnd { get; set; }
        public Nullable<DateTime> ActualStart { get; set; }
        public Nullable<DateTime> ActualEnd { get; set; }
        public Nullable<int> TaskStatus { get; set; }
        public Nullable<short> mstr_Org_FunctionID { get; set; }
        public Nullable<int> tbl_Org_Proj_PlanID { get; set; }
        public Nullable<int> tbl_Org_Plan_ResourceID { get; set; }
        [ForeignKey("tbl_org_proj_group")]
        [Column(Order = 1)]
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        public Nullable<int> mstr_Org_Sub_FunctionID { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Mapped_Proj_ProcessID { get; set; }
        public Nullable<int> tbl_Process_RepositoryID { get; set; }
        public Nullable<int> tbl_Process_Rep_TaskID { get; set; }
        public Nullable<int> tbl_General_TaskID { get; set; }
        public decimal Duration { get; set; }
        public Nullable<decimal> PlannedDuration { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<bool> IsPublish { get; set; }
        public Nullable<bool> IsReview { get; set; }
        public Nullable<int> res_StatusID { get; set; }
        public string res_TaskStatus { get; set; }
        public System.DateTime TSDate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<decimal> BillableDuration { get; set; }
        public Nullable<decimal> OvertimeDuration { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }
        public string Comments { get; set; }
        public Nullable<int> ApprovalID { get; set; }

        public tbl_org_employee tbl_org_employee { get; set; }
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        public mstr_org_function mstr_org_function { get; set; }
        public mstr_org_sub_function mstr_org_sub_function { get; set; }
        public tbl_org_project tbl_org_project { get; set; }
        public tbl_org_proj_plan tbl_org_proj_plan { get; set; }
    }

    public class timesheetEntry : Input
    {
        public DateTime TS_StartDate { get; set; }
        public Nullable<DateTime> TS_EndDate { get; set; }
        public int tbl_Org_EmployeeID { get; set; }       
        public byte ViewType { get; set; }      // 1 - Daily, 2 - Weekly, 3 - Monthly
        public Nullable<int> StatusID { get; set; }
        public string StatusName { get; set; }
        public ICollection<tbl_org_timesheetInput> tbl_org_timesheet { get; set; }
    }

    public class tbl_org_plan_filled_documentInput : Input
    {
        public int tbl_Org_PlanID { get; set; }
        public int tbl_Org_Plan_ResourceID { get; set; }
        public Nullable<int> tbl_Org_Plan_DocumentID { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<int> Uploaded_Doc { get; set; }
        [AllowHtml]
        public string Contents { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<DateTime> UploadedDate { get; set; }
    }

    public class TaskSupportingDocs
    {
        public int ID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public byte Type { get; set; }
        public string DocTypeName { get; set; }
        public string Name { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public int tbl_Process_TaskID { get; set; }
    }

    public partial class tbl_org_proj_estimationInput : Input
    {
        public int tbl_Org_ProjectID { get; set; }
        public int ProjectType { get; set; }
        public short Version { get; set; }
        public Nullable<bool> CurrentVersion { get; set; }
        public System.DateTime CreatedOn { get; set; }
        [Required]
        [Display(Name="SDLC Phase")]
        public int tbl_Org_Proj_PhaseID { get; set; }
        public string phaseName { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Size { get; set; }
        public Nullable<decimal> SimpleEfforts { get; set; }
        public Nullable<decimal> MediumEfforts { get; set; }
        public Nullable<decimal> ComplexEfforts { get; set; }
        public Nullable<decimal> TotalEfforts { get; set; }
        public Nullable<decimal> ScheduleMonths { get; set; }
        public Nullable<decimal> TDI { get; set; }
        public Nullable<decimal> VAF { get; set; }
        public Nullable<decimal> CUT_Effort_FPs { get; set; }
        public Nullable<decimal> CUT_Effort_PDs { get; set; }
        [Required]
        [Display(Name="CUT Effort %")]
        [Range(1,100)]
        public Nullable<decimal> CUT_EffortPercent { get; set; }
        public Nullable<decimal> CUT_TeamSize { get; set; }
        public Nullable<decimal> Overall_Project_PDs { get; set; }
        public Nullable<decimal> Team_Productivity { get; set; }
        [Required]
        [Display(Name = "Defect Density")]
        [Range(0, double.MaxValue)]
        public Nullable<decimal> DefectDensity { get; set; }
        public Nullable<decimal> TotalDefects { get; set; }
        public Nullable<decimal> TotalCutDefects { get; set; }
        public bool newVersion { get; set; }
        [Required]
        [Display(Name = "PM SBR")]
        [Range(1, double.MaxValue)]
        public Nullable<decimal> PMSBR { get; set; }

        public tbl_org_project tbl_org_project { get; set; }
        public ICollection<tbl_org_proj_estm_size> tbl_org_proj_estm_size { get; set; }
        public ICollection<tbl_org_proj_estm_gsc> tbl_org_proj_estm_gsc { get; set; }
        public ICollection<tbl_org_proj_estm_productivity> tbl_org_proj_estm_productivity { get; set; }
        public ICollection<tbl_org_proj_estm_effort_schedule> tbl_org_proj_estm_effort_schedule { get; set; }
        public ICollection<proj_estm_group> proj_estm_group { get; set; }
        public IEnumerable<tbl_org_estm_gsc_master> tbl_org_estm_gsc_master { get; set; }

        public void loadData(Db ctx, int clientID)
        {
            tbl_org_estm_gsc_master = ctx.tbl_org_estm_gsc_master.Where(o => o.ClientID == clientID);
            if (tbl_org_proj_estm_size != null)
            {
                foreach (var g in tbl_org_proj_estm_size)
                {
                    ctx.tbl_org_proj_estm_size.Attach(g);
                    ctx.Entry(g).Reference(k => k.tbl_org_proj_group).Load();
                    ctx.Entry(g).Reference(k => k.tbl_org_estm_parameters).Load();
                }
                proj_estm_group = new List<proj_estm_group>();
                foreach (var grp in tbl_org_proj_estm_size.Select(o => new { o.ProjGroupID, o.tbl_org_proj_group.Name }).Distinct())
                {
                    proj_estm_group.Add(new proj_estm_group()
                    {
                        GroupID = grp.ProjGroupID,
                        Name = grp.Name,
                        OldGroupID = grp.ProjGroupID,
                        ProjectID = tbl_Org_ProjectID,
                        Proj_EstimationID = ID
                    });
                }
            }
            if (tbl_org_proj_estm_effort_schedule != null)
            {
                foreach (var p in tbl_org_proj_estm_effort_schedule)
                {
                    ctx.tbl_org_proj_estm_effort_schedule.Attach(p);
                    ctx.Entry(p).Reference(k => k.mstr_org_proj_phase).Load();
                }
            }
            if (tbl_org_proj_estm_productivity != null)
            {
                foreach (var prd in tbl_org_proj_estm_productivity)
                {
                    ctx.tbl_org_proj_estm_productivity.Attach(prd);
                    ctx.Entry(prd).Reference(k => k.mstr_org_role).Load();
                }
            }
        }
    }

    public partial class tbl_org_estm_parametersInput : Input
    {
        [Required]
        [StrLen(100)]
        [Display(Name = "Parameter Name")]
        public string Name { get; set; }
        [Display(Name="Simple Weightage")]
        public decimal Simple { get; set; }
        [Display(Name = "Simple Weightage")]
        public decimal Medium { get; set; }
        [Display(Name = "Simple Weightage")]
        public decimal Complex { get; set; }
    }

    public partial class tbl_org_estm_gsc_masterInput : Input
    {
        [StrLen(100)]
        [Required]
        [Display(Name="General System Characteristic")]
        public string Name { get; set; }
        [AllowHtml]
        public string HelpText { get; set; }
    }

    public partial class tbl_audit_planInput : Input
    {
        public string RefID { get; set; }
        [Required]
        [Display(Name="Activity Type")]
        public byte Type { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> tbl_Mstr_Org_FunctionID { get; set; }
        [Required(ErrorMessage="Provide a date")]
        public DateTime Start { get; set; }
        [Required(ErrorMessage = "Provide a date")]
        public DateTime Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        [Display(Name = "Execution Frequency")]
        public byte AuditType { get; set; }
        public List<int> AuditorRoles { get; set; }
        public List<int> Participants { get; set; }
        [Display(Name="Recurring Window")]
        public Nullable<byte> Frequency { get; set; }
        public Nullable<byte> Period { get; set; }
        public byte Status { get; set; }
        [StrLen(200)]
        [Display(Name="Description")]
        public string Comments { get; set; }

        public ICollection<tbl_audit_schedule> tbl_audit_schedule { get; set; }
    }

    public partial class tbl_audit_scheduleInput : Input
    {
        public int tbl_Audit_PlanID { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> tbl_Mstr_Org_FunctionID { get; set; }
        public DateTime Planned_Start { get; set; }
        public DateTime Planned_Finish { get; set; }
        [Required]
        [Display(Name = "Actual Start")]
        public Nullable<DateTime> Start { get; set; }
        [Required]
        [Display(Name = "Actual Finish")]
        public Nullable<DateTime> Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        [Display(Name="Review Period Start")]
        public Nullable<DateTime> Review_Start { get; set; }
        [Display(Name = "Review Period Finish")]
        public Nullable<DateTime> Review_Finish { get; set; }
        public byte Status { get; set; }
        public Nullable<DateTime> RecordingDate { get; set; }
        public Nullable<decimal> TimeSpentHrs { get; set; }
        public List<int> Audit_Roles { get; set; }
        public List<int> Auditors { get; set; }
        public List<int> Auditees { get; set; }
        public string Description { get; set; }

    }

    public partial class auditRecordingInput : Input
    {
        public int tbl_Audit_PlanID { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> tbl_Mstr_Org_FunctionID { get; set; }
        public DateTime Planned_Start { get; set; }
        public DateTime Planned_Finish { get; set; }
        [Required]
        [Display(Name = "Actual Start")]
        public Nullable<DateTime> Start { get; set; }
        [Required]
        [Display(Name = "Actual Finish")]
        public Nullable<DateTime> Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        [Display(Name = "Review Period Start")]
        public Nullable<DateTime> Review_Start { get; set; }
        [Display(Name = "Review Period Finish")]
        public Nullable<DateTime> Review_Finish { get; set; }
        public byte Status { get; set; }
        [Required]
        [Display(Name="Date")]
        public Nullable<DateTime> RecordingDate { get; set; }
        [Required]
        [Display(Name = "Hours Spent")]
        public Nullable<decimal> TimeSpentHrs { get; set; }
        public List<int> Auditors { get; set; }
        public List<int> Auditees { get; set; }
        public string Description { get; set; }
        public string Audit_Reference { get; set; }
        public string ProjectName { get; set; }


        public ICollection<tbl_audit_observationInput> tbl_audit_observation { get; set; }
        public ICollection<tbl_audit_checklistInput> tbl_audit_checklist { get; set; }
        public ICollection<tbl_audit_participantInput> tbl_audit_participant { get; set; }
    }

    public class tbl_audit_participantInput : Input
    {
        public int tbl_Audit_ScheduleID { get; set; }
        public byte Type { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public int tbl_Org_RoleID { get; set; }
        public string Name { get; set; }
    }
    
    public partial class tbl_audit_observationInput : Input
    {
        public int tbl_Audit_ScheduleID { get; set; }
        [Required]
        [Display(Name="Finding")]
        [AllowHtml]
        public string NC_Observation { get; set; }
        [AllowHtml]
        public string PossibleImpact_Attr { get; set; }
        public string CorrectiveAction { get; set; }
        public Nullable<int> Responsibility { get; set; }
        public Nullable<DateTime> EstimatedCloseDate { get; set; }
        public Nullable<DateTime> ActualCloseDate { get; set; }
        public short Status { get; set; }
        public Nullable<DateTime> StatusUpdateDate { get; set; }
        [StrLen(200)]
        public string Comments { get; set; }
    }

    public class tbl_audit_checklistInput : Input
    {
        public int tbl_Audit_ScheduleID { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int PhaseID { get; set; }
        public short GroupSequenceNo { get; set; }
        public string GroupName { get; set; }
        public short ChkLstSequenceNo { get; set; }
        public string Task { get; set; }
        public bool Applicable { get; set; }
        public string Score { get; set; }
        public decimal PCI_Score { get; set; }
        [AllowHtml]
        public string Comments { get; set; }
    }

    public class reviewCommentsInput : Input
    {
        public int ClosedBy { get; set; }
        public string AuditorName { get; set; }
        public DateTime CloseDate { get; set; }
        [Required]
        [StrLen(500)]
        [Display(Name="Review Comments")]
        public string ReviewComments { get; set; }
    }

    public class reviewOrgCommentsInput : Input
    {
        public int ClosedBy { get; set; }
        public string AuditorName { get; set; }
        public DateTime CloseDate { get; set; }
        [Required]
        [StrLen(500)]
        [Display(Name = "Notes")]
        public string ClosureComments { get; set; }
        [Display(Name = "Next Audit Date")]
        public Nullable<DateTime> NextAuditOn { get; set; }
        [Display(Name = "Audit Status")]
        public Nullable<byte> Status { get; set; }
        [StrLen(500)]
        [Display(Name = "Notes")]
        public string EvaluationCAPA { get; set; }
    }

    public class tbl_org_audit_planInput : Input
    {
        public string RefID { get; set; }
        [Required(ErrorMessage = "Provide a date")]
        public DateTime Start { get; set; }
        [Required(ErrorMessage = "Provide a date")]
        public DateTime Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        [Display(Name = "Execution Frequency")]
        public byte AuditType { get; set; }
        [Display(Name = "Recurring Window")]
        public Nullable<byte> Frequency { get; set; }
        public Nullable<byte> Period { get; set; }
        [StrLen(200)]
        [Display(Name = "Description")]
        public string Comments { get; set; }
        public List<int> AuditorRoles { get; set; }
        public byte Status { get; set; }

        public ICollection<tbl_org_audit_schedule> tbl_org_audit_schedule { get; set; }
    }

    public class tbl_org_audit_scheduleInput : Input
    {
        public int tbl_Org_Audit_PlanID { get; set; }
        public DateTime Planned_Start { get; set; }
        public DateTime Planned_Finish { get; set; }
        [Required]
        [Display(Name = "Actual Start")]
        public Nullable<DateTime> Start { get; set; }
        [Required]
        [Display(Name = "Actual Finish")]
        public Nullable<DateTime> Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<DateTime> Audit_Date { get; set; }
        public List<int> Audit_Roles { get; set; }
        public List<int> Auditors { get; set; }
        public List<int> Auditees { get; set; }
        public Nullable<byte> TimeSpentHrs { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }
        public string ClosureComments { get; set; }
        public Nullable<byte> ClosureStatus { get; set; }
        public Nullable<DateTime> NextAuditOn { get; set; }
        public string EvaluationCAPA { get; set; }
        public string Description { get; set; }

    }

    public partial class orgAuditRecordingInput : Input
    {
        public int tbl_Org_Audit_PlanID { get; set; }
        public DateTime Planned_Start { get; set; }
        public DateTime Planned_Finish { get; set; }
        [Required]
        [Display(Name = "Actual Start")]
        public Nullable<DateTime> Start { get; set; }
        [Required]
        [Display(Name = "Actual Finish")]
        public Nullable<DateTime> Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        public Nullable<byte> Status { get; set; }
        [Required]
        [Display(Name="Date")]
        public Nullable<DateTime> Audit_Date { get; set; }
        [Required]
        [Display(Name = "Hours Spent")]
        public Nullable<decimal> TimeSpentHrs { get; set; }
        public List<int> Auditors { get; set; }
        public List<int> Auditees { get; set; }
        public string Description { get; set; }
        public string Audit_Reference { get; set; }


        public ICollection<tbl_org_audit_observationInput> tbl_org_audit_observation { get; set; }
        public ICollection<tbl_org_audit_addln_obsInput> tbl_org_audit_addln_obs { get; set; }
        public ICollection<tbl_org_audit_participantInput> tbl_org_audit_participant { get; set; }
    }

    public class tbl_org_audit_participantInput : Input
    {
        public int tbl_Org_Audit_ScheduleID { get; set; }
        public byte Type { get; set; }
        public int tbl_Org_EmployeeID { get; set; }
        public string Name { get; set; }
    }
    
    public partial class tbl_org_audit_observationInput : Input
    {
        public int tbl_Org_Audit_ScheduleID { get; set; }
        [Required]
        [Display(Name="Non Conformance")]
        [AllowHtml]
        public string NCObservation { get; set; }
        [Display(Name="Objective Evidence")]
        [AllowHtml]
        public string EvidenceRefDocs { get; set; }
        public Nullable<byte> Category { get; set; }
        [StrLen(200)]
        public string Clause { get; set; }
        [StrLen(200)]
        public string Reference { get; set; }
        [Display(Name="Status Updated On")]
        public Nullable<DateTime> StatusUpdateDate { get; set; }
        public Nullable<DateTime> ActualCloseDate { get; set; }
        [Display(Name="Corrective Action")]
        public string CorrectiveAction { get; set; }
        [Display(Name="Root Cause Analysis")]
        public string RootCauseAnalysis { get; set; }
        [Display(Name="Target Date for CA")]
        public Nullable<DateTime> TargetDateCA { get; set; }
        [Display(Name="Preventive Action")]
        public string PreventiveAction { get; set; }
        [Display(Name="Target Date for PA")]
        public Nullable<DateTime> TargetDatePA { get; set; }
        [Display(Name="CA Status")]
        public Nullable<byte> StatusCA { get; set; }
        [Display(Name="PA Status")]
        public Nullable<byte> StatusPA { get; set; }
        public Nullable<byte> Status { get; set; }
    }

    public class tbl_org_audit_addln_obsInput : Input
    {
        public int tbl_Org_Audit_ScheduleID {get; set;}
        [AllowHtml]
        public string Observation { get; set; }
        public byte Type { get; set; }
    }

    public class mstr_org_general_tasksInput : Input
    {
        [Required]
        [StrLen(100)]
        [Display(Name="General Task")]
        public string Task { get; set; }
        [StrLen(200)]
        public string Description { get; set; }
        public Nullable<bool> Global { get; set; }
        public Nullable<int> OwnedByProject { get; set; }
        [Required]
        [Display(Name="Display Sequence")]
        public short Sequence { get; set; }
        public List<string> Roles { get; set; }
    }

    public class tbl_proj_general_tasksInput : Input, IValidatableObject
    {
        [Required]
        public int tbl_Org_General_TaskID { get; set; }
        [StrLen(100)]
        [Display(Name = "General Task")]
        public string GeneralTask { get; set; }
        [Required]
        public int tbl_Org_ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<bool> Global { get; set; }
        [Required]
        [Display(Name = "Display Sequence")]
        public short Sequence { get; set; }
        public Nullable<bool> Tailored { get; set; }
        public List<string> Roles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((tbl_Org_General_TaskID == 0 || Tailored == true) && string.IsNullOrEmpty(GeneralTask))
                yield return new ValidationResult("Enter a General Task",new[] { "GeneralTask" });
            if (Global != true)
            {
                if (Roles == null || !Roles.Any())
                    yield return new ValidationResult("Select Roles", new[] { "Roles" });
            }
        }
    }

    public class tbl_org_resourceplan_humanInput : Input
    {
        [Required]
        [Display(Name="Project")]
        public int tbl_Org_ProjectID { get; set; }
        [Required]
        [Display(Name = "Role")]
        public int tbl_Org_RoleID { get; set; }
        [Required]
        public string Skills { get; set; }
        [Required]
        public string JobDescription { get; set; }
        [Required]
        [Display(Name = "From")]
        public DateTime PlannedStart { get; set; }
        [Required]
        [Display(Name = "To")]
        public DateTime PlannedEnd { get; set; }
        [Required]
        [Range(1,int.MaxValue,ErrorMessage="Enter a value greater than 0")]
        public int Count { get; set; }
        [Required]
        [Range(1, 999, ErrorMessage = "Enter a value greater than 0")]
        public Decimal AllocationPercent { get; set; }
        public Nullable<int> tbl_Org_Proj_GroupID { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        [StrLen(500)]
        public string Remarks { get; set; }
        public Nullable<int> tbl_Org_Proj_LocationID { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }

        public tbl_org_project tbl_org_project { get; set; }
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        public mstr_org_role mstr_org_role { get; set; }
        public tbl_org_proj_location tbl_org_proj_location { get; set; }
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
    }

    public class tbl_org_resourceplan_hardwareInput : Input
    {
        [Required]
        public int tbl_Org_ProjectID { get; set; }
        [Required]
        [StrLen(500)]
        public string Details { get; set; }
        public string Specification { get; set; }
        public string Justification { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        [Display(Name="No of Resources")]
        public int Count { get; set; }
        public Nullable<bool> BillableToClient { get; set; }
        [Required]
        public DateTime PlannedFrom { get; set; }
        [Required]
        public DateTime PlannedTo { get; set; }
        [Required]
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }

        public tbl_org_project tbl_org_project { get; set; }
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
    }

    public class tbl_workflow_state_historyInput : Input
    {
        public int UserID { get; set; }
        public string ReviewItemName { get; set; }
        public string UserName { get; set; }
        public string FunctionID { get; set; }
        public int RefID { get; set; }
        public int Status { get; set; }
        public DateTime StatusDate { get; set; }
        [Required]
        [StrLen(200)]
        [Display(Name="Review Comments")]
        public string ReviewComments { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public string StatusName { get; set; }
        public string UpdateID { get; set; }
    }

    public class mstr_org_defect_typeInput : Input
    {
        [Required]
        [StrLen(100)]
        [Display(Name="Defect Type")]
        public string Type { get; set; }
        [Required]
        public short Sequence { get; set; }
    }

    public class mstr_org_defect_severityInput : Input
    {
        [Required]
        [StrLen(100)]
        [Display(Name = "Defect Severity")]
        public string Severity { get; set; }
        [Required]
        public short Sequence { get; set; }
    }

    public class tbl_org_defect_documentInput : Input
    {
        [Required]
        public int tbl_Org_DefectID { get; set; }
        [Required]
        [StrLen(200)]
        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }
        [Required]
        public int tbl_Docmgr_DocumentID { get; set; }
        public string Comments { get; set; }

        public tbl_org_defect tbl_org_defect { get; set; }
        public tbl_docmgr_document tbl_docmgr_document { get; set; }
    }

    public class tbl_org_defectInput : Input
    {
        [Required]
        [Display(Name="Short Description")]
        public string Short_Description { get; set; }
        [AllowHtml]
        [Required]
        public string Details { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> PhaseID { get; set; }
        public Nullable<int> GroupID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        [Required]
        [Display(Name = "Identified On")]
        public Nullable<DateTime> IdentifiedOn { get; set; }
        public string AffectedWP { get; set; }
        public Nullable<int> InjectedInPhaseID { get; set; }
        public string Cause { get; set; }
        [Required]
        public int SeverityID { get; set; }
        public string Impact { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public Nullable<DateTime> AssignedOn { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public string ActionTaken { get; set; }
        public Nullable<DateTime> FixedOn { get; set; }
        public Nullable<int> VerifiedBy { get; set; }
        public Nullable<DateTime> VerifiedOn { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> RepeatIssue { get; set; }
        [Display(Name="Resolution Efforts")]
        public Nullable<decimal> ResolutionEffortsPD { get; set; }

        public ICollection<tbl_org_defect_document> tbl_org_defect_document { get; set; }   
        public tbl_org_project tbl_org_project { get; set; }
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
        public mstr_org_defect_severity mstr_org_defect_severity { get; set; }
        public mstr_org_defect_type mstr_org_defect_type { get; set; }
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        public tbl_org_proj_group tbl_org_proj_group { get; set; }
        public tbl_org_proj_plan tbl_org_proj_plan { get; set; }
        public UserProfile AssignedToUser { get; set; }
        public UserProfile VerifiedByUser { get; set; }
    }

    public class VarianceGraphOutput
    {
        public int GraphType { get; set; }
        public decimal overallEffVariance { get; set; }
        public decimal overallSchVariance { get; set; }
        public decimal LCL { get; set; }
        public decimal UCL { get; set; }
        public string argumentField { get; set; }
        public List<VarianceGraphSeries> series { get; set; }
        public List<VarianceGraphData> data { get; set; }
        public string Title { get; set; }
    }

    public class VarianceGraphSeries
    {
        public string valueField { get; set; }
        public string name { get; set; }
        public string color { get; set; }
    }

    public class VarianceGraphData
    {
        public string project { get; set; }
        public decimal upperlimit { get; set; }
        public decimal metric { get; set; }
        public decimal lowerlimit { get; set; }
    }

    public class pieGraphOuput
    {
        public string argumentField { get { return "groupName"; } }
        public string valueField { get { return "groupCount"; } }
        public string type { get; set; }
        public List<pieGraphData> data { get; set; }
    }

    public class pieGraphData
    {
        public string groupName { get; set; }
        public long groupCount { get; set; }
    }

    public class barGraphOuput
    {
        public string argumentField { get { return "groupName"; } }
        public string valueField { get { return "groupCount"; } }
        public string type { get; set; }
        public List<barGraphData> data { get; set; }
    }

    public class barGraphData
    {
        public string groupName { get; set; }
        public decimal groupCount { get; set; }
    }
    public class tbl_workflowInput : Input
    {
        // Class to process inputs/changes received from the front end
        public tbl_workflowInput()
        {
            processed = false;
        }
        public string FunctionID { get; set; }
        public short Action { get; set; }
        public string UserCaption { get; set; }
        public short Sequence { get; set; }
        public int Status { get; set; }
        public Nullable<int> PreStatusID { get; set; }
        public Nullable<int> PostStatusID { get; set; }
        public Nullable<bool> AdminAccess { get; set; } 
        public Nullable<int> RoleAccess { get; set; }
        public Nullable<bool> SendMail { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<short> RoleType { get; set; }
        public Nullable<bool> Editable { get; set; }
        public Nullable<bool> Review { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Controller { get; set; }
        public string ActionName { get; set; }
        public string SuccessMessage { get; set; }
        public Nullable<bool> Dialog { get; set; }
        public Nullable<bool> ConfirmAction { get; set; }
        public Nullable<bool> WorkFlow { get; set; }
        public bool processed { get; set; }
    }

}

    

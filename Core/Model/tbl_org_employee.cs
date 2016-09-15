using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_employee: Entity
    {
        public tbl_org_employee()
        {
            this.mstr_org_structure = new HashSet<mstr_org_structure>();
        }
    
        public string EmpCode { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public short Gender { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public System.DateTime DateOfJoining { get; set; }
        public string EmailID { get; set; }
        public Nullable<System.DateTime> ConfirmationDate { get; set; }
        public Nullable<System.DateTime> DateOfResignation { get; set; }
        public Nullable<System.DateTime> DateRelieved { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public DateTime StatusDate { get; set; }
        public int CurrentDesignation { get; set; }
        public System.DateTime DesignationEffectiveFrom { get; set; }
        public Nullable<int> HR_Reporting { get; set; }
        public Nullable<int> Dept_Reporting { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Comments { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public virtual ICollection<mstr_org_structure> mstr_org_structure { get; set; }
        public virtual ICollection<tbl_org_emp_role> tbl_org_emp_role { get; set; }
        public virtual ICollection<tbl_org_employee_org_level> tbl_org_employee_org_level { get; set; }
        public virtual ICollection<tbl_org_proj_allocation> tbl_org_proj_allocation { get; set; }
        [ForeignKey("CurrentDesignation")]
        public virtual mstr_org_designation mstr_org_designation { get; set; }
        [ForeignKey("mstr_Process_LC_StatusID")]
        public virtual mstr_process_lc_status mstr_process_lc_status { get; set; } 
    }
}

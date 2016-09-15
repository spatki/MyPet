using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProcessAccelerator.Core.Model
{    
    public partial class mstr_org_role: Entity
    {
        public mstr_org_role()
        {
            this.mstr_org_structure = new HashSet<mstr_org_structure>();
        }
    
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public short Type { get; set; }
        public Nullable<int> mstr_Primary_Process_RoleID { get; set; }
        public Nullable<bool> Project_Approver { get; set; }
        public Nullable<bool> Project_Reviewer { get; set; }
        public Nullable<bool> Dept_Reporting { get; set; }
        public Nullable<bool> HR_Reporting { get; set; }
        public Nullable<bool> ReportingAccess { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [ForeignKey("mstr_Primary_Process_RoleID")]
        public virtual mstr_process_role mstr_process_role { get; set; }
        public virtual ICollection<mstr_org_structure> mstr_org_structure { get; set; }
    }
}

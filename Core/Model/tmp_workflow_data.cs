using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class user_type
    {
        public short Value { get; set; }
        public string Text { get; set; }
    }

    public class workflow_direction
    {
        public Nullable<bool> Value { get; set; }
        public string Text { get; set; }
    }

    public class workflow_edit
    {
        public int ID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public string FunctionID { get; set; }
        public short Action { get; set; }
        public string ImplementationName { get; set; }
        public string UserCaption { get; set; }
        public short Sequence { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> PreStatusID { get; set; }
        public List<int?> RoleAccess { get; set; }
        public Nullable<bool> SendMail { get; set; }
        public Nullable<int> PostStatusID { get; set; }
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
        public Nullable<bool> Workflow { get; set; }
        public string UserName { get; set; }
        public string AccessRoleName { get; set; }
        public string PreStatusName { get; set; }
        public string PostStatusName { get; set; }
        public Nullable<short> TimeLimit { get; set; }
        public List<int?> RowIDs { get; set; }
    }

    public class tmp_workflow_data
    {
        public string FunctionID { get; set; }
        public int newID { get; set; }
        public IEnumerable<tbl_workflow_implementation> actions { get; set; }
        public List<workflow_edit> workflow { get; set; }
        public workflow_edit emptyWorkflow { get; set; }
        public List<mstr_process_lc_status> status { get; set; }
        public List<mstr_org_role> role { get; set; }
        public IEnumerable<user_type> workflow_user_type { get; set; }
        public IEnumerable<workflow_direction> workflow_direction { get; set; }
    }
}

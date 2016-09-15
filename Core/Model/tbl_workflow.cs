using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_workflow : Entity
    {
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
        public Nullable<short> TimeLimit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_org_role_access : Entity
    {
        public Nullable<int> Sys_Role { get; set; }
        public Nullable<int> Org_Role { get; set; }
        public int Menu_ID { get; set; }
        public int RefID { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public Nullable<short> DisplaySequence { get; set; }
        public string FriendlyName { get; set; }
        public string FunctionID { get; set; }
        public int accessRange { get; set; }
        public string HelpText { get; set; }
        public string IconName { get; set; }
        public string IconColour { get; set; }
        public bool IsMain { get; set; }
        public string MainTable { get; set; }
        public short NestingLevel { get; set; }
        public string ParentFunctionID { get; set; }
        public string ToolTip { get; set; }
        public string AccessType { get; set; }
        public Nullable<bool> addAccess { get; set; }
        public Nullable<bool> deleteAccess { get; set; }
        public Nullable<bool> updateAccess { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_access_menu : Entity
    {
        public mstr_access_menu()
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
        public string IconColour { get; set; }


        public string ToolTip { get; set; }
        public string HelpText { get; set; }
        public string FriendlyName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string MainTable { get; set; }

        public virtual ICollection<tbl_system_role_menu_access> tbl_system_role_menu_access { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_role_mapping : Entity
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public Nullable<int> MapID { get; set; }
        public Nullable<int> mstr_Org_RoleID { get; set; }
        public Nullable<int> mstr_Process_RoleID { get; set; }
        public string processShortName { get; set; }
        public string processLongName { get; set; }
    }
}

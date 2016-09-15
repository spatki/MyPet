using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_org_sub_in_function : Entity
    {
        public int mstr_Org_FunctionID { get; set; }
        public int mstr_Org_Sub_FunctionID { get; set; }

        [ForeignKey("mstr_Org_FunctionID")]
        public mstr_org_function mstr_org_function { get; set; }
        [ForeignKey("mstr_Org_Sub_FunctionID")]
        public mstr_org_sub_function mstr_org_sub_function { get; set; }
    }
}

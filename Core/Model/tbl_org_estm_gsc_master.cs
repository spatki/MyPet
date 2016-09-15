using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_estm_gsc_master : Entity
    {
        public string Name { get; set; }
        public string HelpText { get; set; }
    }
}

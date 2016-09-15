using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_estm_parameters : Entity
    {
        public string Name { get; set; }
        public decimal Simple { get; set; }
        public decimal Medium { get; set; }
        public decimal Complex { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class mstr_process_type : Entity
    {
        public string ShortName { get; set; }
        public string Description { get; set; }

    }
}

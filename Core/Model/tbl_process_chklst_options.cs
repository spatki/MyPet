using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_chklst_options: Entity
    {
        public string Name { get; set; }
        public bool DefaultSelection { get; set; }
        public short DisplayOrder { get; set; }
        public Nullable<short> PercentageCorrect { get; set; }
        public Nullable<short> Type { get; set; }
    }
}

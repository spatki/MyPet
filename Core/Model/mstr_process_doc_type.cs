using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_process_doc_type: Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValidExtensions { get; set; }
        public int MaxSizeMB { get; set; }
        public bool ImposeSizeLimit { get; set; }
        public string NameFormat { get; set; }
    }
}

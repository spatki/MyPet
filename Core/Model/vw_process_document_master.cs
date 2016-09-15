using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_process_document_master
    {
        public int ID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public int Type { get; set; }
        public string DocTypeName { get; set; }
        public string Name { get; set; }
        public int Ref_ID { get; set; }
        public Nullable<int> mstr_Process_LC_StatusID { get; set; }
        public string Status { get; set; }
    }
}

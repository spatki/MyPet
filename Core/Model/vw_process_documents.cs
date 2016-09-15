using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_process_documents
    {
        public int ID { get; set; }
        public int ProcessDoc_RefID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public int Type { get; set; }
        public int SourceType { get; set; }
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string Name { get; set; }
        public Nullable<int> DocID { get; set; }
        public string Comments { get; set; }
    }
}

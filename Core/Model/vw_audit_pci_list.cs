using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAccelerator.Core.Model
{
    public class vw_audit_pci_list : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string RepositoryName { get; set; }
        public Nullable<int> PhaseID { get; set; }
        public string PhaseName { get; set; }
        public Nullable<int> GroupSequenceNo { get; set; }
        public Nullable<int> tbl_Process_TaskID { get; set; }
        public string TaskName { get; set; }
        public Nullable<int> TaskSequence { get; set; }
    }
}

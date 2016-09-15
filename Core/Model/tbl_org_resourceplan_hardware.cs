using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_resourceplan_hardware : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public string Details { get; set; }
        public string Specification { get; set; }
        public string Justification {get; set;}
        public int Count { get; set; }
        public Nullable<bool> BillableToClient {get; set;}
        public DateTime PlannedFrom { get; set; }
        public DateTime PlannedTo { get; set; }
        public int mstr_Process_LC_StatusID { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }

        [ForeignKey("tbl_Org_ProjectID")]
        public tbl_org_project tbl_org_project { get; set; }
        [ForeignKey("mstr_Process_LC_StatusID")]
        public mstr_process_lc_status mstr_process_lc_status { get; set; }
    }
}

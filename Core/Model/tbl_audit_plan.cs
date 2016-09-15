using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_audit_plan : Entity
    {
        public string RefID { get; set; }
        public byte Type { get; set; }
        public Nullable<int> tbl_Org_ProjectID { get; set; }
        public Nullable<int> tbl_Mstr_Org_FunctionID { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        public byte AuditType { get; set; }
        public Nullable<byte> Frequency { get; set; }
        public Nullable<byte> Period { get; set; }
        public byte Status { get; set; }
        public string Comments { get; set; }

        public ICollection<tbl_audit_schedule> tbl_audit_schedule { get; set; }
        public ICollection<tbl_audit_role> tbl_audit_role { get; set; }
    }
}

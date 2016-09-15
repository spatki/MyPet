using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_audit_plan : Entity
    {
        public string RefID { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        public byte AuditType { get; set; }
        public Nullable<byte> Frequency { get; set; }
        public Nullable<byte> Period { get; set; }
        public string Comments { get; set; }

        public ICollection<tbl_org_audit_schedule> tbl_org_audit_schedule { get; set; }
        public ICollection<tbl_org_audit_role> tbl_org_audit_role { get; set; }
    }
}

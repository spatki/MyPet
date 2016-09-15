using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_audit_addln_obs : Entity
    {
        [Key,Column(Order=0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int ID { get; set; }
        [Key, Column(Order = 1)]
        public int tbl_Org_Audit_ScheduleID { get; set; }
        [Key, Column(Order = 2)]
        public byte Type { get; set; }
        public string Observation { get; set; }

        [ForeignKey("tbl_Org_Audit_ScheduleID")]
        public tbl_org_audit_schedule tbl_org_audit_schedule { get; set; } 
    }
}

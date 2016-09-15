﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_audit_role : Entity
    {
        public int tbl_Org_Audit_PlanID { get; set; }
        public byte Type { get; set; }
        public int tbl_Org_RoleID { get; set; }

        [ForeignKey("tbl_Org_Audit_PlanID")]
        public tbl_org_audit_plan tbl_org_audit_plan { get; set; }
        [ForeignKey("tbl_Org_RoleID")]
        public mstr_org_role mstr_org_role { get; set; }
    }
}

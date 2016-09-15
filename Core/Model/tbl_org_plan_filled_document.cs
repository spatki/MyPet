using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_org_plan_filled_document : Entity
    {
        public int tbl_Org_PlanID { get; set; }
        public int tbl_Org_Plan_ResourceID { get; set; }
        public Nullable<int> tbl_Org_Plan_DocumentID { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<int> Uploaded_Doc { get; set; }
        public string Contents { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<DateTime> UploadedDate { get; set; }

        [ForeignKey("tbl_Org_Plan_ResourceID")]
        public tbl_org_plan_resource tbl_org_plan_resource { get; set; }
        [ForeignKey("tbl_Org_Plan_DocumentID")]
        public tbl_org_plan_document tbl_org_plan_document { get; set; }
    }
}

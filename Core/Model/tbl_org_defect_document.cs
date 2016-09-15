using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_defect_document : Entity
    {
        public int tbl_Org_DefectID { get; set; }
        public string DocumentName { get; set; }
        public int tbl_Docmgr_DocumentID { get; set; }
        public string Comments { get; set; }

        [ForeignKey("tbl_Org_DefectID")]
        public tbl_org_defect tbl_org_defect { get; set; }
        [ForeignKey("tbl_Docmgr_DocumentID")]
        public tbl_docmgr_document tbl_docmgr_document { get; set; }
    }
}

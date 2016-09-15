using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.Data;
using System.Web.UI;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class DefectDocumentController : Cruder<tbl_org_defect_document, tbl_org_defect_documentInput>
    {
        public DefectDocumentController(ICrudService<tbl_org_defect_document> service, IMapper<tbl_org_defect_document, tbl_org_defect_documentInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFCTDOC")
        {
            functionID = "DFCTDOC";
        }
        //
        // GET: /DefectDocument/

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_org_defect_document o) { return o.DocumentName; }

        protected override bool checkForDuplication(tbl_org_defect_documentInput input)
        {
            var entity = service.Where(rec => rec.DocumentName.Trim().Equals(input.DocumentName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_defect_documentInput input)
        {
            var entity = service.Where(rec => rec.ID != input.ID && rec.DocumentName.Trim().Equals(input.DocumentName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

    }
}

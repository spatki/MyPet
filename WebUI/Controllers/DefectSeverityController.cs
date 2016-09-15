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
    public class DefectSeverityController : Cruder<mstr_org_defect_severity, mstr_org_defect_severityInput>
    {
        public DefectSeverityController(ICrudService<mstr_org_defect_severity> service, IMapper<mstr_org_defect_severity, mstr_org_defect_severityInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSMTRDFSV")
        {
            functionID = "DFPRSMTRDFSV";
        }
        //
        // GET: /DefectSeverity/

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_org_defect_severity o) { return o.Severity; }

        protected override bool checkForDuplication(mstr_org_defect_severityInput input)
        {
            var entity = service.Where(rec => rec.Severity.Trim().Equals(input.Severity.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_defect_severityInput input)
        {
            var entity = service.Where(rec => rec.ID != input.ID && rec.Severity.Trim().Equals(input.Severity.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

    }
}

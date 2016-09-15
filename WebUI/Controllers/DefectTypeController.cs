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
    public class DefectTypeController : Cruder<mstr_org_defect_type, mstr_org_defect_typeInput>
    {
        public DefectTypeController(ICrudService<mstr_org_defect_type> service, IMapper<mstr_org_defect_type, mstr_org_defect_typeInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSMTRDFTYP")
        {
            functionID = "DFPRSMTRDFTYP";
        }
        //
        // GET: /DefectType/

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_org_defect_type o) { return o.Type; }

        protected override bool checkForDuplication(mstr_org_defect_typeInput input)
        {
            var entity = service.Where(rec => rec.Type.Trim().Equals(input.Type.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_defect_typeInput input)
        {
            var entity = service.Where(rec => rec.ID != input.ID && rec.Type.Trim().Equals(input.Type.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

    }
}

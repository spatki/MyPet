using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using System.Linq;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class OrgProjPhaseController : Cruder<mstr_org_proj_phase, mstr_org_proj_phaseInput>
    {
        public OrgProjPhaseController(ICrudService<mstr_org_proj_phase> service, IMapper<mstr_org_proj_phase, mstr_org_proj_phaseInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGPRJPH")
        {
            functionID = "DFORGPRJPH";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_org_proj_phase o) { return o.LongName; }

        protected override bool checkForDuplication(mstr_org_proj_phaseInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_proj_phaseInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override IOrderedEnumerable<mstr_org_proj_phase> orderList(IEnumerable<mstr_org_proj_phase> list) { return list.OrderBy(o => o.SequenceNo); }

        protected override void InitiazeSequence(mstr_org_proj_phaseInput input)
        {
            var seq = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (seq.Any()) { input.SequenceNo = seq.Max(o => o.SequenceNo); input.SequenceNo++; } else { input.SequenceNo = 1; }
        }

    }
}

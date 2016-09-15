using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class OrgDesignationController : Cruder<mstr_org_designation, mstr_org_designationInput>
    {
        public OrgDesignationController(ICrudService<mstr_org_designation> service, IMapper<mstr_org_designation, mstr_org_designationInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGMTRDSG")
        {
            functionID = "DFORGMTRDSG";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName (mstr_org_designation o)
        {
            return (o.LongName == null) ? "" : o.LongName; 
        }

        protected override bool checkForDuplication(mstr_org_designationInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_designationInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

    }
}

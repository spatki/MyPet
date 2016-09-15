using System.Web.Http;
using System.Linq;
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
    public class ProcessRoleController : Cruder<mstr_process_role, mstr_process_roleInput>
    {
        public ProcessRoleController(ICrudService<mstr_process_role> service, IMapper<mstr_process_role, mstr_process_roleInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSRL")
        {
            functionID = "DFPRSRL";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_process_role o)
        {
            return (o.LongName == null) ? "" : o.LongName;
        }

        protected override bool checkForDuplication(mstr_process_roleInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_process_roleInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }
    }
}

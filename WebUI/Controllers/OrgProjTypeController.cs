using System.Collections.Generic;
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
    public class OrgProjTypeController : Cruder<mstr_org_project_type, mstr_org_project_typeInput>
    {
        public OrgProjTypeController(ICrudService<mstr_org_project_type> service, IMapper<mstr_org_project_type, mstr_org_project_typeInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGPRJTYP")
        {
            functionID = "DFORGPRJTYP";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_org_project_type o) { return o.LongName; }

        public ActionResult getConfiguration()
        {
            var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (list.Any())
            {
                var ctx = service.getRepo().getDBContext();
                foreach (var t in list)
                {
                    ctx.Entry(t).Collection(o => o.mstr_org_phase_in_proj).Load();
                    if (t.mstr_org_phase_in_proj.Any())
                    {
                        foreach (var Model in t.mstr_org_phase_in_proj)
                        {
                            ctx.Entry(Model).Reference(d => d.mstr_org_proj_phase).Load();
                        }
                    }
                }
            }
            return PartialView(list);
        }

        public ActionResult getConfigForMapping()
        {
            var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (list.Any())
            {
                var ctx = service.getRepo().getDBContext();
                foreach (var t in list)
                {
                    ctx.Entry(t).Collection(o => o.mstr_org_phase_in_proj).Load();
                    if (t.mstr_org_phase_in_proj.Any())
                    {
                        foreach (var Model in t.mstr_org_phase_in_proj)
                        {
                            ctx.Entry(Model).Reference(d => d.mstr_org_proj_phase).Load();
                        }
                    }
                }
            }
            return PartialView(list);
        }

        protected override bool checkForDuplication(mstr_org_project_typeInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_project_typeInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

    }
}

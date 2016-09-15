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
    public class OrgFunctionController : Cruder<mstr_org_function, mstr_org_functionInput>
    {
        public OrgFunctionController(ICrudService<mstr_org_function> service, IMapper<mstr_org_function, mstr_org_functionInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGFN")
        {
            functionID = "DFORGFN";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_org_function o) { return o.LongName; }

        public ActionResult getConfiguration()
        {
            var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (list.Any())
            {
                var ctx = service.getRepo().getDBContext();
                foreach (var t in list)
                {
                    ctx.Entry(t).Collection(o => o.mstr_org_sub_in_function).Load();
                    if (t.mstr_org_sub_in_function.Any())
                    {
                        foreach (var Model in t.mstr_org_sub_in_function)
                        {
                            ctx.Entry(Model).Reference(d => d.mstr_org_sub_function).Load();
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
                    ctx.Entry(t).Collection(o => o.mstr_org_sub_in_function).Load();
                    if (t.mstr_org_sub_in_function.Any())
                    {
                        foreach (var Model in t.mstr_org_sub_in_function)
                        {
                            ctx.Entry(Model).Reference(d => d.mstr_org_sub_function).Load();
                        }
                    }
                }
            }
            return PartialView(list);
        }

        protected override bool checkForDuplication(mstr_org_functionInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_functionInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

    }
}

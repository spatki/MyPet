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
using WebMatrix.WebData;
using System.Transactions;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class ClientController : Cruder<mstr_client, mstr_clientInput>
    {
        public ClientController(ICrudService<mstr_client> service, IMapper<mstr_client, mstr_clientInput> v, IWorkflowService wf)
            : base(service, v, wf, "SYSCL")
        {
            functionID = "SYSCL";
        }

        [HttpPost]
        public override ActionResult Create(mstr_clientInput input)
        {
            try
            {
                ViewBag.FunctionID = functionID;
                if (!CheckAccess("Create"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, null, null, user.IsAdmin(), null, null, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View("Create", input);
                }
                if (checkForDuplication(input))
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", "Duplicate Entry Found");
                    return View("Create", input);
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    var ctx = (Db)service.getRepo().getDBContext();
                    ReSequenceBeforeCreate(input);
                    var entity = createMapper.MapToEntity(input, new mstr_client());
                    entity.ClientID = ((PAIdentity)User.Identity).clientID;
                    var id = service.Create(entity);
                    var otherData = ctx.InitializeDataForNewClient(id);
                    if (otherData == 0) 
                    {
                        return Content("Application Error. Client could not be created. Contact the system administrator");
                    }
                    var e = service.Get(id);
                    LoadDependencies(e);
                    if (input.followWF.GetValueOrDefault())
                    {
                        wrkflw.saveFlow(e.ID, WebSecurity.CurrentUserId, WebSecurity.CurrentUserId, functionID, ((PAIdentity)User.Identity).clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(), (bool?)true, "");
                    }
                    // Get changed workflow information
                    wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(e), e.ID, user.clientID.GetValueOrDefault());
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                    scope.Complete();
                    //return Json(new { Content = this.RenderView(RowViewName, new[] { e }) });
                    return ProcessView(new[] { e });
                }
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }
        //
        // GET: /Client/

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_client o) { return o.ClientName; }

        protected override bool checkForDuplication(mstr_clientInput input)
        {
            var entity = service.Where(rec => rec.ClientName.Trim().Equals(input.ClientName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_clientInput input)
        {
            var entity = service.Where(rec => rec.ID != input.ID && rec.ClientName.Trim().Equals(input.ClientName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }
    }
}

using System.Web.Mvc;
using System.Linq;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Data;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using System.Collections.Generic;
using System.Transactions;
using WebMatrix.WebData;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class ProjHardwarePlanController : Cruder<tbl_org_resourceplan_hardware, tbl_org_resourceplan_hardwareInput>
    {
        protected string returnViewName;
        //
        public ProjHardwarePlanController(ICrudService<tbl_org_resourceplan_hardware> service, IMapper<tbl_org_resourceplan_hardware, tbl_org_resourceplan_hardwareInput> v, IWorkflowService wf, IDocumentManager dm)
            : base(service, v, wf, "PLRPNHR")
        {
            functionID = "PLRPNHR";
            returnViewName = "GetPlan";
        }

        protected override string RowViewName
        {
            get { return returnViewName; }
        }
        
        public override ActionResult Index()
        {
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            return View();
        }

        [HttpPost]
        public override ActionResult Create(tbl_org_resourceplan_hardwareInput input)
        {
            returnViewName = "Edit";
            ViewBag.Mode = "Add";
            return base.Create(input);
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_resourceplan_hardwareInput input)
        {
            returnViewName = "Edit";
            return base.Edit(input);
        }

        public override ActionResult Create()
        {
            if (Request.QueryString["value"] == null || Request.QueryString["text"] == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project before invoking this action";
                return View("ListItems/showError");
            }

            if (CheckAccess("Create"))
            {
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, null, null, user.IsAdmin(), null, null, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }
                var input = new tbl_org_resourceplan_hardwareInput();
                input.tbl_Org_ProjectID = int.Parse(Request.QueryString["value"]);
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                ViewBag.ProjectName = Request.QueryString["text"];
                return View(input);
            }
            else
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
        }

        public ActionResult GetPlan(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Select a project and try again.";
                return View("ListItems/showError");
            }
            ViewBag.ProjectID = id;
            var ctx = (Db)service.getRepo().getDBContext();

            var entity = ctx.tbl_org_resourceplan_hardware.Include("mstr_process_lc_status").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id);

            return View(entity);
        }

        protected override void LoadDependencies(tbl_org_resourceplan_hardware e)
        {
            var ctx = service.getRepo().getDBContext();
            ctx.Entry(e).Reference(o => o.mstr_process_lc_status).Load();
        }

        public override ActionResult ProcessView(IEnumerable<tbl_org_resourceplan_hardware> entity)
        {
            if (RowViewName == "Edit")
            {
                return View("Edit", editMapper.MapToInput(entity.FirstOrDefault()));
            }
            return View(RowViewName, entity);
        }

        protected override int status(tbl_org_resourceplan_hardware o) { return o.mstr_Process_LC_StatusID; }

        protected override string ReviewItemName(tbl_org_resourceplan_hardware e) { return "Hardware Plan"; }   // Name of the entity being reviewed. This is used by the workflow

        protected override string UpdateChangesTo() { return "openDialogBox"; }

        protected override void setStatus(tbl_org_resourceplan_hardware e, int status)
        {
            e.mstr_Process_LC_StatusID = status;
            returnViewName = "Edit";
        }

        protected override int StatusUserID(tbl_org_resourceplan_hardware e) { return e.CreatedBy.GetValueOrDefault(); }    // The userid of the last user who has worked on this entity
    }
}

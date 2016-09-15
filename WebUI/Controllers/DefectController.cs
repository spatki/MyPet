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
    [InitializeSimpleMembership]
    public class DefectController : Cruder<tbl_org_defect, tbl_org_defectInput>
    {
        protected string returnViewName;

        public DefectController(ICrudService<tbl_org_defect> service, IMapper<tbl_org_defect, tbl_org_defectInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLDFT")
        {
            functionID = "PLDFT";
            returnViewName = "Edit";
        }
        //
        // GET: /Defect/

        public override ActionResult Index()
        {
            var user = (PAIdentity)User.Identity;
            var selectedIDs = "";
            if (!user.IsAdmin())
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var userDetails = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
                if (userDetails == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "User does not exist";
                    return View("ListItems/showError");
                }
                if (userDetails.EmployeeID == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "User is not an employee";
                    return View("ListItems/showError");
                }
                var projIDs = ctx.AccessibleProjects(userDetails.EmployeeID.GetValueOrDefault(), user.role);
                selectedIDs = "0";
                foreach (var prj in projIDs)
                {
                    selectedIDs = selectedIDs + "," + prj.ToString();
                }
            }
            ViewBag.SelectedIDs = selectedIDs;
            return View();
        }

        protected override string RowViewName
        {
            get { return returnViewName; }
        }

        protected override string listDisplayName(tbl_org_defect o) { return o.Short_Description; }

        public override ActionResult GetItems()
        {
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            var ctx = (Db) service.getRepo().getDBContext();
            var list = ctx.tbl_org_defect.Include("mstr_org_defect_type")
                                         .Include("mstr_org_defect_severity")
                                         .Include("mstr_org_proj_phase")
                                         .Include("tbl_org_proj_group")
                                         .Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        public ActionResult GetItemsFor(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            bool admin = false;
            IQueryable<tbl_org_defect> entity;
            // check whether the current user is a defect administrator
            if (((PAIdentity)User.Identity).IsAdmin())
            {
                admin = true;
            }
            else
            {
                var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
                var alloc = ctx.tbl_org_proj_allocation.Where(o => o.tbl_Org_EmployeeID == user.EmployeeID && o.ActualEndDate == null && (o.DefectAdmin != null && o.DefectAdmin == true));
                if (alloc.Any())
                {
                    admin = true;
                }
            }
            if (admin)
            {
                entity = ctx.tbl_org_defect.Include("mstr_org_defect_severity")
                                               .Include("mstr_org_defect_type")
                                               .Include("mstr_process_lc_status")
                                               .Include("mstr_org_proj_phase")
                                               .Include("tbl_org_proj_group")
                                               .Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id);
            }
            else
            {
                var wfStates = ctx.tbl_workflow_state_history.Where(o => o.FunctionID == functionID && o.UserID == WebSecurity.CurrentUserId).Select(k => k.RefID);
                entity = ctx.tbl_org_defect.Include("mstr_org_defect_severity")
                                               .Include("mstr_org_defect_type")
                                               .Include("mstr_process_lc_status")
                                               .Include("mstr_org_proj_phase")
                                               .Include("tbl_org_proj_group")
                                               .Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id && (o.CreatedBy == WebSecurity.CurrentUserId || wfStates.Contains(o.ID)));
            }
            return View("GetItems", entity);
        }

        public override ActionResult ReloadItems()
        {
            int filter = 0;

            var ctx = (Db)service.getRepo().getDBContext();

            if (Request.QueryString["filter"] != null && Request.QueryString["filter"] != "") filter = int.Parse(Request.QueryString["filter"]);

            bool admin = false;
            IEnumerable<tbl_org_defect> entity;
            // check whether the current user is a defect administrator
            if (((PAIdentity)User.Identity).IsAdmin())
            {
                admin = true;
            }
            else
            {
                var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
                var alloc = ctx.tbl_org_proj_allocation.Where(o => o.tbl_Org_EmployeeID == user.EmployeeID && o.ActualEndDate == null && (o.DefectAdmin != null && o.DefectAdmin == true));
                if (alloc.Any())
                {
                    admin = true;
                }
            }
            if (admin)
            {
                entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == filter);
            }
            else
            {
                var wfStates = ctx.tbl_workflow_state_history.Where(o => o.FunctionID == functionID && o.UserID == WebSecurity.CurrentUserId).Select(k => k.RefID);
                entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == filter && (o.CreatedBy == WebSecurity.CurrentUserId || wfStates.Contains(o.ID)));
            }
            foreach (var l in entity)
            {
                LoadDependencies(l);
            }
            return PartialView(entity);
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
                var input = new tbl_org_defectInput();
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

        [HttpPost]
        public override ActionResult Create(tbl_org_defectInput input)
        {
            returnViewName = "Edit";
            ViewBag.Mode = "Add";
            return base.Create(input);
        }

        public override ActionResult Edit(int id)
        {
            try
            {
                //var entity = new TEntity();
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                LoadDependencies(entity);
                // Check for any workflow 
                var ctx = (Db)service.getRepo().getDBContext();
                var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
                if (user == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "The user is not a valid user";
                    return View("ListItems/showError");
                }
                var adminAccess = user.IsAdministrator;
                // Check whether the current user is a defect administrator
                var alloc = ctx.tbl_org_proj_allocation.Where(o => o.tbl_Org_ProjID == entity.tbl_Org_ProjectID && o.tbl_Org_EmployeeID == user.EmployeeID && o.ActualEndDate == null).SingleOrDefault();
                if (alloc != null && alloc.DefectAdmin == true)
                {
                    adminAccess = true;
                }
                var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, ((PAIdentity)User.Identity).role, adminAccess, status(entity), entity.ID, ((PAIdentity)User.Identity).clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }
                ViewBag.Mode = "Edit";
                return View("Edit", editMapper.MapToInput(entity));
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_defectInput input)
        {
            try
            {
                returnViewName = "Edit";
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                var entity = service.Get(input.ID);
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(entity), entity.ID, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View(input);
                }
                if (checkForDuplicateEdit(input))
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", "Duplicate Entry Found");
                    return View(input);
                }
                ReSequenceBeforeEdit(input);
                var e = editMapper.MapToEntity(input, entity);
                using (TransactionScope scope = new TransactionScope())
                {
                    e.ClientID = ((PAIdentity)User.Identity).clientID;
                    var ctx = (Db)service.getRepo().getDBContext();
                    // Get the action type
                    var action_impl = ctx.tbl_workflow_implementation.Where(o => o.ID == input.StatusType).SingleOrDefault();
                    if (action_impl == null)
                    {
                        scope.Dispose();
                        Response.StatusCode = 500;
                        return View(input);
                    }
                    // Custom handling for Defects
                    if (action_impl.IsAssigned)
                    {
                        entity.AssignedOn = System.DateTime.Now.Date;
                        entity.AssignedTo = input.workflowUser;
                    }
                    else {
                        if (action_impl.IsResolved)
                        {
                            entity.FixedOn = System.DateTime.Now.Date;
                        }
                        else
                        {
                            if (action_impl.IsReviewAccepted)
                            {
                                entity.VerifiedOn = System.DateTime.Now.Date;
                                entity.VerifiedBy = input.workflowUser;
                            }
                        }
                    }
                    service.Save();
                    LoadDependencies(e);
                    if (input.followWF.GetValueOrDefault())
                    {
                        wrkflw.saveFlow(e.ID, WebSecurity.CurrentUserId, input.workflowUser, functionID, user.clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(), input.workflow, "");
                        wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(e), e.ID, user.clientID.GetValueOrDefault());
                        ViewBag.WF = true;
                        ViewBag.workflow = wf;
                    }
                    // Reload workflow based on changes
                    scope.Complete();
                }
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                ViewBag.Mode = "Edit";
                return ProcessView(new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }


        protected override void LoadDependencies(tbl_org_defect e)
        {
            var ctx = service.getRepo().getDBContext();
            ctx.Entry(e).Reference(o => o.mstr_org_defect_type).Load();
            ctx.Entry(e).Reference(o => o.mstr_org_defect_severity).Load();
            ctx.Entry(e).Reference(o => o.mstr_org_proj_phase).Load();
            ctx.Entry(e).Reference(o => o.tbl_org_proj_group).Load();
            ctx.Entry(e).Reference(o => o.mstr_process_lc_status).Load();
            ctx.Entry(e).Reference(o => o.AssignedToUser).Load();
            ctx.Entry(e).Reference(o => o.VerifiedByUser).Load();
        }

        protected override int status(tbl_org_defect o) { return o.mstr_Process_LC_StatusID; }

        public override ActionResult ProcessView(IEnumerable<tbl_org_defect> entity)
        {
            if (RowViewName == "Edit")
            {
                return View("Edit", editMapper.MapToInput(entity.FirstOrDefault()));
            }
            return View(RowViewName, entity);
        }

        protected override void setStatus(tbl_org_defect e, int status) 
        {
            e.mstr_Process_LC_StatusID = status;
        }

        public ActionResult ProceedInWF_NOComments(int id, int workflowUser, int status, string message, bool? workflow, int? statusType)
        {
            try
            {
                returnViewName = "Edit";
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                var e = service.Get(id);
                using (TransactionScope scope = new TransactionScope())
                {
                    var user = ((PAIdentity)User.Identity);
                    e.AssignedTo = workflowUser;
                    e.ClientID = user.clientID;
                    e.AssignedOn = System.DateTime.Now.Date;
                    e.mstr_Process_LC_StatusID = status;
                    service.Save();
                    LoadDependencies(e);
                    // Get workflow details
                    wrkflw.saveFlow(e.ID, WebSecurity.CurrentUserId, workflowUser, functionID, user.clientID.GetValueOrDefault(), status, workflow, "Defect Assigned");
                    var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status, e.ID, user.clientID.GetValueOrDefault());
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                    // Reload workflow based on changes
                    scope.Complete();
                }
                ViewBag.Mode = "Edit";
                ViewBag.Result = true;
                return ProcessView(new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult ProceedInWF(tbl_workflow_state_historyInput input)
        {
            returnViewName = "Edit";
            Mapper<tbl_workflow_state_history, tbl_workflow_state_historyInput> histMapper = new Mapper<tbl_workflow_state_history, tbl_workflow_state_historyInput>();
            var ctx = (Db)service.getRepo().getDBContext();

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View("WorkFlowReview", input);
            }

            if (input.RefID == 0)
            {
                input.RefID = input.ID;
            }
            var entity = service.Get(input.RefID);

            using (TransactionScope scope = new TransactionScope())
            {
                // Save Workflow comments
                wrkflw.saveFlow(entity.ID, StatusUserID(entity), input.statusWF.GetValueOrDefault(), functionID, input.ClientID.GetValueOrDefault(), input.Status, input.workflow, input.ReviewComments);
                setStatus(entity, input.Status);
                switch (input.StatusType)
                {
                    case 1:     // General
                        break;
                    case 2:     // Created New
                        break;
                    case 3:     // Assigned
                        entity.AssignedOn = System.DateTime.Now.Date;
                        entity.AssignedTo = input.UserID;
                        break;
                    case 4:     // Resolved
                        entity.FixedOn = System.DateTime.Now.Date;
                        break;
                    case 5:     // Reviewed & Accepted
                        entity.VerifiedOn = System.DateTime.Now.Date;
                        entity.VerifiedBy = input.UserID;
                        break;
                    default:
                        break;
                }
                service.Save();
                scope.Complete();
            }
            LoadDependencies(entity);
            return ProcessView(new[] { entity });
        }

        protected override string UpdateChangesTo() { return "openDialogBox"; }

    }
}

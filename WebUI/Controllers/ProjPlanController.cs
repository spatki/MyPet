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
using System.Data;
using ProcessAccelerator.WebUI.BAL.BusinessRules;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class ProjPlanController : Cruder<tbl_org_proj_plan, tbl_org_proj_planInput>
    {
        //
        // GET: /PlanProj/
        protected readonly IDocumentManager docMgr;
        const string assignResource = "PRJPLNASNR";

        public ProjPlanController(ProjectPlan service, tbl_org_proj_planMapper v, IWorkflowService wf, IDocumentManager dm)
            : base(service, v, wf, "PLPLN")
        {
            docMgr = dm;
            functionID = "PLPLN";
        }
       
        protected override string RowViewName
        {
            get { return "Edit"; }
        }

        protected override bool checkForDuplication(tbl_org_proj_planInput input)
        {
            var entity = service.Where(o => ((o.tbl_Process_Rep_TaskID == null && input.tbl_Process_Rep_TaskID == null) || o.tbl_Process_Rep_TaskID == input.tbl_Process_Rep_TaskID)
                                       && ((o.tbl_Process_RepositoryID == null && input.tbl_Process_RepositoryID == null) || o.tbl_Process_RepositoryID == input.tbl_Process_RepositoryID)
                                       && o.PlanID == input.PlanID && o.PlannedStartDate == input.PlannedStartDate && o.PlannedEndDate == input.PlannedEndDate && o.PlannedDuration == input.PlannedDuration);
            if (entity.Any()) return true;
            else
                return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_proj_planInput input)
        {
            var entity = service.Where(o => ((o.tbl_Process_Rep_TaskID == null && input.tbl_Process_Rep_TaskID == null) || o.tbl_Process_Rep_TaskID == input.tbl_Process_Rep_TaskID)
                                       && ((o.tbl_Process_RepositoryID == null && input.tbl_Process_RepositoryID == null) || o.tbl_Process_RepositoryID == input.tbl_Process_RepositoryID)
                                       && o.PlanID == input.PlanID && o.PlannedStartDate == input.PlannedStartDate && o.PlannedEndDate == input.PlannedEndDate
                                       && o.PlannedDuration == input.PlannedDuration && o.ID != input.ID);
            if (entity.Any()) return true;
            else
                return false;
        }

        protected override int status(tbl_org_proj_plan o) { return o.mstr_Process_LC_StatusID; }

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

        public ActionResult addSupportingDocument(int id, int value, string text, string excludeIDs, int key)
        {
            var entity = service.Get(id);
           
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Task not found";
                return View("ListItems/showError");
            }
            planSupportingDocuments input = new planSupportingDocuments() { MapID = value, PlanID = id, excludeIDs = excludeIDs, key = key };
            return View(input);
        }

        [HttpPost]
        public ActionResult addSupportingDocument(planSupportingDocuments input)
        {
            var entity = service.Get(input.PlanID);
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Task not found";
                return View("ListItems/showError");
            }
            //Mapper<tbl_org_plan_document, tbl_org_plan_documentInput> docMapper = new Mapper<tbl_org_plan_document,tbl_org_plan_documentInput>();

            if (input.tbl_org_plan_document != null && input.tbl_org_plan_document.Any())
            {
                foreach (var doc in input.tbl_org_plan_document.ToList())
                {
                    if (!doc.Include)
                    {
                        input.tbl_org_plan_document.Remove(doc);
                    }
                }
                if (!input.tbl_org_plan_document.Any())
                {
                    // No Documents selected
                    Response.StatusCode = 500;
                    ViewBag.ErrorMessage = "No Documents were added";
                    return View("ListItems/showError");	// Return error in a dialog box
                }
                ViewBag.PlanID = input.PlanID;
                return PartialView("IncludeSupportingDocs", input.tbl_org_plan_document);
            }
            else
            {
                // No Documents selected
                Response.StatusCode = 500;
                ViewBag.ErrorMessage = "No Documents were added";
                return View("ListItems/showError");	// Return error in a dialog box
            }
        }

        public override ActionResult Create()
        {
            try
            {
                if (Request.QueryString["value"] == null || Request.QueryString["text"] == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Pl. select a project before creating a new task";
                    return View("ListItems/showError");
                }
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, null, null, user.IsAdmin(), null, null, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }
                var entity = new tbl_org_proj_plan()
                {
                    tbl_Org_ProjectID = int.Parse(Request.QueryString["value"])
                };
                var input = createMapper.MapToInput(entity);
                input.PlannedStartDate = System.DateTime.Now.Date;
                input.PlannedEndDate = System.DateTime.Now.Date;
                return View(input);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }
        
        [HttpPost]
        public override ActionResult Create(tbl_org_proj_planInput input)
        {
            try
            {
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
                List<ValidationMessage> messages  = new List<ValidationMessage>();
                // UI validations
                if (input.validate((Db)service.getRepo().getDBContext(), ((PAIdentity)User.Identity).clientID.GetValueOrDefault(),WebSecurity.CurrentUserId,out messages))
                {
                    Response.StatusCode = 412;
                    foreach (var mess in messages)
                    {
                        ModelState.AddModelError(mess.key, mess.message);
                    }
                    return View("Create", input);
                }
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                input.FunctionID = assignResource;
                tbl_org_proj_plan e; 

                using (TransactionScope scope = new TransactionScope())
                {
                    var entity = createMapper.MapToEntity(input, new tbl_org_proj_plan());
                    var id = service.Create(entity);
                    e = service.Get(id);

                    if (input.followWF.GetValueOrDefault())
                    {
                        wrkflw.saveFlow(e.ID, WebSecurity.CurrentUserId, WebSecurity.CurrentUserId, functionID, ((PAIdentity)User.Identity).clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(), (bool?)true,"");
                    }
                    scope.Complete();
                }
                return RedirectToAction("Edit", new { id = e.ID });
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_proj_planInput input)
        {
            try
            {
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }

                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), input.mstr_Process_LC_StatusID, input.ID, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }

                var ctx = (Db)service.getRepo().getDBContext();
                var selectedTask = ctx.tbl_org_project_process_mapping.Where(o => o.ID == input.tbl_Mapped_Proj_ProcessID).SingleOrDefault();
                if (selectedTask == null)
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("tbl_Mapped_Proj_ProcessID", "Task mapping not found");
                    return View("Edit", input);
                }
                input.tbl_Process_RepositoryID = selectedTask.tbl_Process_RepositoryID;
                input.tbl_Process_Rep_TaskID = selectedTask.tbl_Process_TaskID;
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                input.CreateDate = System.DateTime.Now;
                input.CreatedBy = WebSecurity.CurrentUserId;
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View("Edit", input);
                }
                if (CheckBusinessValidations(input))
                {
                    return View("Edit", input);
                }
                tbl_org_proj_plan entity;

                using (TransactionScope scope = new TransactionScope())
                {
                    var newID = 1;
                    var e = service.Get(input.ID);
                    e.ClientID = ((PAIdentity)User.Identity).clientID;
                    if (e.tbl_org_plan_document == null) 
                    {
                        e.tbl_org_plan_document = new List<tbl_org_plan_document>();
                    }
                    else
                    {
                        if (e.tbl_org_plan_document.Any())
                        {
                            newID = e.tbl_org_plan_document.Max(o => o.ID) + 1;
                        }
                    }
                    if (input.tbl_org_plan_document != null && input.tbl_org_plan_document.Any())
                    {
                        var planMapper = new Mapper<tbl_org_plan_document,tbl_org_plan_documentInput>();
                        foreach(var doc in input.tbl_org_plan_document)
                        {
                            var selectedDoc = e.tbl_org_plan_document.Where(o => o.DocType == doc.DocType && 
                                ((o.tbl_DocMgr_DocumentID == null && doc.tbl_DocMgr_DocumentID == null) || o.tbl_DocMgr_DocumentID == doc.tbl_DocMgr_DocumentID) &&
                                ((o.tbl_Process_ProcedureID == null && doc.tbl_Process_ProcedureID == null) || o.tbl_Process_ProcedureID == doc.tbl_Process_ProcedureID) &&
                                ((o.tbl_Process_TemplateID == null && doc.tbl_Process_TemplateID == null) || o.tbl_Process_TemplateID == doc.tbl_Process_TemplateID) &&
                                ((o.tbl_Process_ChecklistID == null && doc.tbl_Process_ChecklistID == null) || o.tbl_Process_ChecklistID == doc.tbl_Process_ChecklistID)).FirstOrDefault();
                            if (selectedDoc == null)
                            {
                                doc.ID = newID;
                                doc.ClientID = ((PAIdentity)User.Identity).clientID;
                                e.tbl_org_plan_document.Add(planMapper.MapToEntity(doc,new tbl_org_plan_document()));
                                newID++;
                            }
                            else
                            {
                                doc.ID = selectedDoc.ID;
                            }
                        }
                        if (e.tbl_org_plan_document.Any())
                        {
                            foreach (var doc in e.tbl_org_plan_document.ToList())
                            {
                                if (!input.tbl_org_plan_document.Where(o => o.ID == doc.ID).Any())
                                {
                                    e.tbl_org_plan_document.Remove(doc);
                                    ctx.Entry(doc).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                        }
                    }
                    else
                    {
                        // No Documents selected, hence delete all existing documents
                        foreach (var doc in e.tbl_org_plan_document.ToList())
                        {
                            e.tbl_org_plan_document.Remove(doc);
                            ctx.Entry(doc).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                    entity = editMapper.MapToEntity(input, e);
                    service.Save();
                    if (input.followWF.GetValueOrDefault())
                    {
                        wrkflw.saveFlow(entity.ID, WebSecurity.CurrentUserId, WebSecurity.CurrentUserId, functionID, ((PAIdentity)User.Identity).clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(), (bool?)true,"");
                    }
                    scope.Complete();
                }
                ViewBag.Mode = "Edit";
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult FilterProjPlan(ProjectPlanFilter input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project and try again.";
                return View("ListItems/showError");
            }
            ViewBag.ProjectID = input.SearchProjectID;
            var ctx = (Db)service.getRepo().getDBContext();

            var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.SearchProjectID &&
                                                           (input.SearchFromDate == null || (input.SearchFromDate != null && o.PlannedStartDate >= input.SearchFromDate)) &&
                                                           (input.SearchToDate == null || (input.SearchToDate != null && o.PlannedEndDate <= input.SearchToDate)) &&
                                                           (input.SearchPlanID == null || (input.SearchPlanID != null && o.PlanID == input.SearchPlanID)) &&
                                                           (input.SearchGroupID == null || (input.SearchGroupID != null && o.tbl_Org_Proj_GroupID == input.SearchGroupID)) &&
                                                           (input.SearchStatusID == null || (input.SearchStatusID != null && o.mstr_Process_LC_StatusID == input.SearchStatusID)));
            if (entity != null && entity.Any())
            {
                foreach (var e in entity)
                {
                    ctx.Entry(e).Reference(o => o.mstr_process_lc_status).Load();
                    ctx.Entry(e).Reference(o => o.mstr_org_proj_phase).Load();
                    ctx.Entry(e).Reference(o => o.tbl_org_proj_group).Load();
                    ctx.Entry(e).Collection(o => o.tbl_org_plan_resource).Load();
                    if (e.tbl_org_plan_resource.Any())
                    {
                        foreach (var emp in e.tbl_org_plan_resource)
                        {
                            ctx.Entry(emp).Reference(k => k.tbl_org_employee).Load();
                        }
                    }
                }
            }
            return View("GetPlannedTasks",entity);
        }

        public ActionResult getTaskWiseTracking(int projectID, System.DateTime reportingDate)
        {
            try
            {
                var startDate = reportingDate;
                var endDate = reportingDate.AddDays(6);
                var ctx = (Db)service.getRepo().getDBContext();

                var tasks = ctx.tbl_org_proj_plan.Include("mstr_process_lc_status").Include("tbl_org_plan_resource").Include("tbl_org_plan_resource.tbl_org_employee").Include("tbl_org_plan_resource.mstr_process_lc_status").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                               o.PlannedStartDate <= endDate && o.PlannedEndDate >= startDate);
                ViewBag.reportingDate = reportingDate;
                if (tasks.Any())
                {
                    return View(tasks);
                }
                else
                {
                    return new ContentResult() { Content = "<tr><td colspan='8'>No Tasks Allocated for this duration</td></tr>" };
                }
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult getResourceWiseTracking(int projectID, System.DateTime reportingDate)
        {
            try
            {
                var startDate = reportingDate;
                var endDate = reportingDate.AddDays(6);
                List<resourceWiseTasks> reportOutput = new List<resourceWiseTasks>();
                resourceWiseTaskMapper reportMapper = new resourceWiseTaskMapper();

                var ctx = (Db)service.getRepo().getDBContext();

                var resources = ctx.tbl_org_proj_allocation.Include("tbl_org_employee").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjID == projectID &&
                                                                    o.ActualEndDate == null &&
                                                                    o.PlannedStartDate <= endDate && o.PlannedEndDate >= startDate).OrderBy(o => o.ID);
                ViewBag.reportingDate = reportingDate;
                if (resources.Any())
                {
                    foreach (var r in resources)
                    {
                        // Get allocation data
                        var reportInstance = reportMapper.MapToInput(r);
                        reportInstance.tbl_org_plan_resource = ctx.tbl_org_plan_resource.Include("tbl_org_proj_plan").Include("mstr_process_lc_status").Where(o => o.ClientID == r.ClientID &&
                                                                                                 o.tbl_org_proj_plan.tbl_Org_ProjectID == r.tbl_Org_ProjID &&
                                                                                                 o.tbl_Org_EmployeeID == r.tbl_Org_EmployeeID &&
                                                                                                 o.PlannedStart <= endDate && o.PlannedEnd >= startDate);
                        reportInstance.PlannedDuration = reportInstance.tbl_org_plan_resource.Sum(o => o.PlannedDuration).GetValueOrDefault();
                        reportInstance.ActualDuration = reportInstance.tbl_org_plan_resource.Sum(o => o.ActualDuration).GetValueOrDefault();
                        reportInstance.ActualPercentAllocation = reportInstance.tbl_org_plan_resource.Sum(o => o.AllocationPercent);
                        reportOutput.Add(reportInstance);
                    }
                    return View(reportOutput);
                }
                else
                {
                    return new ContentResult() { Content = "<tr><td colspan='8'>No Resources Allocated for this duration</td></tr>" };
                }
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }

        }
        

        public ActionResult GetPlannedTasks(int? Project)
        {
            if (Project == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Select a project and try again.";
                return View("ListItems/showError");
            }
            ViewBag.ProjectID = Project;
            var ctx = (Db)service.getRepo().getDBContext();

            var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == Project);
            if (entity.Any())
            {
                foreach (var e in entity)
                {
                    ctx.Entry(e).Reference(o => o.mstr_process_lc_status).Load();
                    ctx.Entry(e).Reference(o => o.mstr_org_proj_phase).Load();
                    ctx.Entry(e).Reference(o => o.tbl_org_proj_group).Load();
                    ctx.Entry(e).Collection(o => o.tbl_org_plan_resource).Load();
                    if (e.tbl_org_plan_resource.Any())
                    {
                        foreach (var emp in e.tbl_org_plan_resource)
                        {
                            ctx.Entry(emp).Reference(k => k.tbl_org_employee).Load();
                        }
                    }
                }
            }
            return View(entity);
        }

        public ActionResult addResource(int projectID, int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();

            var projResources = ctx.tbl_org_proj_allocation.Include("tbl_org_employee").Where(o => o.tbl_Org_ProjID == projectID && o.ActualEndDate == null);

            if (!projResources.Any())
            {
                Response.StatusCode = 500;
                return new ContentResult() { Content = "<td colspan=4>No Resources allocated to this project.</td>" };
            }

            ViewBag.projectID = projectID;
            ViewBag.resourceID = id;
            return View(projResources);
        }

        public ActionResult DeleteResource(int id)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var res = ctx.tbl_org_plan_resource.Where(o => o.ID == id).SingleOrDefault();
                if (res == null)
                {
                    return new ContentResult() { Content = "Resource Removed" };
                }
                ctx.tbl_org_plan_resource.Remove(res);
                ctx.SaveChanges();
                return new ContentResult() { Content = "Resource Removed" };
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteSuppDoc(int id)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var res = ctx.tbl_org_plan_document.Where(o => o.ID == id).SingleOrDefault();
                if (res == null)
                {
                    return new ContentResult() { Content = "Document Removed" };
                }
                ctx.tbl_org_plan_document.Remove(res);
                ctx.SaveChanges();
                return new ContentResult() { Content = "Document Removed" };
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult uploadDocForTask(int projectID, int planID)        // Here id represents project ID
        {
            uploaded_doc input = new uploaded_doc();
            input.tbl_Org_ProjectID = projectID;
            input.tbl_Org_PlanID = planID;
            return PartialView(input);
        }

        public bool CheckBusinessValidations(tbl_org_proj_planInput input)
        {
            bool result = false;
            bool test1 = false, test2 = false, test3 = false, test4 = false;
            decimal totalDuration = 0;

            var entity = service.Where(o => ((o.tbl_Process_Rep_TaskID == null && input.tbl_Process_Rep_TaskID == null) || o.tbl_Process_Rep_TaskID == input.tbl_Process_Rep_TaskID)
                                       && ((o.tbl_Process_RepositoryID == null && input.tbl_Process_RepositoryID == null) || o.tbl_Process_RepositoryID == input.tbl_Process_RepositoryID)
                                       && ((o.PlanID == null && input.PlanID == null) || o.PlanID == input.PlanID)
                                       && ((o.tbl_Org_Proj_GroupID == null && input.tbl_Org_Proj_GroupID == null) || o.tbl_Org_Proj_GroupID == input.tbl_Org_Proj_GroupID)
                                       && o.PlannedStartDate == input.PlannedStartDate && o.PlannedEndDate == input.PlannedEndDate
                                       && o.PlannedDuration == input.PlannedDuration && o.ID != input.ID);
            if (entity.Any()) {
                result = true;
                Response.StatusCode = 412;
                ModelState.AddModelError("", "Duplicate Entry Found");
            }
            if (input.PlannedEndDate < input.PlannedStartDate)
            {
                result = true;
                Response.StatusCode = 412;
                ModelState.AddModelError("", "Task start date and end dates cannot be out of sequence");
            }

            var duplicateResources = from r in input.tbl_org_plan_resource
                 group r by r.tbl_Org_EmployeeID into grp
                 where grp.Count() > 1
                 select new { key = grp.Key, cnt = grp.Count()};
            if (duplicateResources.Count() > 0)
            {
                foreach (var emp in duplicateResources)
                {
                    ModelState.AddModelError("", "Resource has been allocated more than once");
                    result = true;
                    Response.StatusCode = 412;
                }
            }
            if (input.tbl_org_plan_resource != null && input.tbl_org_plan_resource.Any())
            {
                foreach (var r in input.tbl_org_plan_resource)
                {
                    if (r.PlannedStart < r.AllocationStart || r.PlannedStart > r.AllocationEnd)
                    {
                        result = true;
                        Response.StatusCode = 412;
                        ModelState.AddModelError("tbl_org_plan_resource[" + r.ID + "].PlannedStart", "*");
                        if (!test1)
                        {
                            ModelState.AddModelError("", "Planned Start cannot be outside resource allocation dates");
                            test1 = true;
                        }
                    }
                    if (r.PlannedEnd < r.AllocationStart || r.PlannedEnd > r.AllocationEnd)
                    {
                        result = true;
                        Response.StatusCode = 412;
                        ModelState.AddModelError("tbl_org_plan_resource[" + r.ID + "].PlannedEnd", "*");
                        if (!test2)
                        {
                            ModelState.AddModelError("", "Planned End cannot be outside resource allocation dates");
                            test2 = true;
                        }
                    }
                    if (r.PlannedEnd < r.PlannedStart)
                    {
                        result = true;
                        Response.StatusCode = 412;
                        ModelState.AddModelError("tbl_org_plan_resource[" + r.ID + "].PlannedEnd", "*");
                        ModelState.AddModelError("tbl_org_plan_resource[" + r.ID + "].PlannedStart", "*");
                        if (!test3)
                        {
                            ModelState.AddModelError("", "Planned start and end dates cannot be out of sequence");
                            test3 = true;
                        }
                    }
                    if (r.PlannedDuration > input.PlannedDuration)
                    {
                        result = true;
                        Response.StatusCode = 412;
                        ModelState.AddModelError("tbl_org_plan_resource[" + r.ID + "].PlannedDuration", "*");
                        if (!test4)
                        {
                            ModelState.AddModelError("", "Resource duration is greater than the task duration");
                            test4 = true;
                        }
                        totalDuration += r.PlannedDuration.GetValueOrDefault();
                    }
                    if (totalDuration > input.PlannedDuration)
                    {
                        result = true;
                        Response.StatusCode = 412;
                        ModelState.AddModelError("", "Total duration of individual resources cannot be greater than the task duration");
                    }
                }
            }
            return result;
        }

        [HttpPost]
        public ActionResult uploadDocForTask(uploaded_doc input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("uploadDocForTask", input);
                }

                var pl = service.Get(input.tbl_Org_PlanID);
                if (pl == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Task does not exist";
                    return View("ListItems/showError");
                }

                var errMessage = "";
                var ctx = (Db)service.getRepo().getDBContext();
                tbl_org_plan_document entity;

                using (TransactionScope scope = new TransactionScope())
                {
                    var docID = docMgr.addDocument(input.fileName.InputStream, Server.MapPath("~"), input.fileName.FileName, ((PAIdentity)User.Identity).clientID.GetValueOrDefault(), "Project", out errMessage);
                    if (docID == 0)
                    {
                        Response.StatusCode = 412;
                        ModelState.AddModelError("", errMessage);
                        return View("uploadDocForTask", input);
                    }
                    else
                    {
                        entity = new tbl_org_plan_document()
                        {
                            ClientID = ((PAIdentity)User.Identity).clientID,
                            tbl_Org_PlanID = input.tbl_Org_PlanID,
                            tbl_DocMgr_DocumentID = docID,
                            Remarks = input.Remarks,
                            Source = 2,
                            ReferenceType = input.ReferenceType
                        };
                        ctx.tbl_org_plan_document.Add(entity);
                        ctx.SaveChanges();
                    }
                    scope.Complete();
                }
                ctx.Entry(entity).Reference(o => o.tbl_docmgr_document).Load();
                ctx.Entry(entity).Reference(o => o.tbl_process_checklist).Load();
                ctx.Entry(entity).Reference(o => o.tbl_process_procedure).Load();
                ctx.Entry(entity).Reference(o => o.tbl_process_template).Load();
                return View("GetDocuments", new[] { entity });
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Error occurred during upload. Pl. try again. " + e.Message;
                return View("ListItems/showError");
            }

        }

        public ActionResult ProjectTasksForReview(int id, System.DateTime startDate, System.DateTime endDate)
        {
            var entity = service.Where(o => o.tbl_Org_ProjectID == id && o.PlannedStartDate <= endDate && o.PlannedEndDate >= startDate);
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View(entity);
        }

        public ActionResult GetTaskTSEntry(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var ts = ctx.tbl_org_plan_resource.Include("tbl_org_employee").Where(o => o.tbl_Org_Proj_PlanID == id);
            return PartialView(ts);
        }
    }
}

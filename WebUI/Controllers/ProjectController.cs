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
    public class ProjectController : Cruder<tbl_org_project, tbl_org_projectInput>
    {
        //
        // GET: /PlanProj/
        protected readonly IDocumentManager docMgr;

        public ProjectController(ICrudService<tbl_org_project> service, IMapper<tbl_org_project, tbl_org_projectInput> v, IWorkflowService wf, IDocumentManager dm)
            : base(service, v, wf, "PLINT")
        {
            docMgr = dm;
            functionID = "PLINT";
        }

        protected override string RowViewName
        {
            get { return "Edit"; }
        }

        public override ActionResult Index()
        {
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.Menu = "PL";
            return View();
        }

        public ActionResult ProjectIndex()
        {
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            return View();
        }

        public override ActionResult GetItems()
        {
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            var ctx = (Db)service.getRepo().getDBContext();
            var list = getProjects();

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        protected IEnumerable<tbl_org_project> getProjects()
        {
            var ctx = (Db)service.getRepo().getDBContext();
            // Consider data access here
            if (((PAIdentity)User.Identity).IsAdmin())
            {
                return ctx.tbl_org_project.Include("mstr_org_project_type").Include("mstr_process_lc_status").Include("UserProfile").Include("UserProfile_StatusUser").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();

                if (user == null)
                {
                    return new List<tbl_org_project>();    // User not found
                }
                else
                {
                    if (user.EmployeeID != null)    // Proceed only if the user is an employee
                    {
                        var empID = user.EmployeeID.GetValueOrDefault();
                        var projList = ctx.AccessibleProjects(empID, ((PAIdentity)User.Identity).role).ToList();

                        return ctx.tbl_org_project.Include("mstr_org_project_type").Include("mstr_process_lc_status").Include("UserProfile").Include("UserProfile_StatusUser").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && projList.Contains(o.ID));
                    }
                }
                return new List<tbl_org_project>();    // User not found
            }
        }

        public ActionResult projectTree()
        {
            var input = getProjects();
            var ctx = service.getRepo().getDBContext();
            mstr_org_proj_phase phase;
            foreach (var typ in input)
            {
                ctx.Entry(typ.mstr_org_project_type).Collection(o => o.mstr_org_phase_in_proj).Load();
                foreach (var Model in typ.mstr_org_project_type.mstr_org_phase_in_proj)
                {
                    ctx.Entry(Model).Reference(o => o.mstr_org_proj_phase).Load();
                }
            }
            return View(input);
        }

        public ActionResult GetProjectResources(int projectID, int selectedItem, string controlName, string reload)
        {
            var ctx = (Db)service.getRepo().getDBContext();

            var res = ctx.tbl_org_proj_allocation.Include("tbl_org_employee").Where(o => o.tbl_Org_ProjID == projectID && o.ActualEndDate == null);

            ViewBag.selectedItem = selectedItem;
            ViewBag.itemName = controlName;
            ViewBag.reload = reload;
            return PartialView(res);
        }
        
        public override ActionResult Create()
        {
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
                var input = new tbl_org_projectInput();
                var ctx = (Db)service.getRepo().getDBContext();

                input.tbl_org_proj_org_level = new List<tbl_org_proj_org_levelInput>();
                var orgLevels = ctx.mstr_org_level.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
                if (orgLevels.Any())
                {
                    foreach (var ol in orgLevels)
                    {
                        input.tbl_org_proj_org_level.Add(new tbl_org_proj_org_levelInput() {
                            ID = ol.ID,
                            org_level_name = ol.LongName,
                            tbl_Org_LevelID = ol.ID
                        });
                    }
                }
                return View(input);
            }
            else
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
        }


        [HttpPost]
        public override ActionResult Create(tbl_org_projectInput input)
        {
            try
            {
                if (!CheckAccess("Create"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                var ctx = (Db)service.getRepo().getDBContext();
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                int id;
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
                    ModelState.AddModelError("Name", "Duplicate Entry Found");
                    return View("Create", input);
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    // Check if the client is new. If yes create a new client
                    input.ClientID = ((PAIdentity)User.Identity).clientID;
                    input.CreateDate = System.DateTime.Now;
                    input.CreatedBy = WebSecurity.CurrentUserId;
                    input.Status_User = WebSecurity.CurrentUserId;
                    if (input.orgClientID == null || input.orgClientID == 0)
                    {
                        // Create a new client
                        var o_client = ctx.mstr_org_client.Where(o => o.Name.ToLower().Equals(input.ClientName.Trim().ToLower()) && o.ClientID == input.ClientID);
                        if (o_client.Any())
                        {
                            ModelState.AddModelError("ClientName", "Duplicate Client Record. Select existing client from the drop down");
                            return View ("Create",input);
                        }
                        ctx.mstr_org_client.Add(new mstr_org_client()
                        {
                            ClientID = input.ClientID,
                            Name = input.ClientName,
                            Type = input.ClientType,
                            Description = input.ClientContactDetail,
                            PContactMailID = input.ClientContactPerson
                        });
                        ctx.SaveChanges();
                        var new_client = ctx.mstr_org_client.Where(o => o.Name == input.ClientName).SingleOrDefault();
                        input.orgClientID = new_client.ID;
                    }
                    else
                    {
                        var o_client = ctx.mstr_org_client.Where(o => o.ID == input.orgClientID && o.ClientID == input.ClientID).SingleOrDefault();
                        if (o_client == null)
                        {
                            ModelState.AddModelError("ClientName", "Client Not Found");
                            return View("Create", input);
                        }
                        // Save changes if any
                        input.ClientID = ((PAIdentity)User.Identity).clientID;
                        o_client.Name = input.ClientName;
                        o_client.Type = input.ClientType;
                        o_client.Description = input.ClientContactDetail;
                        o_client.PContactMailID = input.ClientContactPerson;
                        ctx.SaveChanges();
                    }

                    var entity = createMapper.MapToEntity(input, new tbl_org_project());
                    id = service.Create(entity);
                    // Save org levels
                    if (input.tbl_org_proj_org_level != null && input.tbl_org_proj_org_level.Any())
                    {
                        foreach (var ol in input.tbl_org_proj_org_level)
                        {
                            if (ol.tbl_Org_Level_MasterID != null)
                            {
                                ctx.tbl_org_proj_org_level.Add(new tbl_org_proj_org_level()
                                {
                                    tbl_Org_ProjectID = id,
                                    tbl_Org_LevelID = ol.tbl_Org_LevelID,
                                    tbl_Org_Level_MasterID = ol.tbl_Org_Level_MasterID,
                                    ClientID = input.ClientID
                                });
                            }
                        }
                    }
                    ctx.tbl_org_proj_review_history.Add(new tbl_org_proj_review_history()
                    {
                        ClientID = input.ClientID,
                        tbl_Org_ProjectID = id,
                        ReviewDate = System.DateTime.Now,
                        Comments = "",
                        UserID = WebSecurity.CurrentUserId,
                        StatusID = input.mstr_Process_LC_StatusID
                    });
                    ctx.SaveChanges();
                    // Add Process Mappings
                    service.getRepo().executeStoredCommand("exec sp_mapProjectProcess " + id);
                    if (input.followWF.GetValueOrDefault())
                    {
                        wrkflw.saveFlow(id,WebSecurity.CurrentUserId, input.workflowUser, functionID, user.clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(),input.workflow,"");
                    }
                    scope.Complete();
                }
                return RedirectToAction("Edit", new { id = id });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public override ActionResult Edit(int id)
        {
            try
            {
                //var entity = new TEntity();
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                ctx.Entry(entity).Reference(o => o.mstr_org_client).Load();
                ctx.Entry(entity).Reference(o => o.mstr_process_lc_status).Load();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_review_history).Load();
                var orgLevels = ctx.mstr_org_level.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);

                foreach (var hist in entity.tbl_org_proj_review_history)
                {
                    ctx.Entry(hist).Reference(o => o.mstr_process_lc_status).Load();
                    ctx.Entry(hist).Reference(o => o.UserProfile).Load();
                }
                
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), entity.mstr_Process_LC_StatusID, entity.ID, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }
                var input = editMapper.MapToInput(entity);
                var counter = orgLevels.Max(o => o.ID);
                input.tbl_org_proj_org_level = new List<tbl_org_proj_org_levelInput>();
                if (orgLevels.Any())
                {
                    foreach (var ol in orgLevels)
                    {
                        var projLevel = ctx.tbl_org_proj_org_level.Where(o => o.tbl_Org_ProjectID == id && o.tbl_Org_LevelID == ol.ID).SingleOrDefault();
                        if (projLevel == null)
                        {
                            counter++;
                            input.tbl_org_proj_org_level.Add(new tbl_org_proj_org_levelInput()
                            {
                                ID = counter,
                                org_level_name = ol.LongName,
                                tbl_Org_LevelID = ol.ID,
                                tbl_Org_ProjectID = id,
                                ClientID = input.ClientID,
                                NewEntry = true
                            });
                        }
                        else
                        {
                            input.tbl_org_proj_org_level.Add(new tbl_org_proj_org_levelInput()
                            {
                                ID = projLevel.ID,
                                org_level_name = ol.LongName,
                                tbl_Org_LevelID = ol.ID,
                                tbl_Org_Level_MasterID = projLevel.tbl_Org_Level_MasterID,
                                tbl_Org_ProjectID = id,
                                ClientID = input.ClientID,
                                NewEntry = false
                            });
                        }
                    }
                }
                input.ClientName = entity.mstr_org_client.Name;
                input.ClientType = entity.mstr_org_client.Type;
                input.ClientContactDetail = entity.mstr_org_client.Description;
                input.ClientContactPerson = entity.mstr_org_client.PContactMailID;
                input.projectStatus = entity.mstr_process_lc_status.Status;
                input.tbl_org_proj_review_history = entity.tbl_org_proj_review_history;
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_projectInput input)
        {
            try
            {
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
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
                using (TransactionScope scope = new TransactionScope())
                {
                    var ctx = (Db)service.getRepo().getDBContext();
                    input.ClientID = ((PAIdentity)User.Identity).clientID;
                    input.Status_User = WebSecurity.CurrentUserId;
                    if (input.orgClientID == null || input.orgClientID == 0)
                    {
                        // New client is added
                        var o_client = ctx.mstr_org_client.Where(o => o.Name.ToLower().Equals(input.ClientName.Trim().ToLower()) && o.ClientID == input.ClientID);
                        if (o_client.Any())
                        {
                            ModelState.AddModelError("ClientName", "Duplicate Client Record. Select existing client from the drop down");
                            return View("Edit", input);
                        }
                        ctx.mstr_org_client.Add(new mstr_org_client()
                        {
                            ClientID = input.ClientID,
                            Name = input.ClientName,
                            Type = input.ClientType,
                            Description = input.ClientContactDetail,
                            PContactMailID = input.ClientContactPerson
                        });
                        ctx.SaveChanges();
                        var new_client = ctx.mstr_org_client.Where(o => o.Name == input.ClientName).SingleOrDefault();
                        input.orgClientID = new_client.ID;
                    }
                    else
                    {
                        var o_client = ctx.mstr_org_client.Where(o => o.ID == input.orgClientID && o.ClientID == input.ClientID).SingleOrDefault();
                        if (o_client == null)
                        {
                            ModelState.AddModelError("ClientName", "Client Not Found");
                            return View("Edit", input);
                        }
                        // Save changes if any
                        o_client.Name = input.ClientName;
                        o_client.Type = input.ClientType;
                        o_client.Description = input.ClientContactDetail;
                        o_client.PContactMailID = input.ClientContactPerson;
                        ctx.SaveChanges();
                    }
                    var e = editMapper.MapToEntity(input, service.Get(input.ID));
                    service.Save();
                    // Save org level information
                    Mapper<tbl_org_proj_org_level, tbl_org_proj_org_levelInput> orgLevelMapper = new Mapper<tbl_org_proj_org_level, tbl_org_proj_org_levelInput>();
                    e.tbl_org_proj_org_level = new List<tbl_org_proj_org_level>();
                    foreach (var ol in input.tbl_org_proj_org_level)
                    {
                        if (ol.NewEntry)
                        {
                            if (ol.tbl_Org_Level_MasterID != null)
                            {
                                ctx.tbl_org_proj_org_level.Add(orgLevelMapper.MapToEntity(ol, new tbl_org_proj_org_level()));  // Add new
                            }
                        }
                        else
                        {
                            if (ol.tbl_Org_Level_MasterID == null)
                            {
                                ctx.tbl_org_proj_org_level.Remove(ctx.tbl_org_proj_org_level.Where(o => o.ID == ol.ID).SingleOrDefault()); // Delete Existing
                            }
                            else
                            {
                                orgLevelMapper.MapToEntity(ol, ctx.tbl_org_proj_org_level.Where(o => o.ID == ol.ID).SingleOrDefault()); // Update Existing
                            }
                        }
                    }
                    ctx.SaveChanges();
                    // Save project change history
                    ctx.tbl_org_proj_review_history.Add(new tbl_org_proj_review_history()
                    {
                        ClientID = e.ClientID,
                        tbl_Org_ProjectID = e.ID,
                        ReviewDate = System.DateTime.Now,
                        Comments = "",
                        UserID = WebSecurity.CurrentUserId,
                        StatusID = input.statusWF.GetValueOrDefault()
                    });
                    ctx.SaveChanges();
                    var user = ((PAIdentity)User.Identity);
                    if (input.followWF.GetValueOrDefault())
                    {
                        wrkflw.saveFlow(e.ID, WebSecurity.CurrentUserId, input.workflowUser, functionID, user.clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(),input.workflow,"");
                    }
                    var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(e), e.ID, user.clientID.GetValueOrDefault());
                    if (wf.Any())
                    {
                        ViewBag.WF = true;
                        ViewBag.workflow = wf;
                    }
                    ctx.Entry(e).Reference(o => o.mstr_process_lc_status).Load();
                    ctx.Entry(e).Collection(o => o.tbl_org_proj_review_history).Load();
                    foreach (var hist in e.tbl_org_proj_review_history)
                    {
                        ctx.Entry(hist).Reference(o => o.mstr_process_lc_status).Load();
                        ctx.Entry(hist).Reference(o => o.UserProfile).Load();
                    }
                    input.projectStatus = e.mstr_process_lc_status.Status;
                    input.tbl_org_proj_review_history = e.tbl_org_proj_review_history;
                    scope.Complete();
                }
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public override ActionResult ProceedInWF(int id, int workflowUser, int status, string message, bool? workflow, int? statusType)
        {
            var project = service.Get(id);

            tbl_org_proj_review_historyInput projReview;

            if (project == null) throw new PAException("Project not found");
            var ctx  = (Db) service.getRepo().getDBContext();
            ctx.Entry(project).Reference(o => o.UserProfile).Load();
            var statusText = ctx.mstr_process_lc_status.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == status).SingleOrDefault();
            var WKUser = ctx.UserProfile.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == WebSecurity.CurrentUserId).SingleOrDefault();

            projReview = new tbl_org_proj_review_historyInput()
            {
                projectName = project.Name,
                tbl_Org_ProjectID = project.ID,
                UserID = workflowUser,
                ReviewDate = System.DateTime.Now,
                userName = ((PAIdentity)User.Identity).friendlyName,
                StatusID = status,
                StatusName = statusText.Status,
                CreatedBy = project.UserProfile.DisplayName,
                Comments = "",
                workflow = workflow,
                prevUser = (WKUser == null ? null : (int?) WebSecurity.CurrentUserId)
            };
            ViewBag.message = message;
            return View(projReview);
        }

        [HttpPost]
        public ActionResult ProjectProceedInWF(tbl_org_proj_review_historyInput input)
        {
            Mapper < tbl_org_proj_review_history,tbl_org_proj_review_historyInput> histMapper = new Mapper<tbl_org_proj_review_history,tbl_org_proj_review_historyInput>();
            var ctx = (Db)service.getRepo().getDBContext();

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View("ProceedInWF",input);
            }
            tbl_org_project project = service.Get(input.tbl_Org_ProjectID);
            if (project == null) throw new PAException("Project not found");
            var user = ((PAIdentity)User.Identity);
            var statusMaster = ctx.mstr_process_lc_status.Where(o => o.ClientID == user.clientID && o.ID == input.StatusID).SingleOrDefault();
            var wfUser = true;

            if (input.UserID == 0) 
            { 
                wfUser = false; 
                input.UserID = (input.prevUser == null ? WebSecurity.CurrentUserId : input.prevUser.GetValueOrDefault()); 
            }
            using (TransactionScope scope = new TransactionScope())
            {
                if (statusMaster != null)
                {
                    if (statusMaster.IsReview == true)
                    {
                        project.ReviewedBy = input.UserID;
                        project.ReviewDate = System.DateTime.Now;
                    }
                    if (statusMaster.IsPublish == true)
                    {
                        project.ApprovedBy = input.UserID;
                        project.ApproveDate = System.DateTime.Now;
                    }
                }
                project.mstr_Process_LC_StatusID = input.StatusID;
                project.Status_User = input.UserID;
                service.Save();
                ctx.Entry(project).Reference(o => o.mstr_process_lc_status).Load();

                var e = histMapper.MapToEntity(input, new tbl_org_proj_review_history());
                e.ClientID = ((PAIdentity)User.Identity).clientID;
                ctx.tbl_org_proj_review_history.Add(e);
                ctx.SaveChanges();
                wrkflw.saveFlow(project.ID, input.prevUser.GetValueOrDefault(), (wfUser == true ? input.UserID : 0), functionID, user.clientID.GetValueOrDefault(), input.StatusID, input.workflow,input.Comments);
                scope.Complete();
            }
            ctx.Entry(project).Collection(o => o.tbl_org_proj_review_history).Load();
            foreach (var r in project.tbl_org_proj_review_history)
            {
                ctx.Entry(r).Reference(o => o.UserProfile).Load();
                ctx.Entry(r).Reference(o => o.mstr_process_lc_status).Load();
            }
            tbl_org_projectInput editProject = editMapper.MapToInput(project);

            ctx.Entry(project).Reference(o => o.mstr_org_client).Load();
            ctx.Entry(project).Collection(o => o.tbl_org_proj_org_level).Load();
            editProject.ClientName = project.mstr_org_client.Name;
            editProject.ClientType = project.mstr_org_client.Type;
            editProject.projectStatus = project.mstr_process_lc_status.Status;
            // Check for any workflow 
            var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), project.mstr_Process_LC_StatusID, project.ID, user.clientID.GetValueOrDefault());
            if (wf.Any())
            {
                ViewBag.WF = true;
                ViewBag.workflow = wf;
            }
            else
            {
                ViewBag.WF = false;
                ViewBag.workflow = null;
            }
            return View("Edit", editProject);
        }

        public ActionResult ProjDocuments()
        {
            var user = (PAIdentity)User.Identity;
            ViewBag.FunctionID = "PLDOC";
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

        public ActionResult ProjGroups()
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

        public ActionResult OrgLevelMapping(int? value, string text)
        {
            if (value == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project for mapping";
                return View("ListItems/showError");
            }
            var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == value).SingleOrDefault();
            if (entity == null)
            {
                Response.StatusCode = 500;
                return Content("Project not found");
            }
            ProjOrgLevelMapping input = new ProjOrgLevelMapping();
            input.ID = value.GetValueOrDefault();
            input.ProjectName = entity.Name;
            input.LevelID = entity.LocationOrgLevelID.GetValueOrDefault();
            return View(input);
        }

        [HttpPost]
        public ActionResult OrgLevelMapping(ProjOrgLevelMapping input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View(input);
            }
            var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == input.ID).SingleOrDefault();
            if (entity == null)
            {
                Response.StatusCode = 500;
                return Content("Project not found");
            }
            entity.LocationOrgLevelID = (int?)input.LevelID;
            service.Save();
            return Content(input.ID.ToString());
        }


        public ActionResult GetGroups(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var groups = ctx.tbl_org_proj_group.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id);
            ViewBag.ProjectID = id;
            return View(groups);
        }

        public ActionResult GetDocuments(int id)
        {
            if (id == 0) return null;
            var ctx = (Db)service.getRepo().getDBContext();

            var docs = ctx.tbl_org_project_documents.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id && o.SourceType == 1);
            if (docs.Any())
            {
                foreach (var d in docs)
                {
                    ctx.Entry(d).Reference(o => o.mstr_process_doc_type).Load();
                    ctx.Entry(d).Reference(o => o.tbl_docmgr_document).Load();
                }
            }
            ViewBag.ProjectID = id;
            return View(docs);
        }

        public ActionResult GetProcessDocuments(int id)
        {
            if (id == 0) return null;
            var ctx = (Db)service.getRepo().getDBContext();

            var docs = ctx.vw_process_documents.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id);
            ViewBag.ProjectID = id;
            return View(docs);
        }

        public ActionResult AddProjGroup(int projID, short level, int parent)
        {
            if (projID == 0)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project before invoking this action";
                return View("ListItems/showError");
            }
            var input = new tbl_org_proj_groupInput();
            input.tbl_Org_ProjectID = projID;
            input.Level = level;
            input.IsParent = false;
            if (parent > 0) input.Parent_GroupID = parent;
            return View(input);
        }

        [HttpPost]
        public ActionResult AddProjGroup(tbl_org_proj_groupInput input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 500;
                return View("AddProjGroup", input);
            }
            var ctx = (Db)service.getRepo().getDBContext();
            var proj = ctx.tbl_org_project.Include("tbl_org_proj_group").Where(o => o.ID == input.tbl_Org_ProjectID).SingleOrDefault();

            if (proj == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "This project is not found in the system.";
                return View("ListItems/showError");
            }
            int newGroupID = 1;
            if (proj.tbl_org_proj_group.Any())
            {
                newGroupID = proj.tbl_org_proj_group.Max(o => o.ID);
            }
            var checkDuplicate = ctx.tbl_org_proj_group.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.Name == input.Name);
            if (checkDuplicate.Any())
            {
                ModelState.AddModelError("Name", "Group with this name already exists");
                Response.StatusCode = 500;
                return View("AddProjGroup", input);
            }
            var group = new tbl_org_proj_group()
            {
                ID = newGroupID + 1,
                IsParent = input.IsParent,
                Level = input.Level,
                Name = input.Name,
                Description = input.Description,
                ClientID = ((PAIdentity)User.Identity).clientID,
                tbl_Org_ProjectID = input.tbl_Org_ProjectID,
                Parent_GroupID = input.Parent_GroupID
            };
            proj.tbl_org_proj_group.Add(group);
            service.Save();
            var seed = new seededProjGroup() {
                ProjectID = proj.ID,
                GroupID  = group.ID,
                GroupName = group.Name,
                Level = group.Level,
                tbl_org_proj_group = proj.tbl_org_proj_group
            };
            return View("GetGroup",seed);
        }

        public ActionResult EditProjGroup(int projID, int groupID)
        {
            if (projID == 0)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project before invoking this action";
                return View("ListItems/showError");
            }
            var ctx = (Db)service.getRepo().getDBContext();
            var grp = ctx.tbl_org_proj_group.Where(o => o.tbl_Org_ProjectID == projID && o.ID == groupID).SingleOrDefault();
            if (grp == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Group does not exist";
                return View("ListItems/showError");
            }
            Mapper<tbl_org_proj_group, tbl_org_proj_groupInput> grpMapper = new Mapper<tbl_org_proj_group, tbl_org_proj_groupInput>();

            return View(grpMapper.MapToInput(grp));
        }

        [HttpPost]
        public ActionResult EditProjGroup(tbl_org_proj_groupInput input)
        {
            Mapper<tbl_org_proj_group, tbl_org_proj_groupInput> grpMapper = new Mapper<tbl_org_proj_group, tbl_org_proj_groupInput>();
            var ctx = (Db) service.getRepo().getDBContext();
            var checkDuplicate = ctx.tbl_org_proj_group.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.Name == input.Name && o.ID != input.ID);
            if (checkDuplicate.Any())
            {
                ModelState.AddModelError("Name", "Group with this name already exists");
                Response.StatusCode = 500;
                return View("EditProjGroup", input);
            }
            var entity = ctx.tbl_org_proj_group.Where(o => o.ID == input.ID).SingleOrDefault();
            grpMapper.MapToEntity(input, entity);
            ctx.SaveChanges();
            return Content(input.Name);
        }

        [HttpPost]
        public ActionResult DeleteProjGroup(int id, int projectID)
        {
            try
            {

                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_org_proj_group.Where(o => o.ID == id && o.tbl_Org_ProjectID == projectID).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Group does not exist";
                    return View("ListItems/showError");
                }
                ctx.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
                return Json(new { Id = id, Type = typeof(tbl_org_proj_group).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddNewProjDoc(int id)
        {
            if (id == 0)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project before invoking this action";
                return View("ListItems/showError");
            }
            var input = new tbl_org_project_documentsInput();
            input.tbl_Org_ProjectID = id;
            return View(input);
        }

        [HttpPost]
        public ActionResult AddNewProjDoc(tbl_org_project_documentsInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("AddNewProjDoc", input);
                }
                var errMessage = "";
                var ctx = (Db)service.getRepo().getDBContext();
                tbl_org_project_documents entity;

                using (TransactionScope scope = new TransactionScope())
                {
                    var docID = docMgr.addDocument(input.fileName.InputStream, Server.MapPath("~"), input.fileName.FileName, ((PAIdentity)User.Identity).clientID.GetValueOrDefault(), "Project", out errMessage);
                    if (docID == 0)
                    {
                        Response.StatusCode = 412;
                        ModelState.AddModelError("", errMessage);
                        return View("AddNewProjDoc", input);
                    }
                    else
                    {
                        entity = new tbl_org_project_documents() 
                        {
                            ClientID = ((PAIdentity)User.Identity).clientID,
                            tbl_Org_ProjectID = input.tbl_Org_ProjectID,
                            mstr_Process_Doc_TypeID = input.mstr_Process_Doc_TypeID,
                            tbl_docmgr_documentID = docID,
                            Owner = input.Owner,
                            Name = input.fileName.FileName,
                            SourceType = 1,
                            Comments = input.Comments
                        };
                        ctx.tbl_org_project_documents.Add (entity);
                        ctx.SaveChanges();
                    }
                    scope.Complete();
                }
                ctx.Entry(entity).Reference(o => o.mstr_process_doc_type).Load();
                ctx.Entry(entity).Reference(o => o.tbl_docmgr_document).Load();

                return View("GetDocuments", new[] { entity });
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult uploadRevisedDoc(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();

            var projDoc = ctx.tbl_org_project_documents.Include("tbl_docmgr_document").Include("mstr_process_doc_type").Where(o => o.ID == id).SingleOrDefault();
            if (projDoc == null) throw new PAException("Project Document not found");

            var input = new tbl_org_proj_doc_versionInput()
                {
                    ID = projDoc.ID,
                    tbl_Org_ProjectID = projDoc.tbl_Org_ProjectID,
                    tbl_docmgr_documentID = projDoc.tbl_docmgr_documentID.GetValueOrDefault(),
                    Owner = projDoc.Owner,
                    DocTypeName = projDoc.mstr_process_doc_type.Name,
                    DocumentName = projDoc.tbl_docmgr_document.Name,
                    RevisedBy = ((PAIdentity)User.Identity).friendlyName,
                    Version = projDoc.tbl_docmgr_document.Version
                };
            return View(input);
        }

        [HttpPost]
        public virtual ActionResult uploadRevisedDoc(tbl_org_proj_doc_versionInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("uploadRevisedDoc", input);
                }
                var errMessage = "";
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_org_project_documents.Where(o => o.ID == input.ID).SingleOrDefault();
                if (entity == null) throw new PAException("Project Document not found");

                var result = docMgr.addDocumentVersion(input.tbl_docmgr_documentID,input.fileName.InputStream, Server.MapPath("~"), input.fileName.FileName, entity.Comments, WebSecurity.CurrentUserId, out errMessage);
                if (!result)
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", errMessage);
                    return View("uploadRevisedDoc", input);
                }
                entity.Comments = input.Comments;
                ctx.SaveChanges();
                var entities = ctx.tbl_org_project_documents.Include("mstr_process_doc_type").Include("tbl_docmgr_document").Where(o => o.ID == input.ID);

                return View("GetDocuments", entities);
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }



        public ActionResult DeleteProjDocument(int id, int docID)
        {
            var entity = service.Get(id);
            if (entity == null) throw new PAException("Project Not Found");
            var ctx = (Db) service.getRepo().getDBContext();

            var doc = ctx.tbl_org_project_documents.Where(o => o.ID == docID).SingleOrDefault();
            if (doc == null) throw new PAException("Document does not exist");
            using (TransactionScope scope = new TransactionScope())
            {
                if (doc.SourceType == 1)
                {
                    var result = docMgr.deleteDocument((int)doc.tbl_docmgr_documentID);
                    if (result)
                    {
                        ctx.tbl_org_project_documents.Remove(doc);
                        ctx.SaveChanges();
                        scope.Complete();
                        return Json(new { Id = id, Type = typeof(tbl_org_project).Name.ToLower() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        scope.Dispose();
                        Response.StatusCode = 412;
                        return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var refID = doc.ProcessDoc_RefID;
                    var type = doc.Type;
                    ctx.tbl_org_project_documents.Remove(doc);
                    ctx.SaveChanges();
                    scope.Complete();
                    var newRec = ctx.vw_process_documents.Where(o => o.tbl_Org_ProjectID == id && o.Type == type && o.ProcessDoc_RefID == refID); 
                    return View("GetProcessDocuments",newRec);

                }
            }
        }

        protected override int status(tbl_org_project o) { return o.mstr_Process_LC_StatusID; }

        protected override bool checkForDuplication(tbl_org_projectInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_projectInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override string listDisplayName(tbl_org_project o) { return o.Name; }

        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                if (!CheckAccess("Delete"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                // Delete dependent records first
                using (TransactionScope scope = new TransactionScope())
                {
                    service.getRepo().executeStoredCommand("Delete from tbl_org_proj_review_history where tbl_Org_ProjectID = " + id);
                    service.getRepo().executeStoredCommand("Delete from tbl_org_project_process_mapping where tbl_Org_ProjectID = " + id);
                    service.getRepo().executeStoredCommand("Delete from tbl_org_proj_org_level where tbl_Org_ProjectID = " + id);
                    service.Delete(id);
                    scope.Complete();
                }
                return Json(new { Id = id, Type = typeof(tbl_org_project).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FillRecord(int type, int docID, int projectID, int? filledDocID)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                filledDocID = filledDocID.GetValueOrDefault();
                var filledDoc = ctx.tbl_org_project_documents.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == (int)filledDocID).SingleOrDefault();
                if (filledDoc == null)
                {
                    if (type == 3)      // Template
                    {
                        return RedirectToAction("FillProjectTemplate", "PTemplate", new { projectID = projectID, templateID = docID });
                    }
                    else
                    {
                        return RedirectToAction("FillProjectChecklist", "PChecklist", new { projectID = projectID, checklistID = docID });
                    }
                }
                Mapper<tbl_org_project_documents, project_process_documents> docMapper = new Mapper<tbl_org_project_documents, project_process_documents>();
                return View("FillRecord", docMapper.MapToInput(filledDoc));
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public ActionResult saveRecord(project_process_documents input)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_org_project_documents.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == input.ID).SingleOrDefault();

                input.ClientID = ((PAIdentity)User.Identity).clientID;
                Mapper<tbl_org_project_documents, project_process_documents> docMapper = new Mapper<tbl_org_project_documents, project_process_documents>();
                if (input.ID == 0)
                {
                    // New record
                    entity = ctx.tbl_org_project_documents.Add(docMapper.MapToEntity(input, new tbl_org_project_documents()));
                }
                else
                {
                    // Existing record
                    entity = docMapper.MapToEntity(input, entity);
                }
                ctx.SaveChanges();
                var docs = ctx.vw_process_documents.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.DocID == entity.ID);
                ViewBag.ProjectID = entity.tbl_Org_ProjectID;
                return PartialView("GetProcessDocuments",docs);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        public override ActionResult getListItems(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_project> list = getProjects();
                exclude = new[] { 0 };
                include = new[] { 0 };


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = list.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = list.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
                    }
                }

                var returnList = orderList(list).ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                ViewBag.reload = reload;
                if (list != null && list.Any())
                {
                    if (list.Count() > 1)
                    {
                        return PartialView("ListItems/listCombo", returnList.AsEnumerable());
                    }
                    else
                    {
                        return PartialView("ShowAsDefault",list.First());
                    }
                }
                else
                {
                    return PartialView("ListItems/listCombo", returnList.AsEnumerable());
                }
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult EffortVariance(int? type)
        {
            VarianceGraphOutput variance = new VarianceGraphOutput();
            IQueryable<vw_project_variance> projData = null;
            var ctx = (Db)service.getRepo().getDBContext();

            if (type == null) type = 1;
            if (((PAIdentity)User.Identity).IsAdmin())
            {
                projData = ctx.vw_project_variance.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();

                if (user != null)
                {
                    if (user.EmployeeID != null)    // Proceed only if the user is an employee
                    {
                        var empID = user.EmployeeID.GetValueOrDefault();
                        var projList = ctx.AccessibleProjects(empID, ((PAIdentity)User.Identity).role).ToList();

                        projData = ctx.vw_project_variance.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && projList.Contains(o.tbl_Org_ProjectID));
                    }
                }
            }

            variance = new VarianceGraphOutput()
            {
                GraphType = 1,  // Line Graph
                Title = "Effort / Schedule Variance",
                LCL = -1,
                UCL = 1,
                overallEffVariance = (projData != null && projData.Any() ? decimal.Round(projData.Average(o => o.EffortVariance).GetValueOrDefault(),2) : (decimal)0),
                overallSchVariance = (projData != null && projData.Any() ? (decimal)(projData.Average(o => o.ScheduleVariance).GetValueOrDefault()) : (decimal)0)
            };
            variance.data = new List<VarianceGraphData>();
            variance.argumentField = "project";
            variance.data.Add(new VarianceGraphData()
            {
                project = "",
                upperlimit = 1,
                metric = 0,
                lowerlimit = -1
            });
            variance.series = new List<VarianceGraphSeries>();
            variance.series.Add(new VarianceGraphSeries()
            {
                valueField = "upperlimit",
                name = "Upper Limit",
                color = "#30abe0"
            });
            variance.series.Add(new VarianceGraphSeries()
            {
                valueField = "metric",
                name = "Metric",
                color = "#f06060"
            });
            variance.series.Add(new VarianceGraphSeries()
            {
                valueField = "lowerlimit",
                name = "Lower Limit",
                color = "#66c88d"
            });

            if (projData != null && projData.Any())
            {
                foreach (var p in projData)
                {
                    if ((p.EffortVariance != null) || (p.ScheduleVariance != null))
                    {
                        variance.data.Add(new VarianceGraphData()
                        {
                            project = p.Name,
                            upperlimit = 1,
                            metric = (type == 1 ? decimal.Round(p.EffortVariance.GetValueOrDefault(),2) : (decimal)p.ScheduleVariance),
                            lowerlimit = -1
                        });
                    }
                }
            }
            return Json(variance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeePieChart()
        {
            try
            {
                pieGraphOuput pie = new pieGraphOuput();
                List<pieGraphData> projData = null;
                var ctx = (Db)service.getRepo().getDBContext();

                if (((PAIdentity)User.Identity).IsAdmin())
                {
                    var proj = from alloc in ctx.tbl_org_proj_allocation.Include("tbl_org_project").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID)
                               group alloc.tbl_Org_EmployeeID by new
                                {
                                    alloc.tbl_org_project.Name,
                                } into projects
                               select new pieGraphData
                               {
                                   groupName = projects.Key.Name,
                                   groupCount = (long)projects.Count()
                               };
                    var unallocated = ctx.tbl_org_employee.Include("tbl_org_proj_allocation").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID
                                                                   && (!o.tbl_org_proj_allocation.Any()));
                    if (unallocated.Any())
                    {
                        projData = proj.ToList();
                        projData.Add(new pieGraphData() { groupName = "UnAllocated", groupCount = unallocated.Count() });
                    }
                }
                else
                {
                    var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();

                    if (user != null)
                    {
                        if (user.EmployeeID != null)    // Proceed only if the user is an employee
                        {
                            var empID = user.EmployeeID.GetValueOrDefault();
                            var projList = ctx.AccessibleProjects(empID, ((PAIdentity)User.Identity).role).ToList();

                            var proj = from alloc in ctx.tbl_org_proj_allocation.Include("tbl_org_project").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && projList.Contains(o.tbl_Org_ProjID))
                                       group alloc.tbl_Org_EmployeeID by new
                                       {
                                           alloc.tbl_org_project.Name,
                                       } into projects
                                       select new pieGraphData
                                       {
                                           groupName = projects.Key.Name,
                                           groupCount = (long)projects.Count()
                                       };

                            projData = proj.ToList();
                            var unallocated = ctx.UnAllocatedEmployees(user.EmployeeID.GetValueOrDefault(), ((PAIdentity)User.Identity).role);
                            if (unallocated.Any())
                            {
                                projData.Add(new pieGraphData() { groupName = "UnAllocated", groupCount = unallocated.Count() });
                            }
                        }
                    }
                }

                pie.type = "doughnut";
                pie.data = new List<pieGraphData>();
                if (projData != null && projData.Any())
                {
                    foreach (var p in projData)
                    {
                        pie.data.Add(p);
                    }
                }
                return Json(pie, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
                return null;
            }
        }

        public ActionResult ProjectComplianceChart()
        {
            try
            {
                barGraphOuput pie = new barGraphOuput();
                List<barGraphData> projData = null;
                var ctx = (Db)service.getRepo().getDBContext();

                if (((PAIdentity)User.Identity).IsAdmin())
                {
                    var proj = from alloc in ctx.tbl_audit_checklist.Include("tbl_org_project").Where(o => o.tbl_org_project.ClientID == ((PAIdentity)User.Identity).clientID)
                               group (alloc.PCI_Score * 100) by new
                               {
                                   alloc.tbl_org_project.Name
                               } into projects
                               select new barGraphData
                               {
                                   groupName = projects.Key.Name,
                                   groupCount = decimal.Round(projects.Average(),2)
                               };
                    projData = proj.ToList();
                }
                else
                {
                    var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();

                    if (user != null)
                    {
                        if (user.EmployeeID != null)    // Proceed only if the user is an employee
                        {
                            var empID = user.EmployeeID.GetValueOrDefault();
                            var projList = ctx.AccessibleProjects(empID, ((PAIdentity)User.Identity).role).ToList();
                            var proj = from alloc in ctx.tbl_audit_checklist.Include("tbl_org_project").Where(o => o.tbl_org_project.ClientID == ((PAIdentity)User.Identity).clientID && projList.Contains(o.tbl_Org_ProjectID))
                                       group (alloc.PCI_Score * 100) by new
                                       {
                                           alloc.tbl_org_project.Name,
                                       } into projects
                                       select new barGraphData
                                       {
                                           groupName = projects.Key.Name,
                                           groupCount = decimal.Round(projects.Average(), 2)
                                       };

                            projData = proj.ToList();
                        }
                    }
                }

                pie.type = "bar";
                pie.data = projData;
                return Json(pie, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
                return null;
            }
        }

    }
}

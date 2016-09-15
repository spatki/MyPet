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
    public class ProjEstimationController : Cruder<tbl_org_proj_estimation, tbl_org_proj_estimationInput>
    {
        public ProjEstimationController(ICrudService<tbl_org_proj_estimation> service, IMapper<tbl_org_proj_estimation, tbl_org_proj_estimationInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLEST")
        {
            functionID = "PLEST";
        }

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

        public override ActionResult Edit(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity =  ctx.tbl_org_proj_estimation.Include("tbl_org_project").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id && o.CurrentVersion == true).FirstOrDefault();
            var projDetail = ctx.tbl_org_project.Where(o => o.ID == id).SingleOrDefault();
            var input = new tbl_org_proj_estimationInput();

            if (entity == null)
            {
                input.ID = 1;
                input.newVersion = true;
                input.tbl_Org_ProjectID = id;
                input.ProjectType = projDetail.mstr_Org_Project_TypeID;
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                input.Version = 1;
                input.CreatedOn = System.DateTime.Now.Date;
                // Create empty lists for different tables contained within
                input.tbl_org_proj_estm_effort_schedule = new List<tbl_org_proj_estm_effort_schedule>();
                input.tbl_org_proj_estm_productivity = new List<tbl_org_proj_estm_productivity>();
                input.tbl_org_proj_estm_size = new List<tbl_org_proj_estm_size>();
                input.tbl_org_proj_estm_gsc = new List<tbl_org_proj_estm_gsc>();
                input.proj_estm_group = new List<proj_estm_group>();
            }
            else
            {
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_effort_schedule).Load();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_productivity).Load();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_size).Load();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_gsc).Load();

                input = editMapper.MapToInput(entity);
                input.newVersion = false;
                input.ProjectType = entity.tbl_org_project.mstr_Org_Project_TypeID;
            }
            input.loadData(ctx, ((PAIdentity)User.Identity).clientID.GetValueOrDefault());
            return View("Edit", input);
        }

        public ActionResult AddParameter(int ProjectID, int GroupID, int EstmID, int Key, string ExcludeIDs)
        {
            try
            {
                var input = new proj_estm_size_parameter();
                input.ProjectID = ProjectID;
                input.Proj_EstimationID = EstmID;
                input.GroupID = GroupID;
                input.ID = Key;
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                input.ParameterID = 0;
                input.Name = "";
                input.ExcludeIDs = ExcludeIDs;
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
        public ActionResult AddParameter(proj_estm_size_parameter input)
        {
            try
            {
                if (input.ParameterID == null)
                {
                    Response.StatusCode = 403;
                    ModelState.AddModelError("", "Select a parameter");
                    return View(input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var param = ctx.tbl_org_estm_parameters.Where(o => o.ID == input.ParameterID).SingleOrDefault();
                if (param == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Parameter does not exist anymore";
                    return View("ListItems/showError");
                }
                input.Name = param.Name;
                input.Simple = param.Simple;
                input.Medium = param.Medium;
                input.Complex = param.Complex;
                return View("ShowParameter",input);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult AddGSC(int ProjectID, int EstmID, int Key, string ExcludeIDs)
        {
            try
            {
                var input = new proj_estm_GSC();
                input.ProjectID = ProjectID;
                input.Proj_EstimationID = EstmID;
                input.ID = Key;
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                input.GSCID = 0;
                input.Name = "";
                input.ExcludeIDs = ExcludeIDs;
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
        public ActionResult AddGSC(proj_estm_GSC input)
        {
            try
            {
                if (input.GSCID == null)
                {
                    Response.StatusCode = 403;
                    ModelState.AddModelError("", "Select a General System Characteristic");
                    return View(input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_org_estm_gsc_master.Where(o => o.ID == input.GSCID).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "GSC does not exist anymore";
                    return View("ListItems/showError");
                }
                input.Name = entity.Name;
                input.Helptext = entity.HelpText;
                return View("ShowGSC", input);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult AddPhase(int ProjectID, int EstmID, int Key, string ExcludeIDs)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var project = ctx.tbl_org_project.Where(o => o.ID == ProjectID).SingleOrDefault();
                if (project == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Project does not exist anymore.";
                    return View("ListItems/showError");
                }
                var input = new proj_estm_size_parameter();
                input.ProjectID = ProjectID;
                input.Proj_EstimationID = EstmID;
                input.GroupID = project.mstr_Org_Project_TypeID;        // Being used to store the project type, based on which the phases can be displayed
                input.ID = Key;
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                input.ParameterID = 0;
                input.Name = "";
                input.ExcludeIDs = ExcludeIDs;
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
        public ActionResult AddPhase(proj_estm_size_parameter input)
        {
            try
            {
                if (input.ParameterID == null)
                {
                    Response.StatusCode = 403;
                    ModelState.AddModelError("", "Select a phase");
                    return View(input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var param = ctx.mstr_org_phase_in_proj.Include("mstr_org_proj_phase").Where(o => o.ID == input.ParameterID).SingleOrDefault();
                if (param == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Phase does not exist anymore";
                    return View("ListItems/showError");
                }
                var phaseInfo = ctx.mstr_org_proj_phase.Where(o => o.ID == param.mstr_Org_Proj_PhaseID).SingleOrDefault();
                input.ParameterID = phaseInfo.ID;
                input.Name = phaseInfo.ShortName;
                return View("ShowPhase", input);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult LinkRolesForCost (int ProjectID, int EstmID, int Key, string SelectedIDs, int PhaseID, string ExcludeIDs)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var project = ctx.tbl_org_project.Where(o => o.ID == ProjectID).SingleOrDefault();
                if (project == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Project does not exist anymore.";
                    return View("ListItems/showError");
                }
                var input = new tbl_process_rep_details();      // tbl_process_rep_details is being used to store role details
                input.ID = EstmID;                 // Property used to store the tbl_Estm_EffSchID
                input.repoID = ProjectID;       // Property used to store the PROJECT ID
                input.phaseID = PhaseID;        // Property used to store the ESTIMATION ID
                input.key = PhaseID.ToString();
                input.selectedOptions =  (SelectedIDs == null ? null : (ICollection<int>) SelectedIDs.Split(',').Select(str => int.Parse(str)).ToList<int>());
                ViewBag.ExcludeIDs = SelectedIDs + ((SelectedIDs == "" || SelectedIDs == null) ? ExcludeIDs : ((ExcludeIDs == "" || ExcludeIDs == null) ? "" : "," + ExcludeIDs));
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
        public ActionResult LinkRolesForCost(tbl_process_rep_details input)
        {
            string RoleNames = "";
            string RoleIDs = "";
            var ctx = (Db) service.getRepo().getDBContext();
            
            foreach (var rl in input.selectedOptions)
            {
                var roleDetails = ctx.mstr_org_role.Where(o => o.ID == rl).SingleOrDefault();
                if (roleDetails == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Selected role does not exist anymore";
                    return View("ListItems/showError");
                }
                RoleNames = (RoleNames == "" ? "" : RoleNames + ",") + roleDetails.ShortName;
                RoleIDs = (RoleIDs == "" ? "" : RoleIDs + ",") + roleDetails.ID.ToString();
            }
            tbl_org_proj_estm_effort_schedule entity = new tbl_org_proj_estm_effort_schedule()
            {
                ID = (int)input.phaseID,
                tbl_Org_ProjectID = input.repoID,
                tbl_Proj_EstimationID = input.ID,
                Roles = RoleNames,
                RoleIDs = RoleIDs
            };
            return View("ShowLinkedRoles", entity);
        }
        
        public ActionResult AddTeamRole(int ProjectID, int EstmID, int Key, string ExcludeIDs)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var project = ctx.tbl_org_project.Where(o => o.ID == ProjectID).SingleOrDefault();
                if (project == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Project does not exist anymore.";
                    return View("ListItems/showError");
                }
                var input = new proj_estm_size_parameter();
                input.ProjectID = ProjectID;
                input.Proj_EstimationID = EstmID;
                input.ID = Key;
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                input.ParameterID = 0;      // Will be used to store the role id
                input.Name = "";
                input.ExcludeIDs = ExcludeIDs;
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
        public ActionResult AddTeamRole(proj_estm_size_parameter input)
        {
            try
            {
                if (input.ParameterID == null)
                {
                    Response.StatusCode = 403;
                    ModelState.AddModelError("", "Select a role");
                    return View(input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var param = ctx.mstr_org_role.Where(o => o.ID == input.ParameterID).SingleOrDefault();
                if (param == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Org Role does not exist anymore";
                    return View("ListItems/showError");
                }
                input.ParameterID = param.ID;
                input.Name = param.ShortName;
                return View("ShowTeamRole", input);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_proj_estimationInput input)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                tbl_org_proj_estimation entity;
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    input.loadData(ctx, ((PAIdentity)User.Identity).clientID.GetValueOrDefault());
                    return View("Edit",input);
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    entity = service.Where(o => o.tbl_Org_ProjectID == input.tbl_Org_ProjectID && o.CurrentVersion == true).SingleOrDefault();
                    input.CurrentVersion = true;
                    input.CreatedOn = System.DateTime.Now.Date;
                    if (entity == null)
                    {
                        // This is the first time estimation sheet is being created for this project
                        entity = createMapper.MapToEntity(input, new tbl_org_proj_estimation());
                        entity.ClientID = ((PAIdentity)User.Identity).clientID;
                        var id = service.Create(entity);
                        entity = service.Get(id);
                    }
                    else
                    {
                        // Estimation sheet exists, overwrite the new sheet
                        if (input.ID != entity.ID)
                        {
                            Response.StatusCode = 412;
                            ModelState.AddModelError("", "Estimation cycle is old and overwritten by a new version. Refresh this sheet to get the latest information");
                            input.loadData(ctx, ((PAIdentity)User.Identity).clientID.GetValueOrDefault());
                            return View("Edit", input);
                        }
                        // Delete supporting information related to this entity
                        service.getRepo().executeStoredCommand("delete from tbl_org_proj_estm_size where tbl_Proj_EstimationID = " + entity.ID);
                        service.getRepo().executeStoredCommand("delete from tbl_org_proj_estm_productivity where tbl_Proj_EstimationID = " + entity.ID);
                        service.getRepo().executeStoredCommand("delete from tbl_org_proj_estm_gsc where tbl_Proj_EstimationID = " + entity.ID);
                        service.getRepo().executeStoredCommand("delete from tbl_org_proj_estm_effort_schedule where tbl_Proj_EstimationID = " + entity.ID);
                        entity = editMapper.MapToEntity(input, entity);
                        entity.ClientID = ((PAIdentity)User.Identity).clientID;
                    }
                    service.Save();
                    scope.Complete();
                }
                input.loadData(ctx, ((PAIdentity)User.Identity).clientID.GetValueOrDefault());
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult ShowPreviousCycles(int id)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = service.Where(o => o.tbl_Org_ProjectID == id && o.CurrentVersion == false);
                if (entity.Any())
                {
                    foreach (var e in entity)
                    {
                        ctx.Entry(e).Reference(o => o.mstr_org_phase_in_proj).Load();
                        ctx.Entry(e.mstr_org_phase_in_proj).Reference(o => o.mstr_org_proj_phase).Load();
                    }
                    return PartialView(entity);
                }
                else
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Previous estimation cycles do not exist for this project";
                    return View("ListItems/showError");
                }
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult ViewPreviousEstimation(int id)
        {
            try
            {
                var entity = service.Get(id);
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "This estimation cycle does not exist longer";
                    return View("ListItems/showError");
                }
                var ctx = (Db)service.getRepo().getDBContext();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_effort_schedule).Load();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_productivity).Load();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_size).Load();
                ctx.Entry(entity).Collection(o => o.tbl_org_proj_estm_gsc).Load();
                ctx.Entry(entity).Reference(o => o.mstr_org_phase_in_proj).Load();
                ctx.Entry(entity.mstr_org_phase_in_proj).Reference(o => o.mstr_org_proj_phase).Load();

                var input = editMapper.MapToInput(entity);
                input.loadData(ctx, ((PAIdentity)User.Identity).clientID.GetValueOrDefault());
                input.newVersion = false;
                input.phaseName = entity.mstr_org_phase_in_proj.mstr_org_proj_phase.LongName;
                return View("ShowEstimation", input);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult CreateCycle(tbl_org_proj_estimationInput input)
        {
            ActionResult result = Edit(input);
            if (Response.StatusCode == 200)
            {
                var ctx = (Db) service.getRepo().getDBContext();
                if (ctx.CreateEstimationCycle(input.ID, input.Version) > 0)
                {
                    input.Version = (short) (input.Version + 1);
                }
            }
            return result;
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_org_proj_estimation o)
        {
            return (o.Version == null) ? "" : "Version " + o.Version.ToString();
        }

        protected override bool checkForDuplication(tbl_org_proj_estimationInput input)
        {
            return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_proj_estimationInput input)
        {
            return false;
        }

    }
}

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
    public class ProjAllocationController : Cruder<tbl_org_proj_allocation, tbl_org_proj_allocationInput>
    {
        public ProjAllocationController(ICrudService<tbl_org_proj_allocation> service, IMapper<tbl_org_proj_allocation, tbl_org_proj_allocationInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLRALOC")
        {
            functionID = "PLRALOC";
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


        protected override string RowViewName
        {
            get { return "GetItems"; }
        }
    
        [HttpPost]
        public ActionResult SearchEmployees(searchEmployees input)
        {
            if (input.projectID == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project before searching for employees";
                return View("ListItems/showError");
            }
            IEnumerable<prc_emp_by_location> employees;
            IEnumerable<prc_emp_by_location> entity;

            var ctx = (Db)service.getRepo().getDBContext();

            employees = ctx.EmployeesByLocation(input.projectID.GetValueOrDefault(), input.locationID.GetValueOrDefault()).AsEnumerable();

            // Get employees allocated to this project at this point in time.
            if (input.searchText == null || input.searchText.Trim() == "")
            {
                entity = employees.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                if (input.searchCode == 1)
                {
                    entity = employees.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && (o.GivenName.Contains(input.searchText) || o.FamilyName.Contains(input.searchText)));
                }
                else
                {
                    entity = employees.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.EmpCode == input.searchText);
                }
            }
            ViewBag.projectID = input.projectID;
            return View(entity);
        }

        public virtual ActionResult getReportingManagersFor(int selectedItem, string controlName, int projectID, int employeeID, int roleID)
        {
            try
            {
                IEnumerable<reportingEmp> list;
                var ctx = (Db)service.getRepo().getDBContext();

                list = ctx.ReportingManagers(projectID, employeeID, roleID);

                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = node.GivenName + " " + node.FamilyName
                });

                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                ViewBag.reload = "";
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult AllocateEmployee(int projectID, int employeeID)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var proj = ctx.tbl_org_project.Where(o => o.ID == projectID).SingleOrDefault();
            var emp = ctx.tbl_org_employee.Where(o => o.ID == employeeID).SingleOrDefault();

            if (CheckAccess("Create"))
            {
                if (proj == null || emp == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Project or Employee information not found. Pl. try again.";
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
                var input =  createMapper.MapToInput(new tbl_org_proj_allocation());
                input.tbl_Org_ProjID = projectID;
                input.tbl_Org_EmployeeID = employeeID;
                input.PlannedStartDate = System.DateTime.Now;
                input.ProjectName = proj.Name;
                input.EmployeeName = emp.GivenName + " " + emp.FamilyName;
                return View(input);
            }
            else
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }

        }

        [HttpPost]
        public ActionResult AllocateEmployee(tbl_org_proj_allocationInput input)
        {
            try
            {
                if (!CheckAccess("Create"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                List<string> message;
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var ctx = (Db)service.getRepo().getDBContext();
                var wf = wrkflw.getFunctionStatus(functionID, null, null, user.IsAdmin(), null, null, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View("AllocateEmployee", input);
                }

                if (checkForDuplication(input))
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", "Duplicate Entry Found");
                    return View("AllocateEmployee", input);
                }
                if (CheckBusinessRules(input, out message))
                {
                    Response.StatusCode = 412;
                    foreach (var m in message)
                    {
                        ModelState.AddModelError("", m);
                    }
                    return View("AllocateEmployee", input);
                }
                var entity = createMapper.MapToEntity(input, new tbl_org_proj_allocation());
                entity.ClientID = ((PAIdentity)User.Identity).clientID;
                var id = service.Create(entity);
                if (input.followWF.GetValueOrDefault())
                {
                    wrkflw.saveFlow(id, WebSecurity.CurrentUserId, WebSecurity.CurrentUserId, functionID, ((PAIdentity)User.Identity).clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(), (bool?)true,"");
                }
                //return Json(new { Content = this.RenderView(RowViewName, new[] { e }) });
                var e = ctx.vw_project_allocations.Where(o => o.ID == id);
                return View(RowViewName, e);
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
                var ctx = (Db)service.getRepo().getDBContext();
                //var entity = new TEntity();
                var entity = ctx.tbl_org_proj_allocation.Include("tbl_org_employee").Include("tbl_org_project").Where(o => o.ID == id).SingleOrDefault();
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(entity), entity.ID, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }

                var input = editMapper.MapToInput(entity);
                input.tbl_Org_ProjID = entity.tbl_Org_ProjID;
                input.tbl_Org_EmployeeID = entity.tbl_Org_EmployeeID;
                input.ProjectName = entity.tbl_org_project.Name;
                input.EmployeeName = entity.tbl_org_employee.GivenName + " " + entity.tbl_org_employee.FamilyName;
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_proj_allocationInput input)
        {
            try
            {
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                List<string> message;
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
                if (CheckBusinessRules(input, out message))
                {
                    Response.StatusCode = 412;
                    foreach (var m in message)
                    {
                        ModelState.AddModelError("", m);
                    }
                    return View("Edit", input);
                }
                ReSequenceBeforeEdit(input);
                var e = editMapper.MapToEntity(input, service.Get(input.ID));
                e.ClientID = ((PAIdentity)User.Identity).clientID;
                service.Save();

                var ctx = (Db) service.getRepo().getDBContext();
                var allocRecord = ctx.vw_project_allocations.Where(o => o.ID == e.ID);
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return View(RowViewName, allocRecord);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult ReleaseEmployee(int id)
        {
            try
            {
                if (!CheckAccess("Delete"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_org_proj_allocation.Include("tbl_org_employee").Include("tbl_org_project").Where(o => o.ID == id).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Allocation Details not found.";
                    return View("ListItems/showError");
                }
                var input = editMapper.MapToInput(entity);
                input.EmployeeName = entity.tbl_org_employee.GivenName + " " + entity.tbl_org_employee.FamilyName;
                input.ProjectName = entity.tbl_org_project.Name;
                return View(input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ReleaseEmployee(tbl_org_proj_allocationInput input)
        {
            try
            {
                if (!CheckAccess("Delete"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }

                var entity = service.Get(input.ID);

                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Allocation Details not found.";
                    return View("ListItems/showError");
                }
                if (entity.ActualEndDate != null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "This employee is already released from the project on " + entity.ActualEndDate;
                    return View("ListItems/showError");
                }
                var inputValidate = editMapper.MapToInput(entity);
                List<string> message;
                inputValidate.ActualEndDate = input.ActualEndDate;
                if (CheckBusinessRules(inputValidate, out message))
                {
                    Response.StatusCode = 412;
                    foreach (var m in message)
                    {
                        ModelState.AddModelError("", m);
                    }
                    return View("ReleaseEmployee", input);
                }
                entity.ActualEndDate = input.ActualEndDate;
                service.Save();
                var ctx = (Db)service.getRepo().getDBContext();
                var rEmp = ctx.vw_project_allocations.Where(o => o.ID == input.ID);
                return View("EmployeeHistory", rEmp);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override bool checkForDuplicateEdit(tbl_org_proj_allocationInput input) 
        {
            var ctx = (Db)service.getRepo().getDBContext();

            var entity = ctx.tbl_org_proj_allocation.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjID == input.tbl_Org_ProjID
                                                            && o.tbl_Org_EmployeeID == input.tbl_Org_EmployeeID
                                                            && o.PlannedStartDate == input.PlannedStartDate
                                                            && o.PlannedEndDate == input.PlannedEndDate
                                                            && o.ID != input.ID);
            if (entity.Any()) return true;
            else return false; 
        }

        protected override bool checkForDuplication(tbl_org_proj_allocationInput input)
        {
            var ctx = (Db)service.getRepo().getDBContext();

            var entity = ctx.tbl_org_proj_allocation.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjID == input.tbl_Org_ProjID
                                                            && o.tbl_Org_EmployeeID == input.tbl_Org_EmployeeID
                                                            && o.PlannedStartDate == input.PlannedStartDate
                                                            && o.PlannedEndDate == input.PlannedEndDate);
            if (entity.Any()) return true;
            else return false;
        }

        protected bool CheckBusinessRules(tbl_org_proj_allocationInput input, out List<string> message)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            message = new List<string>();
            bool result = false;

            if (input.PlannedEndDate < input.PlannedStartDate)
            {
                message.Add("Planned end date cannot be earlier than the start date");
                result = true;
            }
            // Get project details
            var prj = ctx.tbl_org_project.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == input.tbl_Org_ProjID).SingleOrDefault();
            var emp = ctx.tbl_org_employee.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == input.tbl_Org_EmployeeID).SingleOrDefault();

            if (input.PlannedStartDate < prj.PlannedStart || input.PlannedStartDate > prj.PlannedEnd)
            {
                message.Add("Planned start date should be with the project start and end dates");
                result = true;
            }
            if (input.PlannedEndDate < prj.PlannedStart || input.PlannedEndDate > prj.PlannedEnd)
            {
                message.Add("Planned end date should be with the project start and end dates");
                result = true;
            }
            if (input.PlannedStartDate < emp.DateOfJoining)
            {
                message.Add("Planned start date cannot be earlier than employees's date of joining: " + emp.DateOfJoining);
                result = true;
            }
            if (input.PlannedStartDate > emp.DateRelieved)
            {
                message.Add("Planned start date cannot be later than employees's last date on job: " + emp.DateRelieved);
                result = true;
            }
            if (input.PlannedEndDate > emp.DateRelieved)
            {
                message.Add("Planned end date cannot be later than employees's last date on job: " + emp.DateRelieved);
                result = true;
            }
            if (input.ActualEndDate != null)
            {
                if (input.PlannedStartDate > input.ActualEndDate)
                {
                    message.Add("Release date is earlier than the allocation date");
                    result = true;
                }
            }
            var entity = ctx.vw_project_allocations.Where(o => o.projectID == input.tbl_Org_ProjID
                                                            && o.EmpID == input.tbl_Org_EmployeeID
                                                            && input.PlannedStartDate <= o.PlannedEnd
                                                            && input.PlannedEndDate >= o.PlannedStart
                                                            && o.ID != input.ID);
            if (entity.Any()) 
            {
                message.Add("This allocation is overlapping or conflicting with some previous allocation records for this employee.");
                result = true;
            }
            return result;
        }

        public ActionResult GetExistingTeam(int? id)
        {
            if (id == null) return new ContentResult() { Content = "No Project Selected" };
            var ctx = (Db)service.getRepo().getDBContext();

            var e = ctx.vw_project_allocations.Where(o => o.projectID == id && o.ActualEnd == null);
            return View("GetItems",e);
        }

        public ActionResult EmployeeHistory(int? id)
        {
            if (id == null) return new ContentResult() { Content = "No Project Selected" };
            var ctx = (Db)service.getRepo().getDBContext();

            var e = ctx.vw_project_allocations.Where(o => o.projectID == id && o.ActualEnd != null);
            return View(e);
        }

        public virtual ActionResult getMemberForRole(int? roleID, int? projectID, string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var AllocID = projectID.GetValueOrDefault();
                var proj = service.Where(o => o.tbl_Org_ProjID == AllocID && o.ActualEndDate == null);
                if (!proj.Any())
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "This record does not exist. Pl. try again or contact the system administrator.";
                    return View("ListItems/showError");	// Return error in a dialog box
                }
                var projID = proj.First().tbl_Org_ProjID;
                var users = service.Where(o => o.tbl_Org_ProjID == projID && o.ActualEndDate == null && (roleID == null || o.mstr_Org_RoleID == roleID)).Select(rec => rec.tbl_Org_EmployeeID);
                if (!users.Any())
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "No project member available.";
                    return View("ListItems/showError");	// Return error in a dialog box
                }

                var list = ctx.UserProfile.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && users.Contains((int)rec.EmployeeID));
                ViewBag.formName = formName;
                ViewBag.key = updateID;
                ViewBag.updateID = displayID;
                ViewBag.source = source;
                ViewBag.message = message;
                ViewBag.status = status;
                ViewBag.method = method;
                ViewBag.workflow = workflow;
                ViewBag.displayID = displayID;
                ViewBag.statusType = statusType;

                return View("NextUserInWF", list);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult validateUser(int? userID, int? projectID, string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var AllocID = projectID.GetValueOrDefault();
                var users = service.Where(o => o.tbl_Org_ProjID == AllocID && o.ActualEndDate == null 
                                               && (userID == null || o.tbl_Org_EmployeeID == userID)).Select(rec => rec.tbl_Org_EmployeeID);
                if (!users.Any())
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "User(s) as per workflow configuration either not available or not valid.";
                    return View("ListItems/showError");	// Return error in a dialog box
                }

                var list = ctx.UserProfile.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && users.Contains((int)rec.EmployeeID));
                ViewBag.formName = formName;
                ViewBag.key = updateID;
                ViewBag.updateID = displayID;
                ViewBag.source = source;
                ViewBag.message = message;
                ViewBag.status = status;
                ViewBag.method = method;
                ViewBag.workflow = workflow;
                ViewBag.displayID = displayID;
                ViewBag.statusType = statusType;

                return View("NextUserInWF", list);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

    }
}

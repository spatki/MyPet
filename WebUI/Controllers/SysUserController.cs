using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using System.Transactions;
using WebMatrix.WebData;
using System.Web.Security;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class SysUserController : Cruder<UserProfile, UserProfileInput>
    {
        public SysUserController(ICrudService<UserProfile> service, IMapper<UserProfile, UserProfileInput> v, IWorkflowService wf)
            : base(service, v, wf, "SYSCLUSR")
        {
            functionID = "SYSCLUSR";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public override ActionResult Create()
        {
            if (Request.QueryString["value"] == null || Request.QueryString["text"] == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a client before invoking this action";
                return View("ListItems/showError");
            }
            var input = createMapper.MapToInput(new UserProfile());
            input.Roles = new List<int>();
            input.ClientID = int.Parse(Request.QueryString["value"]);
            input.AccessType = 0;
            return View(input);
        }

        [HttpPost]
        public override ActionResult Create(UserProfileInput input)
        {
            try
            {
                if (!CheckAccess("Create"))     // Access Control
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                switch (input.UserType)        // Business Validation
                {
                    case 1:             // Employee
                        input.IsAdministrator = false;
                        input.IsGuest = false;
                        if (input.RefCode == null || input.RefCode.Trim().Equals(""))
                        {
                            ModelState.AddModelError("RefCode", "Enter the empoyee code");
                        }
                        else
                        {
                            var dbCon = (Db)service.getRepo().getDBContext();
                            var employee = dbCon.tbl_org_employee.Where(o => o.EmpCode == input.RefCode).SingleOrDefault();
                            if (employee == null)
                            {
                                ModelState.AddModelError("RefCode", "Invalid Code");
                            }
                            else
                            {
                                input.EmployeeID = employee.ID;
                            }
                        }
                        break;
                    case 2:
                        input.IsAdministrator = false;
                        input.IsGuest = false;
                        if (input.Roles == null || !input.Roles.Any())
                        {
                            ModelState.AddModelError("Roles", "Select role(s)");
                        }
                        break;
                    case 3:     // Special User     
                        if (input.AccessType == null || input.AccessType == 0)
                        {
                            ModelState.AddModelError("AccessType", "Select the access type");
                        }
                        else
                        {
                            if (input.AccessType == 1)
                            {
                                input.IsAdministrator = true;
                                input.IsGuest = false;
                            }
                            else
                            {
                                input.IsAdministrator = false;
                                input.IsGuest = true;
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (!ModelState.IsValid)       // Input validation
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
                // After passing all validations
                UserProfile e;

                using (TransactionScope scope = new TransactionScope())
                {
                    WebSecurity.CreateUserAndAccount(input.UserName, input.Password);
                    var acc = service.Where(o => o.UserName == input.UserName).SingleOrDefault();
                    int ID = acc.ID;
                    e = createMapper.MapToEntity(input, acc);
                    e.EMailID = input.UserName;   // Login ID same as email id
                    e.ID = ID;
                    if (input.Roles.Any())
                    {
                        foreach (var r in input.Roles)
                        {
                            e.webpages_UsersInRoles.Add(new webpages_UsersInRoles()
                            {
                                UserId = e.ID,
                                RoleId = r,
                                IsPrimary = false
                            });
                        }
                        // Provide the user with this role access
                    }
                    service.Save();
                    scope.Complete();
                }

                //return Json(new { Content = this.RenderView(RowViewName, new[] { e }) });
                return View(RowViewName, new[] { e });
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
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                var input = editMapper.MapToInput(entity);
                // Get supporting details
                switch (entity.UserType)
                {
                    case 1:
                        // Get the employee Code
                        service.getRepo().getDBContext().Entry(entity).Reference(o => o.tbl_org_employee).Load();
                        if (entity.tbl_org_employee != null)
                        {
                            input.RefCode = entity.tbl_org_employee.EmpCode;
                        }
                        break;
                    case 3:
                        if (input.IsAdministrator != null && input.IsAdministrator == true)
                        {
                            input.AccessType = 1;
                        }
                        else
                        {
                            if (input.IsGuest != null && input.IsGuest == true) input.AccessType = 2;
                        }
                        break;
                    default:
                        input.RefCode = "";
                        input.AccessType = 0;
                        input.IsAdministrator = false;
                        input.IsGuest = false;
                        break;
                }

                // User Role
                service.getRepo().getDBContext().Entry(entity).Collection(o => o.webpages_UsersInRoles).Load();
                if (entity.webpages_UsersInRoles.Any())
                {
                    foreach (var r in entity.webpages_UsersInRoles)
                    {
                        input.Roles.Add(r.RoleId);
                    }
                }
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(UserProfileInput input)
        {
            try
            {
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                switch (input.UserType)        // Business Validation
                {
                    case 1:             // Employee
                        input.IsAdministrator = false;
                        input.IsGuest = false;
                        if (input.RefCode == null || input.RefCode.Trim().Equals(""))
                        {
                            ModelState.AddModelError("RefCode", "Enter the empoyee code");
                        }
                        else
                        {
                            var dbCon = (Db)service.getRepo().getDBContext();
                            var employee = dbCon.tbl_org_employee.Where(o => o.EmpCode == input.RefCode).SingleOrDefault();
                            if (employee == null)
                            {
                                ModelState.AddModelError("RefCode", "Invalid Code");
                            }
                            else
                            {
                                input.EmployeeID = employee.ID;
                            }
                        }
                        break;
                    case 2:
                        input.IsAdministrator = false;
                        input.IsGuest = false;
                        if (input.Roles == null || !input.Roles.Any())
                        {
                            ModelState.AddModelError("Roles", "Select role(s)");
                        }
                        break;
                    case 3:     // Special User     
                        if (input.AccessType == null || input.AccessType == 0)
                        {
                            ModelState.AddModelError("AccessType", "Select the access type");
                        }
                        else
                        {
                            if (input.AccessType == 1)
                            {
                                input.IsAdministrator = true;
                                input.IsGuest = false;
                            }
                            else
                            {
                                input.IsAdministrator = false;
                                input.IsGuest = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (!ModelState.IsValid)       // Input validation
                {
                    Response.StatusCode = 412;
                    return View("Edit", input);
                }
                if (checkForDuplicateEdit(input))
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", "Duplicate Entry Found");
                    return View(input);
                }
                var e = editMapper.MapToEntity(input, service.Get(input.ID));
                e.EMailID = e.UserName;

                using (TransactionScope scope = new TransactionScope())
                {
                    service.Save();
                    if (input.Roles.Any())
                    {
                        // Delete existing roles
                        service.getRepo().executeStoredCommand("Delete from webpages_UsersInRoles where UserID = " + e.ID);
                        foreach (var r in input.Roles)
                        {
                            e.webpages_UsersInRoles.Add(new webpages_UsersInRoles()
                            {
                                UserId = e.ID,
                                RoleId = r,
                                IsPrimary = false
                            });
                        }
                    }
                    service.Save();
                    scope.Complete();
                }
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return View(RowViewName, new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult ResetPassword(int id)
        {
            var entity = service.Get(id);
            if (entity == null) { throw new PAException("User information not found"); }
            PasswordResetInput input = new PasswordResetInput()
            {
                ID = entity.ID,
                UserName = entity.UserName
            };
            return View(input);
        }

        [HttpPost]
        public ActionResult ResetPassword(PasswordResetInput input)
        {
            try
            {
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                var token = WebSecurity.GeneratePasswordResetToken(input.UserName, 100);
                WebSecurity.ResetPassword(token, input.Password);

                return Content("Password reset successfull");
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

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
                using (TransactionScope scope = new TransactionScope())
                {
                    // First delete the roles
                    service.getRepo().executeStoredCommand("Delete from webpages_UsersInRoles where UserID = " + id);
                    service.Delete(id);
                    scope.Complete();
                }

                return Json(new { Id = id, Type = typeof(UserProfile).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public override ActionResult ReloadItems()
        {
            int filter = 0;

            if (Request.QueryString["filter"] != null && Request.QueryString["filter"] != "") filter = int.Parse(Request.QueryString["filter"]);

            var list = service.Where(rec => rec.ClientID == filter) ;

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        protected virtual string listDisplayName(UserProfile o) { return o.DisplayName; }

        protected override bool checkForDuplication(UserProfileInput input)
        {
            var entity = service.Where(rec => rec.UserName.Trim().Equals(input.UserName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(UserProfileInput input)
        {
            var entity = service.Where(rec => rec.ID != input.ID && rec.UserName.Trim().Equals(input.UserName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        public virtual ActionResult getProjectApprovers(string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var employees = ctx.tbl_org_emp_role.Include("mstr_org_role").Where(o => o.mstr_org_role.Project_Approver == true).Select(k => k.tbl_Org_EmployeeID);
                
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && employees.Contains((int)rec.EmployeeID));
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

        public virtual ActionResult getProjectReviewers(string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var employees = ctx.tbl_org_emp_role.Include("mstr_org_role").Where(o => o.mstr_org_role.Project_Reviewer == true).Select(k => k.tbl_Org_EmployeeID);

                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && employees.Contains((int)rec.EmployeeID));
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

        public virtual ActionResult getOrgEmployeeForRole(int roleID, string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var employees = ctx.tbl_org_emp_role.Where(o => o.mstr_Org_RoleID == roleID).Select(k => k.tbl_Org_EmployeeID);

                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && employees.Contains((int) rec.EmployeeID));
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

        public virtual ActionResult getSysUserForRole(int roleID, string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var users = ctx.webpages_UsersInRoles.Where(o => o.RoleId == roleID).Select(k => k.UserId);

                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && users.Contains(rec.ID));
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

        public virtual ActionResult getReportingUser(int userID, string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();

                var userRec = service.Where(t => t.ID == userID).SingleOrDefault();
                if (userRec == null) throw new PAException("User not found");
                int employeeID = userRec.EmployeeID.GetValueOrDefault();
                var employees = ctx.tbl_org_proj_allocation.Where(o => o.tbl_Org_EmployeeID == employeeID && o.ActualEndDate == null).Select(k => k.ReportingTo).ToList();
                var employeeRecord = ctx.tbl_org_employee.Where(o => o.ID == employeeID).SingleOrDefault();
                if (employeeRecord != null)
                {
                    if (employeeRecord.Dept_Reporting != null) employees.Add(employeeRecord.Dept_Reporting);
                }
                // Assign all variables to be transferred
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

                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && employees.Contains(rec.EmployeeID));
                // If no users found, show an error
                if (list == null || !list.Any())
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Reporting Manager is not assigned to you in the system. Pl. contact your reporting manager or the system administrator";
                    return View("ListItems/showError");	// Return error in a dialog box
                }
                return View("NextUserInWF", list);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getReviewerUser(int userID, string formName, string updateID, string source, string message, bool? workflow, string method, int? status, string displayID, int? statusType)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();

                var userRec = ctx.UserProfile.Where(t => t.ID == userID).SingleOrDefault();
                if (userRec == null) throw new PAException("User not found");
                var repID = userRec.EmployeeID.GetValueOrDefault();

                var reportingMgr = ctx.tbl_org_proj_allocation.Where(o => o.tbl_Org_EmployeeID == repID && o.ActualEndDate == null).Select(k => k.ReportingTo);
                var reviewer = ctx.tbl_org_proj_allocation.Where(o => reportingMgr.Contains(o.tbl_Org_EmployeeID) && o.ActualEndDate == null).Select(k => k.ReportingTo);
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && reviewer.Contains(rec.EmployeeID));
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

        public ActionResult EditDetails()
        {
            var entity = service.Get(WebSecurity.CurrentUserId);
            EditUserDetailsInput input = new EditUserDetailsInput()
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                UserName = entity.UserName,
                EMailID = entity.EMailID,
                DisplayName = entity.DisplayName,
                MobileContact = entity.MobileContact,
                IsAdministrator = entity.IsAdministrator,
                IsGuest = entity.IsGuest,
                UserType = entity.UserType,
            };
            return PartialView(input);
        }

        [HttpPost]
        public ActionResult EditDetails(EditUserDetailsInput input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return PartialView(input);
            }
            if (input.ID != WebSecurity.CurrentUserId)
            {
                Response.StatusCode = 412;
                ModelState.AddModelError("", "This user is not currently logged in. Pl. login and try again.");
                return PartialView(input);
            }
            var entity = service.Get(input.ID);
            entity.EMailID = input.EMailID;
            entity.DisplayName = input.DisplayName;
            entity.MobileContact = input.MobileContact;
            service.Save();
            return PartialView(input);
        }
    }
}

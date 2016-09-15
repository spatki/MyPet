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
    public class SysUsrClientController : Cruder<UserProfile, UserProfileInput>
    {
        public SysUsrClientController(ICrudService<UserProfile> service, IMapper<UserProfile, UserProfileInput> v, IWorkflowService wf)
            : base(service, v, wf, "SYSUSR")
        {
            functionID = "SYSUSR";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public override ActionResult Create()
        {
            var input = createMapper.MapToInput(new UserProfile());
            input.Roles = new List<int>();
            input.ClientID = ((PAIdentity)User.Identity).clientID;
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
                if (!ModelState.IsValid)       // Input validation
                {
                    Response.StatusCode = 412;
                    return View("Create", input);
                }
                if (input.UserType == 1)        // Business Validation
                {
                    // User is of type employee
                    if (input.RefCode == null || input.RefCode.Trim().Equals(""))
                    {
                        ModelState.AddModelError("RefCode", "Enter the empoyee code");
                        Response.StatusCode = 412;
                        return View("Create", input);
                    }
                    else
                    {
                        var dbCon = (Db)service.getRepo().getDBContext();
                        var employee = dbCon.tbl_org_employee.Where(o => o.EmpCode == input.RefCode).SingleOrDefault();
                        if (employee == null)
                        {
                            ModelState.AddModelError("RefCode", "Invalid Code");
                            Response.StatusCode = 412;
                            return View("Create", input);
                        }
                        else
                        {
                            input.EmployeeID = employee.ID;
                        }
                    }
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
                    e.IsAdministrator = false;
                    e.IsGuest = false;
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
                if (entity.UserType == 1)
                {
                    // Get the employee Code
                    service.getRepo().getDBContext().Entry(entity).Reference(o => o.tbl_org_employee).Load();
                    if (entity.tbl_org_employee != null)
                    {
                        input.RefCode = entity.tbl_org_employee.EmpCode;
                    }
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
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View(input);
                }
                if (input.UserType == 1)        // Business Validation
                {
                    // User is of type employee
                    if (input.RefCode == null || input.RefCode.Trim().Equals(""))
                    {
                        ModelState.AddModelError("RefCode", "Enter the empoyee code");
                        Response.StatusCode = 412;
                        return View("Edit", input);
                    }
                    else
                    {
                        var dbCon = (Db)service.getRepo().getDBContext();
                        var employee = dbCon.tbl_org_employee.Where(o => o.EmpCode == input.RefCode).SingleOrDefault();
                        if (employee == null)
                        {
                            ModelState.AddModelError("RefCode", "Invalid Code");
                            Response.StatusCode = 412;
                            return View("Edit", input);
                        }
                        else
                        {
                            input.EmployeeID = employee.ID;
                        }
                    }
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

        public ActionResult RoleAccessIndex()
        {
            var oldValue = functionID;
            functionID = "SYSACRL";
            if (!CheckAccess("")) 
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            functionID = oldValue;
            return View();
        }

        public override ActionResult ReloadItems()
        {
            var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID) ;
            return PartialView(list);
        }

        protected override string listDisplayName(UserProfile o) { return o.DisplayName; }

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


    }
}

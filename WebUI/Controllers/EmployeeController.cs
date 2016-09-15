using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.WebUI.BAL.AccessControl;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core;
using System.Transactions;
using WebMatrix.WebData;
using System.Globalization;
using System;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class EmployeeController : Cruder<tbl_org_employee, tbl_org_employeeInput>
    {

        public EmployeeController(ICrudService<tbl_org_employee> service, IMapper<tbl_org_employee, tbl_org_employeeInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGMTREMP")
        
        {
            functionID = "DFORGMTREMP";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public override ActionResult Create()
        {
            tbl_org_employeeInput input = new tbl_org_employeeInput();

            Db dbCon = (Db) service.getRepo().getDBContext();

            var orgLevels = dbCon.mstr_org_level.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (orgLevels.Any())
            {
                var counter = 1;
                foreach (var l in orgLevels)
                {
                    input.employee_org_level.Add(new employee_org_level() { ID = counter, org_levelID = l.ID, org_level_masterID = 0, org_level_name = l.LongName});
                    counter += 1;
                }
            }
            return View(input);
        }

        [HttpPost]
        public override ActionResult Create(tbl_org_employeeInput input)
        {
            if (!CheckAccess("Create"))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            if (input.UserName == "" || input.UserName == null)
            {
                input.UserName = input.EmailID;
                ModelState.Remove("UserName");
                ModelState.Add("UserName", new ModelState());
                ModelState.SetModelValue("UserName", new ValueProviderResult(input.UserName, input.UserName, null));
            }
            if (input.EmpCode == "" || input.EmpCode == null)
            {
                input.EmpCode = "SystemGenerated";
                ModelState.Remove("EmpCode");
                ModelState.Add("EmpCode", new ModelState());
                ModelState.SetModelValue("EmpCode", new ValueProviderResult(input.EmpCode, input.EmpCode, null));
            }
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                if (input.EmpCode == "SystemGenerated") input.EmpCode = "";
                return View("Create", input);
            }
            if (checkForDuplication(input))
            {
                Response.StatusCode = 412;
                if (input.EmpCode == "SystemGenerated") input.EmpCode = "";
                ModelState.AddModelError("", "Duplicate Entry Found");
                return View("Create", input);
            }

            var entity = createMapper.MapToEntity(input, new tbl_org_employee());
            tbl_org_employee e;
            using (TransactionScope scope = new TransactionScope())
            {
                entity.ClientID = ((PAIdentity)User.Identity).clientID;
                var id = service.Create(entity);
                e = service.Get(id);

                if (e.EmpCode == "SystemGenerated")  // EMP code was not provided. Hence populate the system generated ID in it.
                {
                    var code = e.ID.ToString();
                    var chkDuplicate = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.EmpCode.Equals(code));
                    Random rand = new Random();
                    while (chkDuplicate != null)
                    {
                        if (chkDuplicate.Any())
                        {
                            code = rand.Next().ToString();
                            chkDuplicate = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.EmpCode.Equals(code));
                        }
                        else
                        {
                            chkDuplicate = null;
                            e.EmpCode = code;
                            input.EmpCode = code;
                        }
                    }
                }
                if (input.Roles.Any())      // Add Roles
                {
                    e.tbl_org_emp_role = new List<tbl_org_emp_role>();
                    foreach (var rl in input.Roles)
                    {
                        e.tbl_org_emp_role.Add(new tbl_org_emp_role()
                        {
                            EffectiveFrom = System.DateTime.Now.Date,
                            ClientID = e.ClientID,
                            mstr_Org_RoleID = int.Parse(rl),
                            tbl_Org_EmployeeID = e.ID
                        });
                    }
                    service.Save();
                }
                if (input.employee_org_level.Any())     // Add org level information
                {
                    bool orgLevelPresent = false;
                    e.tbl_org_employee_org_level = new List<tbl_org_employee_org_level>();
                    foreach (var lvl in input.employee_org_level)
                    {
                        if (lvl.org_level_masterID != null)
                        {
                            e.tbl_org_employee_org_level.Add(new tbl_org_employee_org_level()
                            {
                                mstr_Org_LevelID = lvl.org_levelID,
                                mstr_Org_Level_MasterID = lvl.org_level_masterID.GetValueOrDefault(),
                                tbl_Org_EmployeeID = e.ID
                            });
                            orgLevelPresent = true;
                        }
                    }
                    if (orgLevelPresent) service.Save();
                }
                WebSecurity.CreateUserAndAccount(input.UserName, input.Password);   // Create a user
                var user = ((Db)service.getRepo().getDBContext()).UserProfile.Where(o => o.UserName == input.UserName).Single();
                user.ClientID = e.ClientID;
                user.EmployeeID = e.ID;
                user.EMailID = input.EmailID;
                user.IsAdministrator = false;
                user.IsGuest = false;
                user.DisplayName = input.GivenName + " " + input.FamilyName;
                user.UserType = 1;
                service.Save();
                scope.Complete();
                input.ID = e.ID;
            }
            return View("Edit", input);
        }

        public override ActionResult Edit(int id)
        {
            try
            {
                //var entity = new TEntity();
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                var input = editMapper.MapToInput(entity);
                input.UserName = input.EmailID;
                // Get org levels
                Db dbCon = (Db)service.getRepo().getDBContext();
                dbCon.Entry(entity).Collection(o => o.tbl_org_employee_org_level).Load();
                dbCon.Entry(entity).Collection(o => o.tbl_org_emp_role).Load();
                var orgLevels = dbCon.mstr_org_level.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);    // Get all levels first
                if (orgLevels.Any())
                {
                    var counter = (entity.tbl_org_employee_org_level.Any() ? entity.tbl_org_employee_org_level.Max(o => o.ID) : 0);
                    counter++;
                    tbl_org_employee_org_level orgMasterID;

                    foreach (var l in orgLevels)
                    {
                        orgMasterID = (entity.tbl_org_employee_org_level.Any() ? entity.tbl_org_employee_org_level.Where(k => k.mstr_Org_LevelID == l.ID && k.tbl_Org_EmployeeID == entity.ID).SingleOrDefault() : null);
                        input.employee_org_level.Add(new employee_org_level() 
                                        { ID = ((orgMasterID == null) ? counter++ : orgMasterID.ID ), 
                                          org_levelID = l.ID, 
                                          org_level_name = l.LongName,
                                          org_level_masterID = ((orgMasterID == null) ? 0 : orgMasterID.mstr_Org_Level_MasterID )
                                        });
                    }
                }
                if (entity.tbl_org_emp_role.Any())    // Get Roles
                {
                    input.Roles = entity.tbl_org_emp_role.Select(o => o.mstr_Org_RoleID.ToString()).ToArray<string>();
                }
                
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_employeeInput input)
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
                using (TransactionScope scope = new TransactionScope())
                {
                    var e = editMapper.MapToEntity(input, service.Get(input.ID));
                    e.ClientID = ((PAIdentity)User.Identity).clientID;
                    service.getRepo().executeStoredCommand("delete from tbl_org_emp_role where tbl_Org_EmployeeID = " + input.ID);
                    service.getRepo().executeStoredCommand("delete from tbl_org_employee_org_level where tbl_Org_EmployeeID = " + input.ID);

                    
                    if (input.Roles.Any())
                    {
                        e.tbl_org_emp_role = new List<tbl_org_emp_role>();
                        foreach (var rl in input.Roles)
                        {
                            e.tbl_org_emp_role.Add(new tbl_org_emp_role()
                            {
                                EffectiveFrom = System.DateTime.Now.Date,
                                ClientID = e.ClientID,
                                mstr_Org_RoleID = int.Parse(rl),
                                tbl_Org_EmployeeID = e.ID
                            });
                        }
                    }
                    if (input.employee_org_level.Any())
                    {
                        e.tbl_org_employee_org_level = new List<tbl_org_employee_org_level>();
                        foreach (var lvl in input.employee_org_level)
                        {
                            if (lvl.org_level_masterID != null)
                            {
                                e.tbl_org_employee_org_level.Add(new tbl_org_employee_org_level()
                                {
                                    mstr_Org_LevelID = lvl.org_levelID,
                                    mstr_Org_Level_MasterID = lvl.org_level_masterID.GetValueOrDefault(),
                                    tbl_Org_EmployeeID = e.ID
                                });
                            }
                        }
                    }
                    service.Save();
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

        public virtual ActionResult Delete(int id)
        {
            try
            {
                if (!CheckAccess("Delete"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                // Delete dependent entries firt
                Db dbCon = (Db) service.getRepo().getDBContext();

                using (TransactionScope scope = new TransactionScope())
                {
                    // Delete dependent data first
                    service.getRepo().executeStoredCommand("delete from tbl_org_emp_role where tbl_Org_EmployeeID = " + id);
                    service.getRepo().executeStoredCommand("delete from tbl_org_employee_org_level where tbl_Org_EmployeeID = " + id);
                    service.getRepo().executeStoredCommand("delete from UserProfile where EmployeeID = " + id);

                    service.Delete(id);
                    scope.Complete();
                }
                return Json(new { Id = id, Type = typeof(tbl_org_employee).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SearchItems(searchEmployees input)
        {
            IEnumerable<tbl_org_employee> entity;
            Db dbCon = (Db)service.getRepo().getDBContext();

            if (input.searchText == null || input.searchText.Trim() == "")
            {
                entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                if (input.searchCode == 1)
                {
                    entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && (o.GivenName.Contains(input.searchText) || o.FamilyName.Contains(input.searchText)));
                }
                else
                {
                    entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.EmpCode == input.searchText);
                }
            }
            if (entity.Any())
            {
                foreach (var e in entity)
                {
                    dbCon.Entry(e).Reference(o => o.mstr_process_lc_status).Load();
                    dbCon.Entry(e).Reference(o => o.mstr_org_designation).Load();
                }
            }
            return View(entity);
        }

        [HttpPost]
        public ActionResult MiniSearchItems(searchEmployees input)
        {
            IEnumerable<tbl_org_employee> entity;
            Db dbCon = (Db)service.getRepo().getDBContext();

            if (input.searchText == null || input.searchText.Trim() == "")
            {
                entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                if (input.searchCode == 1)
                {
                    entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && (o.GivenName.Contains(input.searchText) || o.FamilyName.Contains(input.searchText)));
                }
                else
                {
                    entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.EmpCode == input.searchText);
                }
            }
            if (entity.Any())
            {
                foreach (var e in entity)
                {
                    dbCon.Entry(e).Reference(o => o.mstr_process_lc_status).Load();
                    dbCon.Entry(e).Reference(o => o.mstr_org_designation).Load();
                }
            }
            return View(entity);
        }

        protected override string listDisplayName(tbl_org_employee o) { return o.GivenName + " " + o.FamilyName; }

        protected override bool checkForDuplicateEdit(tbl_org_employeeInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.EmpCode.Trim().Equals(input.EmpCode.Trim()) && rec.ID != input.ID);
            if (entity.Any())
            {
                ModelState.AddModelError("EmpCode", "Emp Code already exists");
                return true;
            }
            else
            {
                // Check if the user name is duplicate
                var user = ((Db)service.getRepo().getDBContext()).UserProfile.Where(o => o.UserName == input.UserName && o.EmployeeID != input.ID);
                if (user.Any())
                {
                    ModelState.AddModelError("EmailID", "Username already exists");
                    return true;
                }
                entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.GivenName.Trim().Equals(input.GivenName.Trim())
                                                                                                   && rec.FamilyName.Trim().Equals(input.FamilyName.Trim())
                                                                                                   && rec.DateOfBirth == input.DateOfBirth
                                                                                                   && rec.ID != input.ID);
                if (entity.Any())
                {
                    ModelState.AddModelError("GivenName", "Duplicate Entry");
                    ModelState.AddModelError("FamilyName", "Duplicate Entry");
                    ModelState.AddModelError("DateOfBirth", "Duplicate Entry");
                    return true;
                }
            }
            return false;
        }

        protected override bool checkForDuplication(tbl_org_employeeInput input) 
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.EmpCode.Trim().Equals(input.EmpCode.Trim()));
            if (entity.Any())
            {
                ModelState.AddModelError("EmpCode", "Emp Code already exists");
                return true;
            }
            else
            {
                // Check if the user name is duplicate
                var user = ((Db)service.getRepo().getDBContext()).UserProfile.Where(o => o.UserName == input.UserName);
                if (user.Any())
                {
                    ModelState.AddModelError("EmailID", "Username already exists");
                    return true;
                }
                entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.GivenName.Trim().Equals(input.GivenName.Trim())
                                                                                                   && rec.FamilyName.Trim().Equals(input.FamilyName.Trim())
                                                                                                   && rec.DateOfBirth == input.DateOfBirth);
                if (entity.Any())
                {
                    ModelState.AddModelError("GivenName", "Duplicate Entry");
                    ModelState.AddModelError("FamilyName", "Duplicate Entry");
                    ModelState.AddModelError("DateOfBirth", "Duplicate Entry");
                    return true;
                }
            }
            return false;
        }

        public ActionResult getListHRMgrs(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_employee> list = new List<tbl_org_employee>();
                exclude = new[] { 0 };
                include = new[] { 0 };
                var ctx = (Db)service.getRepo().getDBContext();
                var approvers = ctx.tbl_org_emp_role.Include("mstr_org_role").Where(r => r.mstr_org_role.HR_Reporting == true).Select(k => k.tbl_Org_EmployeeID).Distinct().ToList();


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
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
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult getListDeptMgrs(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_employee> list = new List<tbl_org_employee>();
                exclude = new[] { 0 };
                include = new[] { 0 };
                var ctx = (Db)service.getRepo().getDBContext();
                var approvers = ctx.tbl_org_emp_role.Include("mstr_org_role").Where(r => r.mstr_org_role.Dept_Reporting == true).Select(k => k.tbl_Org_EmployeeID).Distinct().ToList();


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
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
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult getListApprovers(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_employee> list = new List<tbl_org_employee>();
                exclude = new[] { 0 };
                include = new[] { 0 };
                var ctx = (Db)service.getRepo().getDBContext();
                var approvers = ctx.tbl_org_emp_role.Include("mstr_org_role").Where(r => r.mstr_org_role.Project_Approver == true).Select(k => k.tbl_Org_EmployeeID).Distinct().ToList();


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && approvers.Contains(rec.ID));
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
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult getEmployeesInRole(List<int> roles, int ProjectID, string controlName, List<int> selectedItems)
        {
            if (roles == null)
            {
                ViewBag.itemName = controlName;
                ViewBag.selectedOptions = new List<string>();
                return View("multiSelectCombo");
            }
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_org_emp_role.Include("tbl_org_employee").Where(o => roles.Contains(o.mstr_Org_RoleID));
            var returnList = entity.ToList().Select(node => new SelectListItem
            {
                Value = node.tbl_org_employee.ID.ToString(),
                Text = node.tbl_org_employee.GivenName + " " + node.tbl_org_employee.FamilyName
            });
            ViewBag.selectedItem = selectedItems;
            ViewBag.itemName = controlName;
            return PartialView("ListItems/multiSelectIntCombo", returnList.AsEnumerable());
        }

        public ActionResult EmployeeDetails()
        {
            var ctx = (Db) service.getRepo().getDBContext();
            var user = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            if (user == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "User does not exist";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            if (user.EmployeeID == null)
            {
                // This user is not an employee redirect to System user details
                return RedirectToAction("EditDetails", "SysUser");
            }
            var entity = ctx.tbl_org_employee.Include("tbl_org_emp_role").Include("mstr_org_designation").Where(o => o.ID == user.EmployeeID).SingleOrDefault();
            var input = editMapper.MapToInput(entity);
            if (entity.tbl_org_emp_role.Any())    // Get Roles
            {
                input.Roles = entity.tbl_org_emp_role.Select(o => o.mstr_Org_RoleID.ToString()).ToArray<string>();
            }
            return View(input);
        }

        public ActionResult MiniEmployeeSearch()
        {
            ViewBag.Client = ((PAIdentity)User.Identity).clientID;
            return View();
        }
    }
}

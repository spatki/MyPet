using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class MenuController : Cruder<vw_org_role_access, vw_org_role_accessInput>
    {
        public MenuController(ICrudService<vw_org_role_access> service, IMapper<vw_org_role_access, vw_org_role_accessInput> v, IWorkflowService wf)
            : base(service, v, wf, "")
        {
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

 

        public ActionResult GetMenu(string functionID)
        {
            var user = ((PAIdentity)User.Identity);
            IOrderedEnumerable<vw_org_role_access> entity;
            var ctx = (Db)service.getRepo().getDBContext();

            ViewBag.Menu = functionID;

            if (user.IsAdmin())     // Give Administrative Access
            {
                entity = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Full").AsEnumerable<vw_org_role_access>().OrderBy(k => k.DisplaySequence);
            }
            else
            {
                if (user.IsGuest())     // Give Guest Access
                {
                    var access = ctx.webpages_Roles.Where(k => k.RoleName == "Guest");
                    var role = access.FirstOrDefault().ID;
                    if (access.Any())
                    {
                        entity = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Sys" && o.Sys_Role == role).AsEnumerable < vw_org_role_access>().OrderBy(k => k.DisplaySequence);
                    }
                    else entity = null;     // No Access defined
                }
                else
                {
                    var mode = user.mode();
                    var role = user.role;
                    entity = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == mode && (o.Sys_Role == null ? o.Org_Role : o.Sys_Role) == role).AsEnumerable<vw_org_role_access>().OrderBy(k => k.DisplaySequence);
                }
            }
            return PartialView(entity);
        }

        public ActionResult BreadCrump(string id, string addOnBC, string addOnBcURL, string HeaderTitle, string HeaderHelptext)
        {
            if (id == null)
            {
                return null;
            }

            var user = ((PAIdentity)User.Identity);
            vw_org_role_access parentNode;
            parentNode = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.FunctionID == id && o.AccessType == "Full").SingleOrDefault();
            if (HeaderTitle != null && HeaderTitle != "")
            {
                ViewBag.Title = HeaderTitle;
                ViewBag.headerText = HeaderTitle;
                ViewBag.headerHelp = HeaderHelptext;
            }
            else
            {
                ViewBag.Title = parentNode.ToolTip;
                ViewBag.headerText = parentNode.ToolTip;
                ViewBag.headerHelp = parentNode.HelpText;
            }
            ViewBag.FunctionID = id;
            if (parentNode == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Not a valid menu option. Pl. try again or contact the system administrator.";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            string bc = parentNode.ToolTip;
            string bcURLs = "";
            if (addOnBC != null && addOnBC != "")
            {
                bc = bc + "," + addOnBC;
                bcURLs = Url.Action(parentNode.Action, parentNode.Controller) + "," + addOnBcURL;
            }
            else
            {
                bcURLs = ",";
            }
            string currentFunctionID = parentNode.ParentFunctionID;
            while (parentNode.ParentFunctionID != null)
            {
                parentNode = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.FunctionID == currentFunctionID && o.AccessType == "Full").SingleOrDefault();
                if (parentNode != null)
                {
                    bc = parentNode.ToolTip + "," + bc;
                    bcURLs = Url.Action(parentNode.Action, parentNode.Controller) + "," + bcURLs;
                    currentFunctionID = parentNode.ParentFunctionID;
                }
            }
            ViewBag.breadcrumb = bc.Split(',');
            ViewBag.breadcrumbURLs = bcURLs.Split(',');
            return PartialView("_Header");
        }

        public ActionResult GetMobileMenu(string id)
        {
            functionID = id;
            ViewBag.FunctionID = id;
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            var user = ((PAIdentity)User.Identity);
            IOrderedEnumerable<vw_org_role_access> entity;
            var ctx = (Db)service.getRepo().getDBContext();
            vw_org_role_access parentNode;

            if (user.IsAdmin())     // Give Administrative Access
            {
                entity = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Full" && o.ParentFunctionID == id).AsEnumerable<vw_org_role_access>().OrderBy(k => k.DisplaySequence);
            }
            else
            {
                if (user.IsGuest())     // Give Guest Access
                {
                    var access = ctx.webpages_Roles.Where(k => k.RoleName == "Guest");
                    var role = access.FirstOrDefault().ID;
                    if (access.Any())
                    {
                        entity = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Sys" && o.Sys_Role == role && o.ParentFunctionID == id).AsEnumerable<vw_org_role_access>().OrderBy(k => k.DisplaySequence);
                    }
                    else entity = null;     // No Access defined
                }
                else
                {
                    var mode = user.mode();
                    var role = user.role;
                    entity = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == mode && (o.Sys_Role == null ? o.Org_Role : o.Sys_Role) == role && o.ParentFunctionID == id).AsEnumerable<vw_org_role_access>().OrderBy(k => k.DisplaySequence);
                }
            }
            // Parent node
            parentNode = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.FunctionID == id && o.AccessType == "Full").SingleOrDefault();
            if (parentNode.IsMain == true)
            {
                ViewBag.Menu = id;
            }
            return View(entity);
        }

        public ActionResult getAccessFor(int roleID, string roleType)
        {
            try
            {
                if (roleID == 0 || roleType == "")
                {
                    return null;
                }
                var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && ((roleType == "Sys") ? o.Sys_Role : o.Org_Role) == roleID && o.AccessType == roleType);
                List<MenuAccessInfo> returnList = new List<MenuAccessInfo>();
 
                if (list.Any())
                {
                    foreach (var node in list)
                    {
                        returnList.Add(new MenuAccessInfo() 
                                {
                                    MenuID = node.ID.ToString(),
                                    RefID = node.RefID,
                                    RoleID = (node.AccessType == "Sys" ? node.Sys_Role : node.Org_Role),
                                    RoleType = node.AccessType,
                                    RestrictType = (node.addAccess.GetValueOrDefault() == true && node.updateAccess.GetValueOrDefault() == true && node.deleteAccess.GetValueOrDefault() == true) ? "Full"
                                    : ((node.addAccess.GetValueOrDefault() != true && node.updateAccess.GetValueOrDefault() != true && node.deleteAccess.GetValueOrDefault() != true)
                                    ? "Read Only" : (node.addAccess.GetValueOrDefault() ? "Add " : "") + (node.updateAccess.GetValueOrDefault() ? "Update " : "")
                                    + (node.deleteAccess.GetValueOrDefault() ? "Delete" : ""))
                                });
                    }
                }
                    // if org role, check data access and add that information too
                if (roleType == "Org")
                {
                    var ctx = (Db) service.getRepo().getDBContext();
                    var dataAccess = ctx.tbl_org_role_data_access.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.RoleID == roleID);
                    if (dataAccess.Any())
                    {
                        foreach (var k in dataAccess)
                        {
                            returnList.Add(new MenuAccessInfo()
                            {
                                MenuID = k.OrgLevelID.ToString() + "M" + k.OrgMasterID.ToString(),
                                RefID = k.ID,
                                RoleID = k.RoleID,
                                RoleType = "0",
                                RestrictType = ""
                            });
                        }
                    }
                    // Get reporing access
                    var repAccess = ctx.mstr_org_role.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == roleID).SingleOrDefault();
                    if (repAccess != null)
                    {
                        if (repAccess.ReportingAccess != null && repAccess.ReportingAccess == true)
                        {
                            returnList.Add(new MenuAccessInfo()
                            {
                                MenuID = "ReportingOptionCB",
                                RefID = 0,
                                RoleID = 0,
                                RoleType = "0",
                                RestrictType = ""
                            });
                        }
                    }
                }

                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult getMenuStructure()
        {
            try
            {
                var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.AccessType == "Full");

                return View(new seededMenu() { functionID = null, vw_org_role_access = list });
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult RestrictAccess(int id,int roleID, string roleType)
        {
            try
            {
                if (id == 0 || roleID == 0 || roleType == "")
                {
                    throw new PAException("Invalid input");
                }

                var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == id && (roleType == "Sys" ? o.Sys_Role : o.Org_Role) == roleID && o.AccessType == roleType).SingleOrDefault();
                if (entity == null) throw new PAException("Entity no longer exists");
                return View(new restrictAccessInput() {
                    ID = entity.RefID,
                    MenuID = entity.ID,
                    ClientID = entity.ClientID,
                    accessType = roleType,
                    addAccess = entity.addAccess,
                    updateAccess = entity.updateAccess,
                    deleteAccess = entity.deleteAccess});
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        [HttpPost]
        public ActionResult RestrictAccess(restrictAccessInput input)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                string accessText = "";

                if (input.accessType == "Sys")
                {
                    var entity = ctx.tbl_system_role_menu_access.Where(o => o.ID == input.ID).SingleOrDefault();
                    if (entity == null) throw new PAException("Entity not found");
                    entity.updateAccess = input.updateAccess;
                    entity.addAccess = input.addAccess;
                    entity.deleteAccess = input.deleteAccess;
                    if (input.updateAccess.GetValueOrDefault() && input.addAccess.GetValueOrDefault() && input.deleteAccess.GetValueOrDefault())
                    {
                        entity.AccessType = 0;
                        accessText = "Full";
                    }
                    else
                    {
                        entity.AccessType = 1;
                        accessText = (input.addAccess == true ? "Add " : "") + (input.updateAccess == true ? "Update " : "") + (input.deleteAccess == true ? "Delete " : "");
                    }
                    ctx.SaveChanges();
                }
                else
                {
                    var entity = ctx.tbl_org_role_menu_access.Where(o => o.ID == input.ID).SingleOrDefault();
                    if (entity == null) throw new PAException("Entity not found");
                    entity.updateAccess = input.updateAccess;
                    entity.addAccess = input.addAccess;
                    entity.deleteAccess = input.deleteAccess;
                    if (input.updateAccess.GetValueOrDefault() && input.addAccess.GetValueOrDefault() && input.deleteAccess.GetValueOrDefault())
                    {
                        entity.AccessType = 0;
                        accessText = "Full";
                    }
                    else
                    {
                        if (!input.updateAccess.GetValueOrDefault() && !input.addAccess.GetValueOrDefault() && !input.deleteAccess.GetValueOrDefault())
                        {
                            entity.AccessType = 1;
                            accessText = "Read Only";
                        }
                        else
                        {
                            entity.AccessType = 1;
                            accessText = (input.addAccess == true ? "Add " : "") + (input.updateAccess == true ? "Update " : "") + (input.deleteAccess == true ? "Delete " : "");
                        }
                    }
                    ctx.SaveChanges();
                }
                PACacheManager.Refresh();
                return new ContentResult() { Content = accessText };
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult Add(int roleID, int menuID, string roleType)
        {
            if (roleID == 0 || menuID == 0 || roleType == "")
            {
                throw new PAException("Role or Menu option to be added not found");
            }
            var access = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && (roleType == "Sys" ? o.Sys_Role : o.Org_Role) == roleID && o.ID == menuID).SingleOrDefault();
            if (access == null)
            {
                // Access not set
                var ctx = (Db)service.getRepo().getDBContext();
                if (roleType == "Sys")
                    ctx.tbl_system_role_menu_access.Add(new tbl_system_role_menu_access()
                    {
                        MenuID = menuID,
                        ClientID = ((PAIdentity)User.Identity).clientID,
                        RoleID = roleID,
                        AccessType = 0,
                        addAccess = true,
                        updateAccess = true,
                        deleteAccess = true
                    });
                else
                    ctx.tbl_org_role_menu_access.Add(new tbl_org_role_menu_access()
                    {
                        MenuID = menuID,
                        ClientID = ((PAIdentity)User.Identity).clientID,
                        RoleID = roleID,
                        AccessType = 0,
                        addAccess = true,
                        updateAccess = true,
                        deleteAccess = true
                    });
                ctx.SaveChanges();
            }
            PACacheManager.Refresh();
            return Json(new { Content = "Full" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Remove(int roleID, int menuID, string roleType)
        {
            if (roleID == 0 || menuID == 0 || roleType == "")
            {
                throw new PAException("Role or Menu option to be added not found");
            }
            var access = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && (roleType == "Sys" ? o.Sys_Role : o.Org_Role) == roleID && o.ID == menuID).SingleOrDefault();
            if (access != null)
            {
                // Remove this record
                if (roleType == "Sys")
                    service.getRepo().executeStoredCommand("delete from tbl_system_role_menu_access where ID = " + access.RefID);
                else
                    service.getRepo().executeStoredCommand("delete from tbl_org_role_menu_access where ID = " + access.RefID);
            }
            PACacheManager.Refresh();
            return Json(new { Content = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult addReportingAccess(int roleID, string roleType)  // Applicable only to org roles
        {
            if (roleID == 0 || roleType == "")
            {
                throw new PAException("Role or Menu option to be added not found");
            }
            var ctx = (Db)service.getRepo().getDBContext();

            if (roleType == "Org")
            {
                var access = ctx.mstr_org_role.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == roleID).SingleOrDefault();
                if (access == null)
                {
                    throw new PAException("Role not found");
                }
                else
                {
                    // set reporting access
                    access.ReportingAccess = true;
                    ctx.SaveChanges();
                }
            }
            return Json(new { Content = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult removeReportingAccess(int roleID, string roleType)  // Applicable only to org roles
        {
            if (roleID == 0 || roleType == "")
            {
                throw new PAException("Role or Menu option to be added not found");
            }
            var ctx = (Db)service.getRepo().getDBContext();

            if (roleType == "Org")
            {
                var access = ctx.mstr_org_role.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == roleID).SingleOrDefault();
                if (access == null)
                {
                    throw new PAException("Role not found");
                }
                else
                {
                    // set reporting access
                    access.ReportingAccess = false;
                    ctx.SaveChanges();
                }
            }
            return Json(new { Content = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDataAccess(int roleID, int levelID, int levelMasterID)
        {
            if (roleID == 0 || levelID == 0 || levelMasterID == 0)
            {
                throw new PAException("Insufficient Data");
            }
            var ctx = (Db)service.getRepo().getDBContext();

            var access = ctx.tbl_org_role_data_access.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.RoleID == roleID && o.OrgLevelID == levelID && o.OrgMasterID == levelMasterID).SingleOrDefault();
            if (access == null)
            {
                // Access not set
                ctx.tbl_org_role_data_access.Add(new tbl_org_role_data_access()
                {
                    RoleID = roleID,
                    OrgLevelID = levelID,
                    OrgMasterID = levelMasterID,
                    ClientID = ((PAIdentity)User.Identity).clientID
                });
                ctx.SaveChanges();
                PACacheManager.Refresh();
            }
            return Json(new { Content = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult removeDataAccess(int roleID, int levelID, int levelMasterID)
        {
            if (roleID == 0 || levelID == 0 || levelMasterID == 0)
            {
                throw new PAException("Insufficient Data");
            }
            var ctx = (Db)service.getRepo().getDBContext();

            var access = ctx.tbl_org_role_data_access.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.RoleID == roleID && o.OrgLevelID == levelID && o.OrgMasterID == levelMasterID).SingleOrDefault();
            if (access != null)
            {
                // Access not set
                service.getRepo().executeStoredCommand("delete from tbl_org_role_data_access where ID = " + access.ID);
                PACacheManager.Refresh();
            }
            return Json(new { Content = "" }, JsonRequestBehavior.AllowGet);
        }

    }
}

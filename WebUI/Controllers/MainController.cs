using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class MainController : BaseController
    {
        protected string functionID;
        protected vw_org_role_access accessInfo;

        public ActionResult Define(string returnUrl)
        {
            functionID = "DF";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Menu = "DF";
            return View();
        }
        
        public ActionResult Plan(string id, string bc, string bcURL, string option, string optionURL)
        {
            string[] bcValues;
            string[] bcURLValues;
            if (id == null || id == "")
            {
                functionID = "PL";
                ViewBag.Menu = "PL";
            }
            else
            {
                functionID = id;
            }
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            if (bc == null || bc == "")
            {
                bc = "Plan & Track";
                bcURL = "/Main/Plan";
                bcValues = new string[] { "Plan & Track" };
                bcURLValues = new string[] { "/Main/Plan" };
            }
            else
            {
                bc = bc + "," + option;
                bcValues = bc.Split(',');
                bcURL = bcURL + "," + optionURL;
                bcURLValues = bcURL.Split(',');
            }
            ViewBag.Title = "Plan & Track";
            ViewBag.headerText = "Plan & Track";
            ViewBag.headerHelp = "Plan & track projects/functions";
            ViewBag.breadcrumb = bcValues;
            ViewBag.breadcrumbURLs = bcURLValues;
            ViewBag.FunctionID = functionID;
            ViewBag.bc = bc;
            ViewBag.bcURL = bcURL;

            return View("ShortcutOptions");
        }

        public ActionResult PlanProjOptions(string id)
        {
            if (id == null || id == "")
            {
                functionID = "PL";
            }
            else
            {
                functionID = id;
            }
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.FunctionID = functionID;
            ViewBag.Menu = "PL";
            return View();
        }

        public ActionResult ProcessOptions(string returnUrl)
        {
            functionID = "DFPRS";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        public ActionResult PMaster(string returnUrl)
        {
            functionID = "DFPRS";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        public ActionResult Record(string returnUrl)
        {
            functionID = "REC";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Menu = "REC";
            return View();
        }

        public ActionResult Audit(string returnUrl)
        {
            functionID = "ADT";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Menu = "ADT";
            return View();
        }

        public ActionResult OrgOptions(string returnUrl)
        {
            functionID = "DFORG";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult ProjOptions(string returnUrl)
        {
            functionID = "DFORGPRJ";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult FuncOptions(string returnUrl)
        {
            functionID = "DFORGFN";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult OMasters(string returnUrl)
        {
            functionID = "DFORGMTR";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult MapOptions(string returnUrl)
        {
            functionID = "DFMAP";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult System(string returnUrl)
        {
            functionID = "SYS";
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Menu = "SYS";
            return View();
        }

        public ActionResult ProcessL(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult UnderConstruction()
        {
            return View();
        }

        public ActionResult Unauthorized()
        {
            Response.StatusCode = 403;
            return View();
        }

        public ActionResult Prototype(string view)
        {
            return View("\\UnderConstruction\\" + view);
        }

        protected virtual bool CheckAccess(string restrictedFunction)
        {
            try
            {
                if (!User.Identity.IsAuthenticated) return true; // Let the cshtml page re-direct to login page for authentication
                var user = ((PAIdentity)User.Identity);

                if (functionID != "" & functionID != "Account")
                {
                    if (user.IsAdmin())     // Give Administrative Access
                    {
                        accessInfo = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Full" && o.FunctionID == functionID).SingleOrDefault();
                        if (accessInfo != null)
                        {
                            if (accessInfo.accessRange == 1)
                            {
                                // Has restricted access
                                switch (restrictedFunction)
                                {
                                    case "":
                                        return true;
                                    case "Create":
                                        if (accessInfo.addAccess == true) return true;
                                        else return false;
                                    case "Edit":
                                        if (accessInfo.updateAccess == true) return true;
                                        else return false;
                                    case "Delete":
                                        if (accessInfo.deleteAccess == true) return true;
                                        else return false;
                                    default:
                                        return false;
                                }
                            }
                            else return true;
                        }
                        else return false;
                    }
                    else
                    {
                        if (user.IsGuest())     // Give Guest Access
                        {
                            Db ctx = new Db();
                            var access = ctx.webpages_Roles.Where(k => k.RoleName == "Guest");
                            if (access.Any())
                            {
                                accessInfo = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Sys" && o.Sys_Role == access.SingleOrDefault().ID && o.FunctionID == functionID).FirstOrDefault();
                                if (accessInfo != null)
                                {
                                    // Has restricted access
                                    switch (restrictedFunction)
                                    {
                                        case "":
                                            return true;
                                        case "Create":
                                            if (accessInfo.addAccess == true) return true;
                                            else return false;
                                        case "Edit":
                                            if (accessInfo.updateAccess == true) return true;
                                            else return false;
                                        case "Delete":
                                            if (accessInfo.deleteAccess == true) return true;
                                            else return false;
                                        default:
                                            return false;
                                    }
                                }
                                else return false;
                            }
                            else
                            {
                                accessInfo = null;
                                return false;
                            }
                        }
                        else
                        {
                            var mode = user.mode();
                            var role = user.role;

                            if (mode == "Org")
                                accessInfo = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.Org_Role == role && o.AccessType == mode && o.FunctionID == functionID).SingleOrDefault();
                            else
                                accessInfo = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.Sys_Role == role && o.AccessType == mode && o.FunctionID == functionID).SingleOrDefault();
                            if (accessInfo != null)
                            {
                                // Has restricted access
                                switch (restrictedFunction)
                                {
                                    case "":
                                        return true;
                                    case "Create":
                                        if (accessInfo.addAccess == true) return true;
                                        else return false;
                                    case "Edit":
                                        if (accessInfo.updateAccess == true) return true;
                                        else return false;
                                    case "Delete":
                                        if (accessInfo.deleteAccess == true) return true;
                                        else return false;
                                    default:
                                        return false;
                                }
                            }
                            else return false;
                        }
                    }
                }
                return true; // Default to full access
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

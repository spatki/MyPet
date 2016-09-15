using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web;
using WebMatrix.WebData;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using ProcessAccelerator.WebUI.Controllers;
using System.Web.Routing;
using System.Net.Http;
using System.Net;
using System.Web.Http.Filters;
using System.Web.Http;

namespace ProcessAccelerator.WebUI.Filters
{
    public class AuthorizePAAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            HttpRequestMessage request = actionContext.ControllerContext.Request;
            var user = (PAIdentity)HttpContext.Current.User.Identity;

            Db con = new Db();

            switch (user.mode())
            {
                case "Sys":
                    if (con.vw_org_role_access.Where(o => o.AccessType == "Sys" && o.Sys_Role == user.role && o.Controller == actionContext.ControllerContext.Controller.ToString() && o.Action == actionContext.ActionDescriptor.ActionName).Any())
                        return;
                    else
                    {
                        actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, new Exception("You are not an authorized user to view this information. Pl. contact the system administrator"));
                            // new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Main", action = "AccessDenied" }));
                    }
                    break;
                case "Org":
                    if (con.vw_org_role_access.Where(o => o.AccessType == "Org" && o.Org_Role == user.role && o.Controller == actionContext.ControllerContext.Controller.ToString() && o.Action == actionContext.ActionDescriptor.ActionName).Any())
                        return;
                    else 
                    {
                        actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, new Exception("You are not an authorized user to view this information. Pl. contact the system administrator"));
                        //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Main", action = "AccessDenied" }));
                    }
                    break;
                case "Full":
                    if (con.vw_org_role_access.Where(o => o.AccessType == "Full" && o.Controller == actionContext.ControllerContext.Controller.ToString() && o.Action == actionContext.ActionDescriptor.ActionName).Any())
                        return;
                    else
                    {
                        actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, new Exception("You are not an authorized user to view this information. Pl. contact the system administrator"));
                        //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Main", action = "AccessDenied" }));
                    }
                    break;
                case "X":
                        actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, new Exception("You are not an authorized user to view this information. Pl. contact the system administrator"));
                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Main", action = "AccessDenied" }));
                    break;
                default:
                        actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, new Exception("You are not an authorized user to view this information. Pl. contact the system administrator"));
                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Main", action = "AccessDenied" }));
                    break;
            }
        }        
    }
}
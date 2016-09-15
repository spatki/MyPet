using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using ProcessAccelerator.WebUI.Controllers;
using ProcessAccelerator.WebUI.BAL.AccessControl;
using WebMatrix.WebData;
using ProcessAccelerator.WebUI.Dto;

namespace ProcessAccelerator.WebUI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            Bootstrapper.Bootstrap();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            var binder = new DateTimeModelBinder("en-GB");
            ModelBinders.Binders.Add(typeof(DateTime), binder);
            ModelBinders.Binders.Add(typeof(DateTime?), binder);
        }

        protected void application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            IPrincipal currentUser = HttpContext.Current.User;  // Get a reference of the current user

            // if we are dealing with an Authenticated forms authentication request
            if (currentUser.Identity.IsAuthenticated && currentUser.Identity.GetType() != typeof(PAIdentity))
            {
                // Create PA Identity
                var identity = new PAIdentity(((FormsIdentity)currentUser.Identity).Ticket);
                // Check if client code and user roles are found
                if (identity.clientID == null || identity.clientID == 0)
                {
                    // Client information is lost or timed out, hence forcefully logout the user
                    WebSecurity.Logout();
                }
                var user = new GenericPrincipal(identity, new string[] { identity.role.ToString() });  // Create custom principal

                HttpContext.Current.User = user;    // Attach custom PAPrincipal to current user
            }
            /*
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        var identity = ((FormsIdentity) User.Identity).Ticket;
                        string clientCode = identity.UserData;

                        //Let us set the Pricipal with our user specific details
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                          new System.Security.Principal.GenericIdentity(username, "Forms"), new string[] { clientCode });
                    }
                    catch (Exception)
                    {
                        //somehting went wrong
                    }
                }
            }
             */
        }
    }

}
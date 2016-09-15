using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using System.Web;

namespace ProcessAccelerator.WebUI.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // POST: /Account/JsonLogin

        [AllowAnonymous]
        [HttpPost]
        public ActionResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    
                    // Get account details from database
                    using (Db dbCon = new Db())
                    {
                        UserProfile user = dbCon.UserProfile.Where(o => o.UserName == model.UserName).Single();  // Get Client ID
                        string userData = user.ClientID.ToString() + "," + user.DisplayName + "," + user.IsAdministrator + "," + user.IsGuest;

                        var roles = dbCon.webpages_UsersInRoles.Where(u => u.UserId == user.ID).OrderBy(l => l.IsPrimary);

                        if (roles.Any())
                        {
                            userData = userData + ",S";
                            var roleName = roles.First();
                            dbCon.Entry(roleName).Reference(o => o.webpages_Roles).Load();
                            userData = userData + "," + roleName.webpages_Roles.RoleName;
                            userData = userData + "," + roleName.RoleId + "," + roles.Count();
                            foreach (var r in roles)
                            {
                                userData = userData + "," + r.RoleId;
                            }
                        }
                        else
                        {
                            var orgRole = dbCon.tbl_org_emp_role.Where(usr => usr.tbl_Org_EmployeeID == user.EmployeeID).OrderBy(k => k.PrimaryRole);
                            if (orgRole.Any())
                            {
                                userData = userData + ",O";
                                var roleName = orgRole.First();
                                dbCon.Entry(roleName).Reference(o => o.mstr_org_role).Load();
                                userData = userData + "," + roleName.mstr_org_role.LongName;
                                userData = userData + "," + roleName.mstr_Org_RoleID + "," + orgRole.Count();
                                foreach (var r in orgRole)
                                {
                                    userData = userData + "," + r.mstr_Org_RoleID;
                                }
                            }
                            else
                            {
                                if (user.IsAdministrator == true) userData = userData + ",F,Administrator,0,0";
                                else userData = userData + ",X,Role Undefined,0,0";
                            }
                        }
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                            0,  // Ticket version
                            model.UserName, // Username associated with this ticket
                            DateTime.Now,  // Date / Time issued
                            DateTime.Now.AddMinutes(120), // Date/Time to expire
                            true, // True for a persistant user cookie
                            userData, // Store the client id
                            FormsAuthentication.FormsCookiePath); // Path cookie valid for

                        // Encrypt the cookie using machince key for secure transport
                        string hash = FormsAuthentication.Encrypt(ticket);

                        if (!FormsAuthentication.CookiesSupported)  // If cookies are disabled in the browser
                        {
                            FormsAuthentication.SetAuthCookie(hash, model.RememberMe);
                        }
                        else    // Cookies are supported
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
                            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;    // Set the cookie's expiration time to tickets expiration time
                            Response.Cookies.Add(cookie);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("","Username or password is incorrect");
                }
            }
            return View("Index",model);
            // return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            PACacheManager.Refresh();
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/JsonRegister
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult JsonRegister(RegisterModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);

                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    Db con = new Db();
                    var user = con.UserProfile.Where(o => o.UserName == model.UserName).Single();
                    user.DisplayName = model.DisplayName;
                    user.ClientID = model.ClientID;
                    con.SaveChanges();
                    return Json(new { success = true, redirect = returnUrl }, JsonRequestBehavior.AllowGet);
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewUser()
        {
            // Attempt to register the user
            try
            {
                RegisterModel input = new RegisterModel();
                return View(input);
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("System Error", ErrorCodeToString(e.StatusCode));
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult FirstUser()
        {
            // Attempt to register the user
            try
            {
                RegisterModel input = new RegisterModel();
                input.ClientID = 0;
                return View(input);
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("System Error", ErrorCodeToString(e.StatusCode));
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult ChangePassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        ViewBag.StatusMessage = "Your password has been changed.";
                        return PartialView("_ChangePasswordPartial", new LocalPasswordModel());
                    }
                    else
                    {
                        Response.StatusCode = 412;
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
                else
                {
                    Response.StatusCode = 412;
                    return View("ChangePassword", model);
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View("ChangePassword", model);
        }


        public ActionResult Manage()
        {
            ViewBag.Mode = ((PAIdentity)User.Identity).mode();
            return View();
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (Db db = new Db())
                {
                    UserProfile user = db.UserProfile.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfile.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        public ActionResult ChangeRole()
        {
            var user = ((PAIdentity)User.Identity);
            UserRoles roleList = new UserRoles();
            roleList.userid = WebSecurity.CurrentUserId;
            roleList.Role = user.role;
            roleList.selectRoles = user.roleList;
            roleList.type = user.mode();
            return View(roleList);
        }

        [HttpPost]
        public ActionResult ChangeRole(UserRoles input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View(input);
            }
            if (input.Role == 0)
            {
                Response.StatusCode = 412;
                return Content("No Role Selected");
            }

            var user = ((PAIdentity)User.Identity);
            string userData;
            string roleName = "";
            string cslistRoles = "";

            userData = user.clientID + "," + user.friendlyName + "," + user.IsAdmin().ToString() + "," + user.IsGuest().ToString();
            using (Db dbCon = new Db())
            {
                if (user.mode() == "Sys")
                {
                    // Get roles from sys roles
                    userData = userData + ",S";
                    var roles = dbCon.webpages_UsersInRoles.Where(u => u.UserId == WebSecurity.CurrentUserId);
                    if (roles.Any())
                    {
                        foreach (var r in roles)
                        {
                            if (r.RoleId == input.Role)
                            {
                                r.IsPrimary = true;
                                user.role = r.RoleId;
                                dbCon.Entry(r).Reference(o => o.webpages_Roles).Load();
                                user.roleName = r.webpages_Roles.RoleName;
                                roleName = r.webpages_Roles.RoleName;
                            }
                            else r.IsPrimary = false;
                            cslistRoles = cslistRoles + "," + r.RoleId.ToString();
                        }
                    }
                    dbCon.SaveChanges();
                    userData = userData + "," + roleName + "," + input.Role + "," + roles.Count() + "," + cslistRoles;
                }
                else  // Assume its org
                {
                    userData = userData + ",O";
                    var userDetails = dbCon.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).Single();
                    var empid = userDetails.EmployeeID;
                    var roles = dbCon.tbl_org_emp_role.Where(usr => usr.tbl_Org_EmployeeID == empid);
                    if (roles.Any())
                    {
                        foreach (var r in roles)
                        {
                            if (r.mstr_Org_RoleID == input.Role)
                            {
                                r.PrimaryRole = true;
                                user.role = r.mstr_Org_RoleID;
                                dbCon.Entry(r).Reference(o => o.mstr_org_role).Load();
                                user.roleName = r.mstr_org_role.LongName;
                                roleName = r.mstr_org_role.LongName;
                            }
                            else r.PrimaryRole = false;
                            cslistRoles = cslistRoles + "," + r.mstr_Org_RoleID.ToString();
                        }
                    }
                    dbCon.SaveChanges();
                    userData = userData + "," + roleName + "," + input.Role + "," + roles.Count() + cslistRoles;
                }
            }

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                                    0,  // Ticket version
                                                    user.Name, // Username associated with this ticket
                                                    DateTime.Now,  // Date / Time issued
                                                    DateTime.Now.AddMinutes(120), // Date/Time to expire
                                                    true, // True for a persistant user cookie
                                                    userData, // Store the client id
                                                    FormsAuthentication.FormsCookiePath); // Path cookie valid for

            // Encrypt the cookie using machince key for secure transport
            string hash = FormsAuthentication.Encrypt(ticket);

            if (!FormsAuthentication.CookiesSupported)  // If cookies are disabled in the browser
            {
                FormsAuthentication.SetAuthCookie(hash, true);
            }
            else    // Cookies are supported
            {
                FormsAuthentication.SetAuthCookie(user.Name, true);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
                if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;    // Set the cookie's expiration time to tickets expiration time
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index","Home");
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
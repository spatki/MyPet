using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProcessAccelerator.WebUI.Controllers;
using ProcessAccelerator.Data;
using System.Linq;

namespace ProcessAccelerator.WebUI.BAL.AccessControl
{
    public static class AccessManager
    {
        public static int checkAccess(PAIdentity user, string cntlr)
        {
            if (cntlr != "" & cntlr != "Account")
            {
                if (user.IsAdmin())     // Give Administrative Access
                {
                    var checkAccess = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Full" && o.FunctionID == cntlr);
                    if (checkAccess.Any())
                    {
                        return checkAccess.FirstOrDefault().accessRange;
                    }
                    else
                    {
                        return 2;       // Access Denied
                    }
                }
                else
                {
                    if (user.IsGuest())     // Give Guest Access
                    {
                        Db ctx = new Db();
                        var access = ctx.webpages_Roles.Where(k => k.RoleName == "Guest");
                        if (access.Any())
                        {
                            var checkAccess = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Sys" && o.Sys_Role == access.FirstOrDefault().ID && o.FunctionID == cntlr);
                            if (checkAccess.Any())
                            {
                                return checkAccess.FirstOrDefault().accessRange;
                            }
                            else
                            {
                                return 2;
                            }
                        }
                        else
                            return 2;
                    }
                    else
                    {
                        var mode = user.mode();
                        var role = user.role;
                        var checkAccess = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == mode && o.Sys_Role == role && o.FunctionID == cntlr);
                        if (checkAccess.Any())
                        {
                            return checkAccess.FirstOrDefault().accessRange;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
            }
            return 0;   // Default to un-restricted access
        }

    }
}
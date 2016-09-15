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
    public class RoleAccessController : Controller
    {
        //
        // GET: /RoleAccess/

        public ActionResult Index()
        {
            return View();
        }

    }
}

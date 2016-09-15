using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.Data;
using System.Web.UI;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class WorkflowController : Controller
    {
        protected IWorkflowService wrkFlow;
        protected string functionID;

        public WorkflowController (IWorkflowService workflow)
        {
            wrkFlow = workflow;
            functionID = "DFPRSWF";
        }

        public ActionResult Index()
        {
            ViewBag.FunctionID = functionID;
            ViewBag.Menu = "DF";
            return View();
        }

        public virtual ActionResult getWorkflowFunctions(string selectedItem, string controlName, string dataBinding)
        {
            try
            {
                var list = wrkFlow.getConfigurableFunctions();

                var returnList = list.ToList().Select(node => new SelectListItem()
                {
                    Value = node.FunctionID,
                    Text = node.FriendlyName
                });

                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                ViewBag.dataBinding = dataBinding;
                return PartialView("WorkflowFunctions", returnList.AsEnumerable());
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getWorkflowDetailsFor(string functionID)
        {
            try
            {
                WorkflowRoleTypes roleTypes = new WorkflowRoleTypes();
                var workflowDetails = wrkFlow.getWorkflowForFunction(((PAIdentity)User.Identity).clientID.GetValueOrDefault(), functionID);
                return Json(workflowDetails, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
                return null;
            }
        }

        [HttpPost]
        public virtual ActionResult saveWF(workflow_edit input)
        {
            try
            {
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                return Json(wrkFlow.setWorkflow(input), JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        [HttpPost]
        public virtual ActionResult deleteWF(List<int?> input)
        {
            try
            {
                if (wrkFlow.DeleteWF(input))
                {
                    return Json(Content("Deleted"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Response.StatusCode = 403;
                    return Json(Content("Failed"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }
    }
}

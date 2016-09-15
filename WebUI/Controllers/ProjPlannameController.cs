using System.Web.Mvc;
using System.Linq;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Data;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using System.Collections.Generic;
using System.Transactions;
using WebMatrix.WebData;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class ProjPlannameController : Cruder<tbl_org_proj_planname, tbl_org_proj_plannameInput>
    {

        public ProjPlannameController(ICrudService<tbl_org_proj_planname> service, IMapper<tbl_org_proj_planname, tbl_org_proj_plannameInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLPLN")
        {
            functionID = "PLPLN";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected virtual string listDisplayName(tbl_org_proj_planname o) { return o.Name; }


        public ActionResult EditPlan(int? id, int? projectid, int? caller)
        {
            if (projectid == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Select a project before trying this option";
                return View("ListItems/showError");
            }
            var ctx = (Db)service.getRepo().getDBContext();
            tbl_org_proj_plannameInput input;
            switch (caller)
            {
                case 1:
                    ViewBag.caller = "SearchPlanID";
                    break;
                case 2:
                    ViewBag.caller = "PlanID";
                    break;
                default:
                    break;
            }
            if (id == null)
            {
                input = new tbl_org_proj_plannameInput()
                {
                    ClientID = ((PAIdentity)User.Identity).clientID,
                    tbl_Org_ProjectID = projectid.GetValueOrDefault()
                };
                return View("Create", input);
            }
            else
            {
                var entity = service.Get(id.GetValueOrDefault());
                input = editMapper.MapToInput(entity);
                return View("Edit", input);
            }
        }

        protected override bool checkForDuplication(tbl_org_proj_plannameInput input)
        {
            var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.Name.Equals(input.Name));
            if (entity.Any()) return true;
            else
                return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_proj_plannameInput input)
        {
            var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.Name.Equals(input.Name) && o.ID != input.ID);
            if (entity.Any()) return true;
            else
                return false;
        }

        public ActionResult getListItemsJSONFor(int projectID)
        {
            try
            {
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == projectID);
                var returnList = from node in list
                                 select new
                                 {
                                     index = node.ID,
                                     name = node.Name
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getPlansFor(int selectedItems, string controlName, int projectID)
        {
            try
            {
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == projectID);
                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

                ViewBag.selectedItems = selectedItems;
                ViewBag.itemName = controlName;
                return PartialView(returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }


    }
}

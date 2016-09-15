using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using System.Collections.Generic;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class OrgLevelMasterController : Cruder<mstr_org_level_master, mstr_org_level_masterInput>
    {
        public OrgLevelMasterController(ICrudService<mstr_org_level_master> service, IMapper<mstr_org_level_master, mstr_org_level_masterInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGMRTLVLMTR")
        {
            functionID = "DFORGMRTLVLMTR";
        }

        public override ActionResult ReloadItems()
        {
            int filter = 0;

            if (Request.QueryString["filter"] != null && Request.QueryString["filter"] != "") filter = int.Parse(Request.QueryString["filter"]);

            var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_LevelID == filter).OrderBy(o => o.ShortName); ;

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        public override ActionResult Create()
        {
            if (Request.QueryString["value"] == null || Request.QueryString["text"] == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a organisation level before invoking this action";
                return View("ListItems/showError");
            }
            var input = createMapper.MapToInput(new mstr_org_level_master());
            input.mstr_Org_LevelID = int.Parse(Request.QueryString["value"]);
            return View(input);
        }

        public virtual ActionResult getListItemsFor(int selectedItem, string controlName, string excludeIds, string selectIds, int levelID)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<mstr_org_level_master> list = new List<mstr_org_level_master>();
                exclude = new[] { 0 };
                include = new[] { 0 };


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_LevelID == levelID);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_LevelID == levelID);
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_LevelID == levelID);
                    }
                }

                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                ViewBag.reload = "";
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getListItemsJSONFor(int levelID, string excludeIDs)
        {
            try
            {
                IEnumerable<int> exclude = new[] { 0 };
                if (excludeIDs != null & excludeIDs != "")
                {
                    exclude = excludeIDs.Split(',').Select(str => int.Parse(str));
                }

                var list = service.Where(rec => !exclude.Contains(rec.ID) &&  rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_LevelID == levelID);
                var returnList = from node in list
                                 select new
                                 {
                                     index = node.ID,
                                     name = listDisplayName(node)
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected virtual string listDisplayName(mstr_org_level_master o) { return o.LongName; }

        protected override bool checkForDuplication(mstr_org_level_masterInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()) && rec.mstr_Org_LevelID == input.mstr_Org_LevelID);
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_level_masterInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()) && rec.mstr_Org_LevelID == input.mstr_Org_LevelID);
            if (entity.Any()) return true;
            else return false;
        }

    }
}

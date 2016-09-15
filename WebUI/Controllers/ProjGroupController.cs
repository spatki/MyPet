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

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class ProjGroupController : Cruder<tbl_org_proj_group, tbl_org_proj_groupInput>
    {
        public ProjGroupController(ICrudService<tbl_org_proj_group> service, IMapper<tbl_org_proj_group, tbl_org_proj_groupInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLGRP")
        {
            functionID = "PLGRP";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }


        public ActionResult AddNewModuleForEstm(int ProjectID, int EstmID, int Key, string excludeIds)
        {
            try
            {
                if (CheckAccess("Edit"))
                {
                    ViewBag.allowEdit = true;
                }
                else
                {
                    ViewBag.allowEdit = false;
                }
                var input = new proj_estm_group();
                input.ProjectID = ProjectID;
                input.Proj_EstimationID = EstmID;
                input.GroupID = null;
                input.OldGroupID = null;
                input.ExcludeIDs = excludeIds;
                return View("SelectEstmModule", input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult EditModuleForEstm(int ProjectID, int EstmID, int Key, int GroupID, string excludeIds)
        {
            try
            {
                if (CheckAccess("Edit"))
                {
                    ViewBag.allowEdit = true;
                }
                else
                {
                    ViewBag.allowEdit = false;
                }
                var groupInfo = service.Where(o => o.tbl_Org_ProjectID == ProjectID && o.ID == GroupID).SingleOrDefault();
                if (groupInfo == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Module does not exist anymore";
                    return View("ListItems/showError");
                }
                var input = new proj_estm_group();
                input.ProjectID = ProjectID;
                input.Proj_EstimationID = EstmID;
                input.GroupID = GroupID;
                input.OldGroupID = GroupID;
                input.Name = groupInfo.Name;
                input.ExcludeIDs = excludeIds;
                return View("SelectEstmModule", input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public ActionResult AddNewModuleForEstm(proj_estm_group input)
        {
            if (CheckAccess("Edit"))
            {
                ViewBag.allowEdit = true;
            }
            else
            {
                ViewBag.allowEdit = false;
            }
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 500;
                return View("SelectEstmModule",input);
            }
            if (input.GroupID == 0 && input.Name.Trim() != "")
            {
                // This is a new group being added
                var LastGroupDetails = service.Where(o => o.tbl_Org_ProjectID == input.ProjectID);
                var NewGroupID = (LastGroupDetails.Any() ? LastGroupDetails.Max(k => k.ID) : 1);
                var newGroup = new tbl_org_proj_group()
                {
                    ID = NewGroupID + 1,
                    ClientID = ((PAIdentity)User.Identity).clientID,
                    Name = input.Name,
                    Level = 1,
                    tbl_Org_ProjectID = input.ProjectID
                };
                if (checkForDuplication(createMapper.MapToInput(newGroup)))
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("", "Duplicate Entry");
                    return View("SelectEstmModule", input);
                }
                var newID = service.Create(newGroup);
                input.GroupID = newGroup.ID;
            }
            else
            {
                var grpDetails = service.Where(o => o.ID == (int) input.GroupID && o.tbl_Org_ProjectID == input.ProjectID).SingleOrDefault();
                if (grpDetails == null)
                {
                    ModelState.AddModelError("","This group does not exist. Pl. try again");
                    return View("SelectEstmModule", input);
                }
                if (grpDetails.Name != input.Name)
                {
                    // Group Name has been modified. Modify the same.
                    var grpInput = editMapper.MapToInput(grpDetails);
                    grpInput.Name = input.Name;
                    if (checkForDuplicateEdit(grpInput))
                    {
                        Response.StatusCode = 500;
                        ModelState.AddModelError("", "Duplicate Entry");
                        return View("SelectEstmModule", input);
                    }
                    grpDetails.Name = input.Name;
                    service.Save();
                }
            }
            return View("EstmGroupDetails", input);
        }
        
        protected override string listDisplayName(tbl_org_proj_group o) { return o.Name; }

        protected override bool checkForDuplication(tbl_org_proj_groupInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == input.tbl_Org_ProjectID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_proj_groupInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == input.tbl_Org_ProjectID && rec.ID != input.ID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        public ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult getListItemsFor(int selectedItem, string controlName, string excludeIds, string selectIds, int projectID, bool allowNewClient, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_proj_group> list = new List<tbl_org_proj_group>();
                exclude = new[] { 0 };
                include = new[] { 0 };


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == projectID);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == projectID);
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == projectID);
                    }
                }

                var returnList = new List<SelectListItem>();
                CreateList(list, ref returnList, (int?)null, "");
                if (allowNewClient)
                {
                    if (CheckAccess("Create"))
                    {
                        ViewBag.defaultItem = "New Module";
                    }
                }
                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                ViewBag.reload = reload;
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        protected void CreateList(IEnumerable<tbl_org_proj_group> groups, ref List<SelectListItem> currentList, int? ParentID, string ParentName)
        {
            if (currentList == null) currentList = new List<SelectListItem>();
            var processList = groups.Where(o => (o.Parent_GroupID == null && ParentID == null) || (ParentID != null && o.Parent_GroupID == ParentID)).ToList();
            if (processList != null && processList.Any())
            {
                foreach (var grp in processList)
                {
                    string NodeName = ParentName + (ParentName != "" ? " - " : "") + grp.Name;
                    currentList.Add(new SelectListItem()
                    {
                        Value = grp.ID.ToString(),
                        Text = NodeName
                    });
                    CreateList(groups, ref currentList, grp.ID, NodeName);
                }
            }
        }

        public ActionResult getListItemsJSONFor(int projectID)
        {
            try
            {
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == projectID);
                var returnList = new List<SelectListItem>();
                CreateList(list, ref returnList, (int?)null, "");
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

    }
}

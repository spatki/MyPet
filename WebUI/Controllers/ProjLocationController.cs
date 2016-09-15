using System.Web.Mvc;using System.Linq;
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
    public class ProjLocationController : Cruder<tbl_org_proj_location, tbl_org_proj_locationInput>
    {
        //
        // GET: /ProjLocation/
        public ProjLocationController(ICrudService<tbl_org_proj_location> service, IMapper<tbl_org_proj_location, tbl_org_proj_locationInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLLOC")
        {
            functionID = "PLLOC";
        }

        public override ActionResult Index()
        {
            var user = (PAIdentity)User.Identity;
            var selectedIDs = "";
            if (!user.IsAdmin())
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var userDetails = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
                if (userDetails == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "User does not exist";
                    return View("ListItems/showError");
                }
                if (userDetails.EmployeeID == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "User is not an employee";
                    return View("ListItems/showError");
                }
                var projIDs = ctx.AccessibleProjects(userDetails.EmployeeID.GetValueOrDefault(), user.role);
                selectedIDs = "0";
                foreach (var prj in projIDs)
                {
                    selectedIDs = selectedIDs + "," + prj.ToString();
                }
            }
            ViewBag.SelectedIDs = selectedIDs;
            return View();
        }


        public ActionResult GetItemsFor(int id)
        {
            var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == id);
            return View("GetItems",entity);
        }

        public override ActionResult ReloadItems()
        {
            int filter = 0;

            if (Request.QueryString["filter"] != null && Request.QueryString["filter"] != "") filter = int.Parse(Request.QueryString["filter"]);

            var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == filter);

            foreach (var l in list)
            {
                LoadDependencies(l);
            }
            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        public override ActionResult Create()
        {
            if (Request.QueryString["value"] == null || Request.QueryString["text"] == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project before invoking this action";
                return View("ListItems/showError");
            }
            var input = new tbl_org_proj_locationInput();
            input.tbl_Org_ProjectID = int.Parse(Request.QueryString["value"]);

            var ctx = (Db)service.getRepo().getDBContext();
            var projLevelMappingInfo = ctx.tbl_org_project.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == input.tbl_Org_ProjectID).SingleOrDefault();

            if (projLevelMappingInfo == null) input.LevelID = 0;
            else input.LevelID = (projLevelMappingInfo.LocationOrgLevelID == null ? 0 : projLevelMappingInfo.LocationOrgLevelID);

            ViewBag.ProjectName = Request.QueryString["text"];
            return View(input);
        }

        public override ActionResult Edit(int id)
        {
            var input = new tbl_org_proj_locationInput();

            var entity = service.Get(id);
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select a project before invoking this action";
                return View("ListItems/showError");
            }
            
            var ctx = (Db)service.getRepo().getDBContext();
            var projLevelMappingInfo = ctx.tbl_org_project.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == entity.tbl_Org_ProjectID).SingleOrDefault();
            input = editMapper.MapToInput(entity);
            if (projLevelMappingInfo == null) input.LevelID = 0;
            else input.LevelID = (projLevelMappingInfo.LocationOrgLevelID == null ? 0 : projLevelMappingInfo.LocationOrgLevelID);

            ViewBag.ProjectName = projLevelMappingInfo.Name;
            return View(input);
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_org_proj_location o) { return o.Name; }

        protected override bool checkForDuplication(tbl_org_proj_locationInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == input.tbl_Org_ProjectID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_proj_locationInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == input.tbl_Org_ProjectID &&  rec.ID != input.ID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override void ReSequenceBeforeCreate(tbl_org_proj_locationInput input)
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.SequenceNo >= input.SequenceNo && o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.tbl_Org_ProjectID).OrderBy(p => p.SequenceNo);
                if (restEntries.Any())
                {
                    byte sequence = (byte)(input.SequenceNo + 1);
                    foreach (var r in restEntries)
                    {
                        if (r.ID != input.ID)
                        {
                            r.SequenceNo = sequence;
                            sequence = (byte)(sequence + 1);
                        }
                    }
                }
                service.Save();
            }
        }

        protected override void ReSequenceBeforeEdit(tbl_org_proj_locationInput input)
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.SequenceNo >= input.SequenceNo && o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.tbl_Org_ProjectID).OrderBy(p => p.SequenceNo);
                if (restEntries.Any())
                {
                    byte sequence = (byte)(input.SequenceNo + 1);
                    foreach (var r in restEntries)
                    {
                        if (r.ID != input.ID)
                        {
                            r.SequenceNo = sequence;
                            sequence = (byte)(sequence + 1);
                        }
                    }
                }
                service.Save();
            }
        }

        public virtual ActionResult getListItemsJSONFor(int id)
        {
            try
            {
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == id);
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

        public virtual ActionResult getListItemsFor(int selectedItem, string controlName, string excludeIds, string selectIds, int projectID)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_proj_location> list = new List<tbl_org_proj_location>();
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

                var returnList = orderList(list).ToList().Select(node => new SelectListItem
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


    }
}

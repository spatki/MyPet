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
    [InitializeSimpleMembership]
    public class OConfigurationController : Cruder<tbl_org_level_organisation, tbl_org_level_organisationInput>
    {
        public OConfigurationController(ICrudService<tbl_org_level_organisation> service, IMapper<tbl_org_level_organisation, tbl_org_level_organisationInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGCFG")
        {
            functionID = "DFORGCFG";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_org_level_organisation o) { return o.ID.ToString(); }


        public ActionResult getStructure()
        {
            try
            {
                var list = service.Where(s => s.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(o => o.Level);
                if (list.Any())
                {
                    foreach (var e in list)
                    {
                        service.getRepo().getDBContext().Entry(e).Reference(x => x.mstr_org_level_master).Load();
                    }
                }
                var returnList = from node in list
                                 select new
                                 {
                                     ID = node.ID,
                                     nodeID = node.mstr_Org_Level_MasterID,
                                     nodeName = node.mstr_org_level_master.LongName,
                                     Level = node.Level,
                                     ParentNodeID = node.mstr_Org_Level_ParentID
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }


        public override ActionResult Create()
        {
            string level = Request.QueryString["level"];
            string parent = Request.QueryString["parent"];


            if (short.Parse(level) == 0 || int.Parse(parent) < 0)
            {
                Response.StatusCode = 500;
                Response.Write("Invalid Request. Pl. contact the administrator");
                return null;
            }
            IMapper<tbl_org_level_organisation, tbl_org_level_organisationInput> cMapper = new Mapper<tbl_org_level_organisation, tbl_org_level_organisationInput>();
            tbl_org_level_organisationInput tbl = cMapper.MapToInput(new tbl_org_level_organisation());

            tbl.Level = short.Parse(level);
            if (parent == "0")
            {
                // Node created at the root.
                tbl.mstr_Org_Level_ParentID = null;
                tbl.StructPath = "0";
                ViewBag.excludeMasterIDs = tbl.StructPath;
                // Add to the exclude list of roles. No roles should be added twice of create a cyclic reference
                var excludeList = service.Where(rec => rec.Level == tbl.Level);
                foreach (var e in excludeList)
                {
                    ViewBag.excludeMasterIDs = ViewBag.excludeMasterIDs + "," + e.mstr_Org_Level_MasterID.ToString();
                }
            }
            else
            {
                // This is a sub node
                tbl.mstr_Org_Level_ParentID = int.Parse(parent);
                // Add to the struct path of the parent
                var parentEntity = service.Get(int.Parse(parent));
                tbl.StructPath = parentEntity.StructPath + "," + parentEntity.mstr_Org_Level_MasterID.ToString();
                ViewBag.excludeMasterIDs = tbl.StructPath;
                // Add to the exclude list of roles. No roles should be added twice of create a cyclic reference
                var excludeList = service.Where(rec => rec.mstr_Org_Level_ParentID == tbl.mstr_Org_Level_ParentID && rec.Level == tbl.Level);
                foreach (var e in excludeList)
                {
                    ViewBag.excludeMasterIDs = ViewBag.excludeMasterIDs + "," + e.mstr_Org_Level_MasterID.ToString();
                }
            }

            return View(tbl);
        }

        [HttpPost]
        public override ActionResult Create(tbl_org_level_organisationInput input)
        {
            // This method is overridden here because the navigation property of entity object is not geting loaded
            // even without lazy loading. Rest of the code is same as Crudere implementation.
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;

                    ViewBag.excludeMasterIDs = input.StructPath;
                    // Add to the exclude list of roles. No roles should be added twice of create a cyclic reference
                    var excludeList = service.Where(rec => rec.mstr_Org_Level_ParentID == input.mstr_Org_Level_ParentID && rec.Level == input.Level);
                    foreach (var node in excludeList)
                    {
                        ViewBag.excludeMasterIDs = ViewBag.excludeMasterIDs + "," + node.mstr_Org_Level_MasterID.ToString();
                    }

                    return View("Create", input);
                }
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                var id = service.Create(createMapper.MapToEntity(input, new tbl_org_level_organisation()));
                var e = service.Get(id);
                // Somehow the navigation property (FK) is not getting loaded by EF. Hence explicitely load it.
                // This data is needed in the view to display the role name.
                service.getRepo().getDBContext().Entry(e).Reference(x => x.mstr_org_level_master).Load();
                service.getRepo().getDBContext().Entry(e).Reference(x => x.mstr_org_level).Load();

                //return Json(new { Content = this.RenderView(RowViewName, new[] { e }) });
                return View(RowViewName, new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [OutputCache(Location = OutputCacheLocation.None)]//for ie
        public override ActionResult Edit(int id)
        {
            try
            {
                //var entity = new TEntity();
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                service.getRepo().getDBContext().Entry(entity).Reference(x => x.mstr_org_level).Load();
                service.getRepo().getDBContext().Entry(entity).Reference(x => x.mstr_org_level_master).Load();
                var input = editMapper.MapToInput(entity);
                input.levelName = entity.mstr_org_level.ShortName;
                input.masterDataName = entity.mstr_org_level_master.LongName;
                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_level_organisationInput input)
        {
            // This method is overridden here because the navigation property of entity object is not geting loaded
            // even without lazy loading. Rest of the code is same as Crudere implementation.
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View(input);
                }
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                var e = editMapper.MapToEntity(input, service.Get(input.ID));
                service.Save();
                // Somehow the navigation property (FK) is not getting loaded by EF. Hence explicitely load it.
                // This data is needed in the view to display the role name.
                service.getRepo().getDBContext().Entry(e).Reference(x => x.mstr_org_level).Load();
                service.getRepo().getDBContext().Entry(e).Reference(x => x.mstr_org_level_master).Load();
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return View(RowViewName, new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }


    }
}

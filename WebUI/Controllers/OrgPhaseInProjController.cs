using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class OrgPhaseInProjController : Cruder<mstr_org_phase_in_proj, mstr_org_phase_in_projInput>
    {
        public OrgPhaseInProjController(ICrudService<mstr_org_phase_in_proj> service, IMapper<mstr_org_phase_in_proj, mstr_org_phase_in_projInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGCFG")
        {
            functionID = "DFORGCFG";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public ActionResult linkPhase (int projID)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.mstr_org_project_type.Where(p => p.ID == projID);
                if (!entity.Any()) throw new PAException("Project type doesn't exist anymore");

                var phases = service.Where(o => o.mstr_Org_Project_TypeID == projID);
                string excludeIDs = "0";

                if (phases.Any())
                {
                    foreach (var p in phases)
                    {
                        excludeIDs = excludeIDs + "," + p.mstr_Org_Proj_PhaseID.ToString();
                    }
                }
                ViewBag.excludeIDs = excludeIDs;

                tbl_org_config_details input = new tbl_org_config_details();
                input.ID = projID;
                input.Name = entity.FirstOrDefault().LongName;

                return View("Create",input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult linkPhase(tbl_org_config_details input)
        {
            try
            {
                mstr_org_phase_in_proj entity;
                int id;
                
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("Create", input);
                }
                if (input.selectedOptions.Any())
                {
                    foreach (var opt in input.selectedOptions)
                    {
                        entity = new mstr_org_phase_in_proj()
                                    {
                                        mstr_Org_Proj_PhaseID = opt,
                                        mstr_Org_Project_TypeID = input.ID,
                                        ClientID = ((PAIdentity)User.Identity).clientID
                                    };
                        id = service.Create(entity);
                    }
                }
                var ctx = (Db) service.getRepo().getDBContext();
                var projType = ctx.mstr_org_project_type.Include("mstr_org_phase_in_proj").Where(o => o.ID == input.ID).SingleOrDefault();
                if (projType.mstr_org_phase_in_proj.Any())
                {
                    foreach (var Model in projType.mstr_org_phase_in_proj)
                    {
                        ctx.Entry(Model).Reference(l => l.mstr_org_proj_phase).Load();
                    }
                }
                return View(RowViewName, projType);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public virtual ActionResult getListItemsFor(int selectedItem, string controlName, string excludeIds, string selectIds, int projectTypeID, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<mstr_org_phase_in_proj> list = new List<mstr_org_phase_in_proj>();
                exclude = new[] { 0 };
                include = new[] { 0 };
                var ctx = (Db)service.getRepo().getDBContext();


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = ctx.mstr_org_phase_in_proj.Include("mstr_org_proj_phase").Where(rec => !exclude.Contains(rec.mstr_Org_Proj_PhaseID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_Project_TypeID == projectTypeID);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = ctx.mstr_org_phase_in_proj.Include("mstr_org_proj_phase").Where(rec => include.Contains(rec.mstr_Org_Proj_PhaseID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_Project_TypeID == projectTypeID);
                    }
                    else
                    {
                        list = ctx.mstr_org_phase_in_proj.Include("mstr_org_proj_phase").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.mstr_Org_Project_TypeID == projectTypeID);
                    }
                }

                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

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

        protected override string listDisplayName(mstr_org_phase_in_proj o) { return o.mstr_org_proj_phase.LongName; }

    }
}

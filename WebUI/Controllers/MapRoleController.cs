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
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using System;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class MapRoleController : Cruder<tbl_mapping_role, tbl_mapping_roleInput>
    {
        public MapRoleController(ICrudService<tbl_mapping_role> service, IMapper<tbl_mapping_role, tbl_mapping_roleInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFMAPRL")
        {
            functionID = "DFMAPRL";
        }

        public override ActionResult Index()
        {
            try
            {
                var ctx = (Db) service.getRepo().getDBContext();
                var entity = ctx.vw_role_mapping.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
                return View(entity);
            }
            catch (PAException ex)
            {
                ex.Raize();
            }
            return null;
        }

        public ActionResult addRole(int id)
        {
            var entity = new tbl_mapping_roleInput();
            entity.ClientID = ((PAIdentity)User.Identity).clientID;
            entity.mstr_Org_RoleID = id;
            return View("Create",entity);
        }

        [HttpPost]
        public override ActionResult Create(tbl_mapping_roleInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("Create", input);
                }
                var entity = createMapper.MapToEntity(input, new tbl_mapping_role());
                entity.ClientID = ((PAIdentity)User.Identity).clientID;
                var id = service.Create(entity);
                var e = service.Get(id);
                service.getRepo().getDBContext().Entry(e).Reference(o => o.mstr_process_role).Load();
                //return Json(new { Content = this.RenderView(RowViewName, new[] { e }) });
                return View(RowViewName, e);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_mapping_roleInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View(input);
                }
                var e = editMapper.MapToEntity(input, service.Get(input.ID));
                e.ClientID = ((PAIdentity)User.Identity).clientID;
                service.Save();
                service.getRepo().getDBContext().Entry(e).Reference(o => o.mstr_process_role).Load();
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return View(RowViewName, e);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult editRole(int id)
        {
            var entity = service.Get(id);
            if (entity == null) throw new PAException("Organisation Role does not exist anymore");
            return View("Edit", editMapper.MapToInput(entity));
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

    }
}

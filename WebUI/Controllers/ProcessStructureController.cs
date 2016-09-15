﻿using System.Web.Mvc;
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
    public class ProcessStructureController : Cruder<tbl_process_structure, tbl_process_structureInput>
    {
        public ProcessStructureController(ICrudService<tbl_process_structure> service, IMapper<tbl_process_structure, tbl_process_structureInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSSTR")
        {
            functionID = "DFPRSSTR";
        }

        public override ActionResult Create()
        {
            string level =  Request.QueryString["level"];
            string parent = Request.QueryString["parent"];


            if (short.Parse(level) == 0 || int.Parse(parent) < 0)
            {
                Response.StatusCode = 500;
                Response.Write("Invalid Request. Pl. contact the administrator");
                return null;
            }
            IMapper<tbl_process_structure, tbl_process_structureInput> cMapper = new Mapper<tbl_process_structure, tbl_process_structureInput>();
            tbl_process_structureInput tbl = cMapper.MapToInput(new tbl_process_structure());

            tbl.Level = short.Parse(level);
            if (parent == "0")
            {
                // Node created at the root.
                tbl.ParentRoleID = null;
                tbl.StructPath = "0";
                ViewBag.excludeRoles = tbl.StructPath;
                // Add to the exclude list of roles. No roles should be added twice of create a cyclic reference
                var excludeList = service.Where(rec => rec.Level == tbl.Level);
                foreach (var e in excludeList)
                {
                    ViewBag.excludeRoles = ViewBag.excludeRoles + "," + e.mstr_Process_RoleID.ToString();
                }
            }
            else
            {
                // This is a sub node
                tbl.ParentRoleID = int.Parse(parent);
                // Add to the struct path of the parent
                var parentEntity = service.Get(int.Parse(parent));
                tbl.StructPath = parentEntity.StructPath + "," + parentEntity.mstr_Process_RoleID.ToString();
                ViewBag.excludeRoles = tbl.StructPath;
                // Add to the exclude list of roles. No roles should be added twice of create a cyclic reference
                var excludeList = service.Where(rec => rec.ParentRoleID == tbl.ParentRoleID && rec.Level == tbl.Level);
                foreach (var e in excludeList)
                {
                    ViewBag.excludeRoles = ViewBag.excludeRoles + "," + e.mstr_Process_RoleID.ToString();
                }
            }
            tbl.IsParent = false;

            return View(tbl);
        }

        [HttpPost]
        public override ActionResult Create(tbl_process_structureInput input)
        {
            // This method is overridden here because the navigation property of entity object is not geting loaded
            // even without lazy loading. Rest of the code is same as Crudere implementation.
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;

                    ViewBag.excludeRoles = input.StructPath;
                    // Add to the exclude list of roles. No roles should be added twice of create a cyclic reference
                    var excludeList = service.Where(rec => rec.ParentRoleID == input.ParentRoleID && rec.Level == input.Level);
                    foreach (var node in excludeList)
                    {
                        ViewBag.excludeRoles = ViewBag.excludeRoles + "," + node.mstr_Process_RoleID.ToString();
                    }

                    return View("Create", input);
                }
                input.ClientID = ((PAIdentity)User.Identity).clientID;
                var id = service.Create(createMapper.MapToEntity(input, new tbl_process_structure()));
                var e = service.Get(id);
                // Somehow the navigation property (FK) is not getting loaded by EF. Hence explicitely load it.
                // This data is needed in the view to display the role name.
                service.getRepo().getDBContext().Entry(e).Reference(x => x.mstr_process_role).Load();
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
                service.getRepo().getDBContext().Entry(entity).Reference(x => x.mstr_process_role).Load();
                ViewBag.roleName = entity.mstr_process_role.LongName;
                return View("Edit", editMapper.MapToInput(entity));
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_process_structureInput input)
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
                
                var e = editMapper.MapToEntity(input, service.Get(input.ID));
                service.Save();
                // Somehow the navigation property (FK) is not getting loaded by EF. Hence explicitely load it.
                // This data is needed in the view to display the role name.
                service.getRepo().getDBContext().Entry(e).Reference(x => x.mstr_process_role).Load();
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return View(RowViewName, new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

    }
}

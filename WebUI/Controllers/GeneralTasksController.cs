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
    [InitializeSimpleMembership]
    public class GeneralTasksController : Cruder<mstr_org_general_tasks, mstr_org_general_tasksInput>
    {
        //
        // GET: /GeneralTask/
        public GeneralTasksController(GeneralTasksService service, org_general_tasksMapper v, IWorkflowService wf)
            : base(service, v, wf, "DFORGMTRGT")
        {
            functionID = "DFORGMTRGT";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_org_general_tasks o) { return o.Task; }

        [HttpPost]
        public override ActionResult Edit(mstr_org_general_tasksInput input)
        {
            try
            {
                if (!CheckAccess("Edit"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View(input);
                }
                if (checkForDuplicateEdit(input))
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", "Duplicate Entry Found");
                    return View(input);
                }
                ReSequenceBeforeEdit(input);
                var e = editMapper.MapToEntity(input, service.Get(input.ID));
                if (input.Global == true || input.Roles == null || !input.Roles.Any())
                {
                    foreach (var rl in e.tbl_org_general_task_roles.ToList())
                    {
                        e.tbl_org_general_task_roles.Remove(rl);
                        service.getRepo().getDBContext().Entry(rl).State = System.Data.Entity.EntityState.Deleted;
                    }
                }
                else
                {
                    foreach (var rl in e.tbl_org_general_task_roles.Where(o => o.ID > 0).ToList())
                    {
                        if (!input.Roles.Contains(rl.tbl_Org_RoleID.ToString()))
                        {
                            e.tbl_org_general_task_roles.Remove(rl);
                            service.getRepo().getDBContext().Entry(rl).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }
                e.ClientID = ((PAIdentity)User.Identity).clientID;
                service.Save();
                e = service.Get(input.ID);
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return ProcessView(new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        protected override void ReSequenceBeforeEdit(mstr_org_general_tasksInput input)
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.Sequence >= input.Sequence && o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(p => p.Sequence);
                if (restEntries.Any())
                {
                    short sequence = (short)(input.Sequence + 1);
                    foreach (var r in restEntries)
                    {
                        if (r.ID != input.ID)
                        {
                            r.Sequence = sequence;
                            sequence = (short)(sequence + 1);
                        }
                    }
                }
                service.Save();
            }
        }

        public virtual ActionResult getTasksFor(int selectedItem, string controlName, string reload, int projectID)
        {
            try
            {
                ViewBag.defaultItem = "Add New (Enter Task Below)";
                var ctx = (Db)service.getRepo().getDBContext();
                var selectedTasks = ctx.tbl_proj_general_tasks.Where(o => o.tbl_Org_ProjectID == projectID).Select(o => o.tbl_Org_General_TaskID);
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && 
                                                                !selectedTasks.Contains(rec.ID) &&
                                                                (rec.OwnedByProject == null || rec.OwnedByProject == projectID));

                var returnList = orderList(list).ToList().Select(node => new SelectListItem
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

        public ActionResult PublishToAll(int id)
        {
            try
            {
                var entity = service.Get(id);
                if (entity == null)
                {
                    Response.StatusCode = 500;
                    return Content("This entry does not exist");
                }
                if (entity.OwnedByProject == null)
                {
                    Response.StatusCode = 500;
                    return Content("This task is already accessible to all projects. No action taken");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    service.getRepo().executeStoredCommand("Update tbl_proj_general_tasks set Tailored = null where tbl_Org_General_TaskID = " + id);
                    service.getRepo().executeStoredCommand("Update mstr_org_general_tasks set OwnedByProject = null where ID = " + id);
                    scope.Complete();
                }
                ((Db)service.getRepo().getDBContext()).Entry(entity).Reload();
                return View("GetItems", new[] { entity });
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Content("System Error : " + e.Message);
            }
        }

        protected override void InitiazeSequence(mstr_org_general_tasksInput input)
        {
            var seq = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (seq.Any()) { input.Sequence = seq.Max(o => o.Sequence); input.Sequence++; } else { input.Sequence = 1; }
        }


        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                if (!CheckAccess("Delete"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                // Delete dependent records first
                using (TransactionScope scope = new TransactionScope())
                {
                    service.getRepo().executeStoredCommand("delete from tbl_org_general_task_roles where tbl_Org_General_TaskID = " + id);
                    service.Delete(id);
                    service.Save();
                    scope.Complete();
                }
                return Json(new { Id = id, Type = typeof(mstr_org_general_tasks).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}

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
    public class PrjGTTailorController : Cruder<tbl_proj_general_tasks, tbl_proj_general_tasksInput>
    {
        //
        // GET: /GeneralTask/
        public PrjGTTailorController(GTTailorService service, proj_GTTailorMapper v, IWorkflowService wf)
            : base(service, v, wf, "PLPTLRGT")
        {
            functionID = "PLPTLRGT";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_proj_general_tasks o) { return o.mstr_org_general_tasks.Task; }

        [HttpPost]
        public override ActionResult Edit(tbl_proj_general_tasksInput input)
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
                if (input.Global == true)
                {
                    foreach (var rl in e.tbl_proj_general_task_roles.ToList())
                    {
                        e.tbl_proj_general_task_roles.Remove(rl);
                        service.getRepo().getDBContext().Entry(rl).State = System.Data.Entity.EntityState.Deleted;
                    }
                }
                else
                {
                    foreach (var rl in e.tbl_proj_general_task_roles.Where(o => o.ID > 0).ToList())
                    {
                        if (!input.Roles.Contains(rl.tbl_Org_RoleID.ToString()))
                        {
                            e.tbl_proj_general_task_roles.Remove(rl);
                            service.getRepo().getDBContext().Entry(rl).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }
                e.ClientID = ((PAIdentity)User.Identity).clientID;
                service.Save();

                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                return ProcessView(service.Where(o => o.ID == e.ID));
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public override ActionResult Create()
        {
            try
            {
                if (Request.QueryString["value"] == null || Request.QueryString["text"] == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Pl. select a project before invoking this action";
                    return View("ListItems/showError");
                }
                var input = new tbl_proj_general_tasksInput();
                input.tbl_Org_ProjectID = int.Parse(Request.QueryString["value"]);
                input.ProjectName = Request.QueryString["text"];
                InitiazeSequence(input);

                if (CheckAccess("Create"))
                {
                    // Check for any workflow 
                    var user = ((PAIdentity)User.Identity);
                    var wf = wrkflw.getFunctionStatus(functionID, null, null, user.IsAdmin(), null, null, user.clientID.GetValueOrDefault());
                    if (wf.Any())
                    {
                        ViewBag.WF = true;
                        ViewBag.workflow = wf;
                    }
                    return View(input);
                }
                else
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        protected override bool checkForDuplication(tbl_proj_general_tasksInput input) 
        {
            var ctx = (Db)service.getRepo().getDBContext();
            if (input.tbl_Org_General_TaskID == 0)
            {
                // This is a new entry, check whether any GT by this name exists
                var gt = ctx.mstr_org_general_tasks.Where(o => o.Task.ToUpper().Equals(input.GeneralTask.ToUpper()));
                if (gt.Any()) return true;
                // Else create this new entry in org master and replace the ID
                var gtMaster = ctx.mstr_org_general_tasks.Add(new mstr_org_general_tasks()
                {
                    Task = input.GeneralTask,
                    Sequence = input.Sequence,
                    OwnedByProject = input.tbl_Org_ProjectID,
                    Global = input.Global,
                    Description = input.GeneralTask,
                    ClientID = ((PAIdentity)User.Identity).clientID
                });
                // Check the roles to be associated with this record
                if (input.Global != true)
                {
                    if (input.Roles.Any())
                    {
                        gtMaster.tbl_org_general_task_roles = new List<tbl_org_general_task_roles>();
                        foreach (var rl in input.Roles)
                        {
                            gtMaster.tbl_org_general_task_roles.Add(new tbl_org_general_task_roles()
                            {
                                tbl_Org_General_TaskID = gtMaster.ID,
                                tbl_Org_RoleID = int.Parse(rl),
                                ClientID = gtMaster.ClientID
                            });
                        }
                    }
                }
                ctx.SaveChanges();
                input.tbl_Org_General_TaskID = gtMaster.ID;  // Assign the new task id
                input.Tailored = true;
            }
            else
            {
                var gt = ctx.tbl_proj_general_tasks.Where(o => o.ID == input.tbl_Org_General_TaskID && o.tbl_Org_ProjectID == input.tbl_Org_ProjectID);
                if (gt.Any()) return true;
            }
            return false; 
        }

        protected override void ReSequenceBeforeEdit(tbl_proj_general_tasksInput input)
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

        protected override bool checkForDuplicateEdit(tbl_proj_general_tasksInput input) 
        {
            var ctx = (Db)service.getRepo().getDBContext();
            
            if (input.Tailored == true)
            {
                var gtMaster = ctx.mstr_org_general_tasks.Where(o => o.Task.ToUpper().Equals(input.GeneralTask.ToUpper()) && o.ID != input.tbl_Org_General_TaskID);
                if (gtMaster.Any()) return true;
                var gtMasterChange = ctx.mstr_org_general_tasks.Where(o => o.ID == input.tbl_Org_General_TaskID).SingleOrDefault();
                if (gtMasterChange == null) return true;
                gtMasterChange.Task = input.GeneralTask;
                gtMasterChange.Description = input.GeneralTask;
                ctx.SaveChanges();
            }
            return false; 
        }

        public override ActionResult ReloadItems()
        {
            int filter = 0;

            if (Request.QueryString["filter"] != null && Request.QueryString["filter"] != "") filter = int.Parse(Request.QueryString["filter"]);

            var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.tbl_Org_ProjectID == filter);

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        protected override void InitiazeSequence(tbl_proj_general_tasksInput input)
        {
            var seq = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (seq.Any())
                input.Sequence = (short)(seq.Max(o => o.Sequence) + 1);
            else input.Sequence = 1;
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
                    service.getRepo().executeStoredCommand("delete from tbl_proj_general_task_roles where tbl_Proj_General_TaskID = " + id);
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

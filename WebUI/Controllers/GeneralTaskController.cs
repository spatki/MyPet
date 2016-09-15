using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class GeneralTaskController : Cruder<tbl_process_general_task, tbl_process_general_taskInput>
    {
        //
        // GET: /GeneralTask/
        public GeneralTaskController(ICrudService<tbl_process_general_task> service, IMapper<tbl_process_general_task, tbl_process_general_taskInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSCNFG")
        {
            functionID = "DFPRSCNFG";
        }

        public ActionResult UpdateTasks(List<tbl_process_general_taskInput> input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("showTasks", input);
                }
                ViewBag.repoID = 9;
                var repoID = 9;
                if (input.Any())
                {
                    var entity = new tbl_process_general_task();
                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (var g in input)
                        {
                            entity = service.Get(g.ID);
                            if (entity == null)
                            {
                                entity = createMapper.MapToEntity(g, new tbl_process_general_task());
                                service.Create(entity);
                            }
                            else
                            {
                                entity = editMapper.MapToEntity(g, entity);
                                service.Save();
                            }
                        }
                        scope.Complete();
                    }

                }
                return View(RowViewName, service.Where(o => o.tbl_Process_RepositoryID == repoID));
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult manageTasksFor(int processId)
        {
            if (processId == 0) {
                Response.StatusCode = 500;
                Response.StatusDescription = "Repository not found";
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                IEnumerable<tbl_process_general_task> tasks = service.Where(t => t.tbl_Process_RepositoryID == processId && t.ClientID == ((PAIdentity)User.Identity).clientID).AsEnumerable();
                var input = new List<tbl_process_general_taskInput>();

                if (tasks.Any()) {
                    foreach (var g in tasks)
                    {
                        input.Add(editMapper.MapToInput(g));
                    }
                }
                ViewBag.mode = "edit";
                return View("showTasks",input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Json(new { Content = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_process_general_task o) { return o.Name; }

        protected override void InitiazeSequence(tbl_process_general_taskInput input)
        {
            var seq = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (seq.Any()) { input.SequenceNo = seq.Max(o => o.SequenceNo); input.SequenceNo++; } else { input.SequenceNo = 1; }
        }

    }
}

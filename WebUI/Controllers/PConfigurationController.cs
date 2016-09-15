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
    public class PConfigurationController : Cruder<tbl_process_repository, tbl_process_repositoryInput>
    {
        public PConfigurationController(ICrudService<tbl_process_repository> service, IMapper<tbl_process_repository, tbl_process_repositoryInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSCNFG")
        {
            functionID = "DFPRSCNFG";
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
            IMapper<tbl_process_repository, tbl_process_repositoryInput> cMapper = new Mapper<tbl_process_repository, tbl_process_repositoryInput>();
            tbl_process_repositoryInput tbl = cMapper.MapToInput(new tbl_process_repository() { ClientID = ((PAIdentity)User.Identity).clientID });

            tbl.Level = short.Parse(level);
            Db dbc = (Db)service.getRepo().getDBContext();
            var selectLevels = dbc.mstr_process_level.Where(o => o.LevelSequence == tbl.Level);
            if (selectLevels.Any())
            {
                foreach (var l in selectLevels)
                {
                    ViewBag.includeLevel = ViewBag.includeLevel + "," + l.ID;
                }
            }
            else
            {
                ViewBag.includeLevel = "0";
            }
            if (parent == "0")
            {
                // Node created at the root.
                tbl.ParentID = null;
                tbl.StructPath = "0";
            }
            else
            {
                // This is a sub node
                tbl.ParentID = int.Parse(parent);
                // Add to the struct path of the parent
                var parentEntity = service.Get(int.Parse(parent));
                tbl.StructPath = parentEntity.StructPath + "," + parentEntity.ID.ToString();
            }
            tbl.IsParent = false;
            InitiazeSequence(tbl);
            return View(tbl);
        }

        [HttpPost]
        public override ActionResult Create(tbl_process_repositoryInput input)
        {
            // This method is overridden here because the navigation property of entity object is not geting loaded
            // even without lazy loading. Rest of the code is same as Crudere implementation.
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("Create", input);
                }
                input.ClientID = ((PAIdentity)User.Identity).clientID;                  // Add the client code
                ReSequenceBeforeCreate(input);
                var id = service.Create(createMapper.MapToEntity(input, new tbl_process_repository()));
                var e = service.Get(id);
                var ctx = service.getRepo().getDBContext();
                ctx.Entry(e).Reference(x => x.mstr_process_level).Load();
                //ctx.Entry(e).Collection(x => x.tbl_process_general_task).Load();
                ctx.Entry(e).Collection(x => x.tbl_process_rep_chklst).Load();
                ctx.Entry(e).Collection(x => x.tbl_process_rep_document).Load();
                ctx.Entry(e).Collection(x => x.tbl_process_rep_procedure).Load();
                ctx.Entry(e).Collection(x => x.tbl_process_rep_task).Load();
                ctx.Entry(e).Collection(x => x.tbl_process_rep_template).Load();
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
                service.getRepo().getDBContext().Entry(entity).Reference(x => x.mstr_process_level).Load();
                ViewBag.levelName = entity.mstr_process_level.LongName;
                return View("Edit", editMapper.MapToInput(entity));
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_process_repositoryInput input)
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

                ReSequenceBeforeEdit(input);
                var e = editMapper.MapToEntity(input, service.Get(input.ID)); 
                service.Save();
                service.getRepo().getDBContext().Entry(e).Reference(o => o.mstr_process_level).Load();
                // Somehow the navigation property (FK) is not getting loaded by EF. Hence explicitely load it.
                // This data is needed in the view to display the role name.
                ViewBag.repoID = e.ID;
                ViewBag.lvl = e.mstr_process_level.LongName;
                ViewBag.lvlDesc = e.Name;
                ViewBag.IsActivity = (e.TreatAsTask == true ? "Y" : "N");
                ViewBag.Sequence = e.Sequence;
                return View("ChangedNode");
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

        public ActionResult getConfiguration()
        {
            try
            {
                var list = service.Where (o => o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(o => o.Level).ThenBy(o => o.Sequence);
                var ctx = service.getRepo().getDBContext();

                foreach (var c in list)
                {
                    ctx.Entry(c).Reference(p => p.mstr_process_level).Load();
                }
                var returnList = from node in list
                                 select new
                                 {
                                     ID = node.ID,
                                     Sequence = node.Sequence,
                                     levelID = node.mstr_Process_LevelID,
                                     levelName = node.mstr_process_level.LongName,
                                     nodeName = node.Name,
                                     Level = node.Level,
                                     ParentID = node.ParentID,
                                     IsActivity = (node.TreatAsTask == true ? "Y" : "N")
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult configureRules(int repoID, int activityID)
        {
            if (repoID == 0 && activityID == 0)
            {
                throw new PAException("Insufficient information");
            }
            return View();
        }

        public ActionResult getRepositoryForMapping()
        {
            try
            {
                var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(o => o.Level).OrderBy(o => o.Level).ThenBy(o => o.Sequence);
                List<returnData> returnList = new List<returnData>();

                var ctx = (Db) service.getRepo().getDBContext();

                foreach (var c in list)
                {
                    ctx.Entry(c).Reference(p => p.mstr_process_level).Load();
                    var tasks = ctx.tbl_process_rep_task.Where(o => o.tbl_Process_RepositoryID == c.ID);

                    returnList.Add(new returnData 
                                        {
                                            ID = c.ID,
                                            IsActivity = (c.TreatAsTask == true ? "Y" : "N"),
                                            levelID = c.mstr_Process_LevelID,
                                            levelName = c.mstr_process_level.LongName,
                                            nodeName = c.Name,
                                            Level = c.Level,
                                            ParentID = c.ParentID,
                                            accessID = "R" + c.ID,
                                            Type = "R",
                                            paramName = "repoID",
                                            paramValue = c.ID

                                        });
                    if (tasks.Any())
                    {
                        foreach (var t in tasks)
                        {
                            returnList.Add(new returnData 
                                                {
                                                    ID = c.ID,
                                                    IsActivity = "Y",
                                                    levelID = c.mstr_Process_LevelID,
                                                    levelName = "Activity",
                                                    nodeName = t.Name,
                                                    Level = c.Level + 1,
                                                    ParentID = c.ID,
                                                    accessID = "R" + c.ID + "A" + t.ID,
                                                    Type = "A",
                                                    paramName = "activityID",
                                                    paramValue = t.ID
                                                });
                        }
                    }
                }
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        [HttpPost]
        public ActionResult DeleteActivityDoc(int id)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_process_rep_task_ref_docs.Where(o => o.ID == id).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "This record does not exist";
                    return View("ListItems/showError");	// Return error in a dialog box
                }
                ctx.tbl_process_rep_task_ref_docs.Remove(entity);
                ctx.SaveChanges();
                return Json(new { Id = id, Type = typeof(tbl_process_rep_task_ref_docs).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult addSupportingDoc(int repoID, int taskID, int key)
        {
            ViewBag.repoID = repoID;
            ViewBag.taskID = taskID;
            ViewBag.key = key;
            ViewBag.clientID = ((PAIdentity)User.Identity).clientID;
            return View();
        }

        public ActionResult getRepository(int id)       // Get repository for a specific node
        {
            try
            {
                var repository = service.Get(id);
                var ctx = service.getRepo().getDBContext();

                ctx.Entry(repository).Reference(x => x.mstr_process_level).Load();
                ctx.Entry(repository).Collection(x => x.tbl_process_rep_document).Load();
                ctx.Entry(repository).Collection(x => x.tbl_process_rep_chklst).Load();
                ctx.Entry(repository).Collection(x => x.tbl_process_rep_procedure).Load();
                ctx.Entry(repository).Collection(x => x.tbl_process_rep_template).Load();
                ctx.Entry(repository).Collection(x => x.tbl_process_rep_task).Load();
                //ctx.Entry(repository).Collection(x => x.tbl_process_general_task).Load();
                
                List<repoData> returnList = new List<repoData>();

                // Add Process Artifacts
                returnList.Add(new repoData
                {
                    ID = repository.ID,
                    repoName = "Process Artifacts",
                    isHeader = true,
                    levelID = repository.mstr_Process_LevelID,
                    levelName = repository.mstr_process_level.LongName,
                    Level = repository.tbl_process_rep_procedure.Count(),    // Use this as a counter for Header items
                    ParentID = repository.ParentID,
                    itemName = "Proc" + repository.ID.ToString(),
                    itemID = 0,
                    itemKey = "/PConfiguration/manageProcedures?repoID=" + repository.ID.ToString(),
                    className = "openDialog"
                });     // Default entry

                if (repository.tbl_process_rep_procedure.Any())
                {
                    foreach (var p in repository.tbl_process_rep_procedure.Where(o => o.tbl_Process_Rep_ProcessID == null))
                    {
                        ctx.Entry(p).Reference(x => x.tbl_process_procedure).Load();
                        returnList.Add(new repoData
                        {
                            ID = repository.ID,
                            repoName = "Process Artifacts",
                            isHeader = false,
                            levelID = repository.mstr_Process_LevelID,
                            levelName = repository.mstr_process_level.LongName,
                            Level = repository.Level,
                            ParentID = repository.ParentID,
                            itemName = p.tbl_process_procedure.Name,
                            itemID = p.ID,
                            itemKey = "/PProcedure/showPreview?id=" + p.tbl_process_procedure.ID,
                            className = "label-configured openDialog"
                        });
                    }
                }

                
                // Add Templates
                returnList.Add(new repoData
                {
                    ID = repository.ID,
                    repoName = "Templates",
                    isHeader = true,
                    levelID = repository.mstr_Process_LevelID,
                    levelName = repository.mstr_process_level.LongName,
                    Level = repository.tbl_process_rep_template.Count(),    // Use this as a counter for Header items
                    ParentID = repository.ParentID,
                    itemName = "Template" + repository.ID.ToString(),
                    itemID = 0,
                    itemKey = "/PConfiguration/manageTemplates?repoID=" + repository.ID.ToString(),
                    className = "openDialog"
                });     // Default entry

                if (repository.tbl_process_rep_template.Any())
                {
                    foreach (var t in repository.tbl_process_rep_template.Where(o => o.tbl_Process_Rep_ProcessID == null))
                    {
                        ctx.Entry(t).Reference(x => x.tbl_process_template).Load();
                        returnList.Add(new repoData
                        {
                            ID = repository.ID,
                            repoName = "Templates",
                            isHeader = false,
                            levelID = repository.mstr_Process_LevelID,
                            levelName = repository.mstr_process_level.LongName,
                            Level = repository.Level,
                            ParentID = repository.ParentID,
                            itemName = t.tbl_process_template.Name,
                            itemID = t.ID,
                            itemKey = "/PTemplate/showPreview?id=" + t.tbl_process_template.ID,
                            className = "label-configured openDialog"
                        });
                    }
                }
                


                // Add Checklists
                returnList.Add(new repoData
                {
                    ID = repository.ID,
                    repoName = "Checklists",
                    isHeader = true,
                    levelID = repository.mstr_Process_LevelID,
                    levelName = repository.mstr_process_level.LongName,
                    Level = repository.tbl_process_rep_chklst.Count(),    // Use this as a counter for Header items
                    ParentID = repository.ParentID,
                    itemName = "Checklist" + repository.ID.ToString(),
                    itemID = 0,
                    itemKey = "/PConfiguration/manageChecklists?repoID=" + repository.ID.ToString(),
                    className = "openDialog"
                });     // Default entry

                if (repository.tbl_process_rep_chklst.Any())
                {
                    foreach (var c in repository.tbl_process_rep_chklst)
                    {
                        ctx.Entry(c).Reference(x => x.tbl_process_checklist).Load();

                        returnList.Add(new repoData
                        {
                            ID = repository.ID,
                            repoName = "Checklists",
                            isHeader = false,
                            levelID = repository.mstr_Process_LevelID,
                            levelName = repository.mstr_process_level.LongName,
                            Level = repository.Level,
                            ParentID = repository.ParentID,
                            itemName = c.tbl_process_checklist.Name,
                            itemID = c.ID,
                            itemKey = "/PChecklist/showPreview?id=" + c.tbl_process_checklist.ID,
                            className = "label-configured openDialog"
                        });
                    }

                }

                // Add Documents
                returnList.Add(new repoData
                {
                    ID = repository.ID,
                    repoName = "Documents",
                    isHeader = true,
                    levelID = repository.mstr_Process_LevelID,
                    levelName = repository.mstr_process_level.LongName,
                    Level = repository.tbl_process_rep_document.Count(),    // Use this as a counter for Header items
                    ParentID = repository.ParentID,
                    itemName = "Document" + repository.ID.ToString(),
                    itemID = 0,
                    itemKey = "/PConfiguration/manageDocuments?repoID=" + repository.ID.ToString(),
                    className = "openDialog"
                });     // Default entry

                if (repository.tbl_process_rep_document.Any())
                {
                    foreach (var c in repository.tbl_process_rep_document.Where(o => o.tbl_Process_Rep_TaskID == null))
                    {
                        ctx.Entry(c).Reference(x => x.tbl_process_document).Load();
                        ctx.Entry(c.tbl_process_document).Reference(x => x.tbl_docmgr_document).Load();


                        returnList.Add(new repoData
                        {
                            ID = repository.ID,
                            repoName = "Documents",
                            isHeader = false,
                            levelID = repository.mstr_Process_LevelID,
                            levelName = repository.mstr_process_level.LongName,
                            Level = repository.Level,
                            ParentID = repository.ParentID,
                            itemName = c.tbl_process_document.tbl_docmgr_document.Name,
                            itemID = (int) c.tbl_process_document.tbl_DocMgr_DocumentID,
                            itemKey = Url.Action("ViewDocument", "DocMgr", new { id = c.tbl_process_document.tbl_DocMgr_DocumentID }, Request.Url.Scheme),
                            className = "label-configured openDocument"
                        });
                    }

                }
                
                // Add Activities
                returnList.Add(new repoData
                {
                    ID = repository.ID,
                    repoName = "Activities",
                    isHeader = true,
                    levelID = repository.mstr_Process_LevelID,
                    levelName = repository.mstr_process_level.LongName,
                    Level = repository.tbl_process_rep_task.Count(),    // Use this as a counter for Header items
                    ParentID = repository.ParentID,
                    itemName = "Activity" + repository.ID.ToString(),
                    itemID = 0,
                    itemKey = "/PConfiguration/manageActivities?repoID=" + repository.ID.ToString(),
                    className = "openActivities"
                });     // Default entry

                if (repository.tbl_process_rep_task.Any())
                {
                    foreach (var d in repository.tbl_process_rep_task.OrderBy(o => o.SequenceNo))
                    {
                        returnList.Add(new repoData
                        {
                            ID = repository.ID,
                            repoName = "Activities",
                            isHeader = false,
                            levelID = repository.mstr_Process_LevelID,
                            levelName = repository.mstr_process_level.LongName,
                            Level = repository.Level,
                            ParentID = repository.ParentID,
                            itemName = d.Name,
                            itemID = d.ID,
                            itemKey = "/PConfiguration/manageActivities?repoID=" + repository.ID.ToString() + "&id=" + d.ID.ToString(),
                            className = "label-configured openActivities"
                        });
                    }
                }

                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult getTaskRepository(int repoID, int taskID)       // Get repository for a specific node
        {
            try
            {

            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult manageProcedures(int repoID, int? taskID)
        {
            try
            {
                tbl_process_rep_details input = new tbl_process_rep_details();

                var e = service.Get(repoID);
                if (e == null) throw new PAException("this entity doesn't exist anymore");
                var ctx = (Db) service.getRepo().getDBContext();
                var procs = ctx.tbl_process_rep_procedure.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Process_RepositoryID == repoID
                                                                && ((o.tbl_Process_Rep_ProcessID == taskID) || (taskID == null && o.tbl_Process_Rep_ProcessID == null)));
                input.ID = repoID;
                input.key = repoID.ToString();
                if (procs.Any())
                {
                    foreach (var g in procs)
                    {
                        input.selectedOptions.Add((int)g.tbl_Process_ProcedureID);
                    }
                }
                ViewBag.postURL = Url.Action("manageProcedures","PConfiguration");
                return View("showProcedures", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }            
        }

        [HttpPost]
        public ActionResult manageProcedures(tbl_process_rep_details input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    ViewBag.postURL = Url.Action("manageProcedures","PConfiguration");
                    return View("showProcedures", input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var e = ctx.tbl_process_repository.Include("tbl_process_rep_procedure").Where(o => o.ID == input.ID).SingleOrDefault();
                if (e == null) throw new PAException("this entity doesn't exist anymore");

                using (TransactionScope scope = new TransactionScope())
                {
                    if (input.selectedOptions != null && input.selectedOptions.Any())
                    {
                        int sequence;
                        if (e.tbl_process_rep_procedure.Any())
                            sequence = e.tbl_process_rep_procedure.Max(o => o.ID) + 1;
                        else
                            sequence = 1;    // Get the next ID
                        foreach (var g in input.selectedOptions)
                        {
                            if (!(e.tbl_process_rep_procedure.Where(o => o.tbl_Process_ProcedureID == g).Any()))
                            {
                                // Add this record
                                e.tbl_process_rep_procedure.Add(new tbl_process_rep_procedure()
                                {
                                    ID = sequence,
                                    ClientID = ((PAIdentity)User.Identity).clientID,
                                    tbl_Process_RepositoryID = input.ID,
                                    tbl_Process_ProcedureID = g
                                });
                                sequence++;
                            }
                        }
                        // check if any of the IDs where deleted
                        foreach (var t in e.tbl_process_rep_procedure.ToList())
                        {
                            if (!(input.selectedOptions.Contains(t.tbl_Process_ProcedureID.GetValueOrDefault())))
                            {
                                // This procedure needs to be deleted
                                service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_ProcID = " + t.ID); // delete references for activities if any
                                e.tbl_process_rep_procedure.Remove(t);
                                ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                            }
                        }
                    }
                    else
                    {
                        // As there are no procedures selected, delete all procedures associated with this repository
                        foreach (var t in e.tbl_process_rep_procedure.ToList())
                        {
                            service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_ProcID = " + t.ID); // delete references for activities if any
                            e.tbl_process_rep_procedure.Remove(t);
                            ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                        }

                    }
                    service.Save();
                    scope.Complete();
                }
                // Load procedure details
                if (e.tbl_process_rep_procedure.Any())
                {
                    foreach (var t in e.tbl_process_rep_procedure)
                    {
                        ctx.Entry(t).Reference(o => o.tbl_process_procedure).Load();
                    }
                }
                return View("getProcedures", e);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult manageTemplates(int repoID, int? taskID)
        {
            try
            {
                tbl_process_rep_details input = new tbl_process_rep_details();

                var e = service.Get(repoID);
                if (e == null) throw new PAException("this entity doesn't exist anymore");
                var ctx = (Db)service.getRepo().getDBContext();

                var tmpls = ctx.tbl_process_rep_template.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Process_RepositoryID == repoID
                                                                                 && ((o.tbl_Process_Rep_ProcessID == taskID) || (taskID == null && o.tbl_Process_Rep_ProcessID == null)));

                input.ID = repoID;
                input.key = repoID.ToString();
                if (tmpls.Any())
                {
                    foreach (var g in tmpls)
                    {
                        input.selectedOptions.Add((int)g.tbl_Process_TemplateID);
                    }
                }
                ViewBag.postURL = Url.Action("manageTemplates", "PConfiguration");
                return View("showTemplates", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult manageTemplates(tbl_process_rep_details input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    ViewBag.postURL = Url.Action("manageTemplates", "PConfiguration");
                    return View("showTemplates", input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var e = ctx.tbl_process_repository.Include("tbl_process_rep_template").Where(o => o.ID == input.ID).SingleOrDefault();
                if (e == null) throw new PAException("this entity doesn't exist anymore");

                using (TransactionScope scope = new TransactionScope())
                {
                    if (input.selectedOptions != null && input.selectedOptions.Any())
                    {
                        int sequence;
                        if (e.tbl_process_rep_template.Any())
                            sequence = e.tbl_process_rep_template.Max(o => o.ID) + 1;
                        else
                            sequence = 1;    // Get the next ID
                        foreach (var g in input.selectedOptions)
                        {
                            if (!(e.tbl_process_rep_template.Where(o => o.tbl_Process_TemplateID == g).Any()))
                            {
                                // Add this record
                                e.tbl_process_rep_template.Add(new tbl_process_rep_template()
                                {
                                    ID = sequence,
                                    ClientID = ((PAIdentity)User.Identity).clientID,
                                    tbl_Process_RepositoryID = input.ID,
                                    tbl_Process_TemplateID = g
                                });
                                sequence++;
                            }
                        }
                        // check if any of the IDs where deleted
                        foreach (var t in e.tbl_process_rep_template.ToList())
                        {
                            if (!(input.selectedOptions.Contains(t.tbl_Process_TemplateID.GetValueOrDefault())))
                            {
                                // This template needs to be deleted
                                service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_ProcID = " + t.ID); // delete references for activities if any
                                e.tbl_process_rep_template.Remove(t);
                                ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                            }
                        }
                    }
                    else
                    {
                        // As there are no templates selected, delete all templates associated with this repository
                        foreach (var t in e.tbl_process_rep_template.ToList())
                        {
                            service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_ProcID = " + t.ID); // delete references for activities if any
                            e.tbl_process_rep_template.Remove(t);
                            ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                        }

                    }
                    service.Save();
                    scope.Complete();
                }
                // Load template details
                if (e.tbl_process_rep_template.Any())
                {
                    foreach (var t in e.tbl_process_rep_template)
                    {
                        ctx.Entry(t).Reference(o => o.tbl_process_template).Load();
                    }
                }
                return View("getTemplates", e);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult manageChecklists(int repoID, int? taskID)
        {
            try
            {
                tbl_process_rep_details input = new tbl_process_rep_details();

                var e = service.Get(repoID);
                if (e == null) throw new PAException("this entity doesn't exist anymore");
                var ctx = (Db)service.getRepo().getDBContext();
                var chkLsts = ctx.tbl_process_rep_chklst.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Process_RepositoryID == repoID
                                                               && ((taskID == null && o.tbl_Process_Rep_ProcessID == null) || (o.tbl_Process_Rep_ProcessID == taskID)));
                input.ID = repoID;
                input.key = repoID.ToString();
                if (chkLsts.Any())
                {
                    foreach (var g in chkLsts)
                    {
                        input.selectedOptions.Add((int)g.tbl_Process_ChecklistID);
                    }
                }
                ViewBag.postURL = Url.Action("manageChecklists", "PConfiguration");
                return View("showChecklists", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult manageChecklists(tbl_process_rep_details input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("showProcedures", input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var e = ctx.tbl_process_repository.Include("tbl_process_rep_chklst").Where(o => o.ID == input.ID).SingleOrDefault();
                if (e == null) throw new PAException("this entity doesn't exist anymore");

                using (TransactionScope scope = new TransactionScope())
                {
                    if (input.selectedOptions != null && input.selectedOptions.Any())
                    {
                        int sequence;
                        if (e.tbl_process_rep_chklst.Any())
                            sequence = e.tbl_process_rep_chklst.Max(o => o.ID) + 1;
                        else
                            sequence = 1; // Get the next ID
                        foreach (var g in input.selectedOptions)
                        {
                            if (!(e.tbl_process_rep_chklst.Where(o => o.tbl_Process_ChecklistID == g).Any()))
                            {
                                // Add this record
                                e.tbl_process_rep_chklst.Add(new tbl_process_rep_chklst()
                                {
                                    ID = sequence,
                                    ClientID = ((PAIdentity)User.Identity).clientID,
                                    tbl_Process_RepositoryID = input.ID,
                                    tbl_Process_ChecklistID = g
                                });
                                sequence++;
                            }
                        }
                        // check if any of the IDs where deleted
                        foreach (var t in e.tbl_process_rep_chklst.ToList())
                        {
                            if (!(input.selectedOptions.Contains(t.tbl_Process_ChecklistID)))
                            {
                                // This checklist needs to be deleted
                                service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_ChklstID = " + t.ID); // delete references for activities if any
                                e.tbl_process_rep_chklst.Remove(t);
                                ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                            }
                        }
                    }
                    else
                    {
                        // As there are no checklists selected, delete all templates associated with this repository
                        foreach (var t in e.tbl_process_rep_chklst.ToList())
                        {
                            service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_ChklstID = " + t.ID); // delete references for activities if any
                            e.tbl_process_rep_chklst.Remove(t);
                            ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                    service.Save();
                    scope.Complete();
                }
                // Load checklist details
                if (e.tbl_process_rep_chklst.Any())
                {
                    foreach (var t in e.tbl_process_rep_chklst)
                    {
                        ctx.Entry(t).Reference(o => o.tbl_process_checklist).Load();
                    }
                }
                return View("getChecklists", e);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }


        public ActionResult manageGeneralTasks(int repoID)
        {
            try
            {
                tbl_process_rep_generaltasks input = new tbl_process_rep_generaltasks();
                tbl_process_general_taskMapper gtMapper  = new tbl_process_general_taskMapper();

                var e = service.Get(repoID);
                if (e == null) throw new PAException("this entity doesn't exist anymore");
                service.getRepo().getDBContext().Entry(e).Collection(o => o.tbl_process_general_task).Load();

                input.ID = repoID;
                input.key = repoID.ToString();
                if (e.tbl_process_general_task.Any())
                {
                    foreach (var g in e.tbl_process_general_task)
                    {
                        g.UpdateDate = System.DateTime.Now.Date;
                        g.UpdatedBy = ((PAIdentity)User.Identity).clientID;
                        input.tbl_process_general_task.Add(gtMapper.MapToInput(g));
                    }
                }
                ViewBag.PostURL = Url.Action("manageGeneralTasks", "PConfiguration");
                ViewBag.deleteURL = Url.Action("deleteGeneralTask", "PConfiguration");
                return View("showTasks", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult manageGeneralTasks(tbl_process_rep_generaltasks input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    ViewBag.PostURL = Url.Action("manageGeneralTasks", "PConfiguration");
                    ViewBag.deleteURL = Url.Action("deleteGeneralTask", "PConfiguration");
                    return View("showTasks", input);
                }
                if (input.tbl_process_general_task.Any())
                {
                    var taskMapper = new tbl_process_general_taskMapper();
                    var ctx = (Db)service.getRepo().getDBContext();
                    tbl_process_general_task entity;

                    foreach (var g in input.tbl_process_general_task)
                    {
                        entity = ctx.tbl_process_general_task.Where(o => o.ID == g.ID && o.tbl_Process_RepositoryID == input.ID).SingleOrDefault();
                        if (entity == null)
                        {
                            entity = ctx.tbl_process_general_task.Add(taskMapper.MapToEntity(g, new tbl_process_general_task())); // Add new record
                        }
                        else
                        {
                            entity = taskMapper.MapToEntity(g, entity);     // Edit existing record
                        }
                        ctx.SaveChanges();
                    }
                }
                return View("GetGeneralTasks", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult deleteGeneralTask(int id, int repoID)
        {
            try
            {
                var ctx = (Db) service.getRepo().getDBContext();

                var gt = ctx.tbl_process_general_task.Where(o => o.ID == id && o.tbl_Process_RepositoryID == repoID).SingleOrDefault();

                ctx.tbl_process_general_task.Remove(gt);
                ctx.SaveChanges();

                var input = new tbl_process_rep_generaltasks();
                tbl_process_general_taskMapper gtMapper = new tbl_process_general_taskMapper();
                var updatedGT = ctx.tbl_process_general_task.Where(o => o.tbl_Process_RepositoryID == repoID);

                if (updatedGT.Any())
                {
                    foreach (var g in updatedGT)
                    {
                        input.tbl_process_general_task.Add(gtMapper.MapToInput(g));
                    }
                }
                input.ID = repoID;
                return View("GetGeneralTasks", input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult deleteActivity(int id, int repoID)
        {
            try
            {
                var tblObj = service.getRepo();

                tblObj.executeStoredCommand("delete from tbl_process_rep_task where ID = " + id.ToString() + " and tbl_Process_RepositoryID = " + repoID.ToString());
                var entity = service.Get(repoID);
                if (entity == null) throw new PAException("System Error: Repository not found");
                service.getRepo().getDBContext().Entry(entity).Collection(o => o.tbl_process_rep_task).Load();
                return View("getActivities", entity);
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult manageActivities(int repoID, int? id)
        {
            try
            {
                var e = service.Get(repoID);
                if (e == null) throw new PAException("this entity doesn't exist anymore");
                var taskMapper = new Mapper<tbl_process_rep_task, tbl_process_rep_taskInput>();
                var tasksAssigned = new tbl_process_rep_taskdetails();

                service.getRepo().getDBContext().Entry(e).Collection(o => o.tbl_process_rep_task).Load();
                tasksAssigned.ID = repoID;  // Pass the repository ID
                // Get the node position
                if (e.StructPath == "0")
                {
                    tasksAssigned.position.Add(e.Name);
                }
                else
                {
                    var IDs = e.StructPath.Split(',').Select(str => int.Parse(str));
                    var path = service.Where(o => IDs.Contains(o.ID)).OrderBy(o => o.Level);
                    foreach (var n in path)
                    {
                        tasksAssigned.position.Add(n.Name);
                    }
                    tasksAssigned.position.Add(e.Name);
                }
                if (!e.tbl_process_rep_task.Any())
                {
                    tasksAssigned.selectedOptions.Add(new tbl_process_rep_taskInput() 
                            { tbl_Process_RepositoryID = repoID, 
                                ID = 0, 
                                SequenceNo = 1 });
                    ViewBag.sequence = 2;
                }
                else
                {
                    ViewBag.sequence = e.tbl_process_rep_task.Max(o => o.SequenceNo) + 1;
                    IEnumerable<tbl_process_rep_task> activity;
                    if (id != null)
                    {
                        activity = e.tbl_process_rep_task.Where(k => k.ID == id);
                    }
                    else
                    {
                        activity = e.tbl_process_rep_task;
                    }
                    foreach (var t in activity)
                    {
                        tasksAssigned.selectedOptions.Add(taskMapper.MapToInput(t));
                    }
                }
                return View("showActivities", tasksAssigned);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult manageActivities(tbl_process_rep_taskdetails input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("showActivities", input);
                }

                var e = service.Get(input.ID);
                if (e == null) throw new PAException("this entity doesn't exist anymore");
                // Get the node position
                service.getRepo().getDBContext().Entry(e).Collection(o => o.tbl_process_rep_task).Load();

                var taskMapper = new Mapper<tbl_process_rep_task, tbl_process_rep_taskInput>();
                tbl_process_rep_taskInput reArrangeEntity = null;
                    using (TransactionScope scope = new TransactionScope())
                    {
                        tbl_process_rep_task record;

                        if (input.selectedOptions.Any())
                        {
                            //short sequence = 0;
                            foreach (var a in input.selectedOptions)
                            {
                                if (a.reSequence != null && a.reSequence == true)
                                {
                                    // Re-sequencing has been requested by the user
                                    reArrangeEntity = a;
                                }
                                record = e.tbl_process_rep_task.Where(o => o.ID == a.ID).SingleOrDefault();
                                if (record == null) // This is a new record
                                {
                                    a.ClientID = e.ClientID;
                                    a.tbl_Process_RepositoryID = e.ID;
                                    e.tbl_process_rep_task.Add(taskMapper.MapToEntity(a, new tbl_process_rep_task()));   // Add new task
                                }
                                else
                                {
                                    a.ClientID = e.ClientID;
                                    record = taskMapper.MapToEntity(a, record);  // Modify existing record
                                }
                            }
                        }
                        if (reArrangeEntity != null)    // Re-arranging sequences
                        {
                            var sequence = (short)(reArrangeEntity.SequenceNo + 1);
                            foreach (var tsk in e.tbl_process_rep_task.Where(l => l.SequenceNo >= reArrangeEntity.SequenceNo && l.ID != reArrangeEntity.ID).OrderBy(k => k.SequenceNo).ThenBy(i => i.ID))
                            {
                                tsk.SequenceNo = sequence;
                                sequence += 1;
                            }
                        }
                        service.Save();
                        scope.Complete();
                    }
                return View("getActivities", e);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult manageDocuments(int repoID, int? taskID)
        {
            try
            {
                tbl_process_rep_details input = new tbl_process_rep_details();

                var e = service.Get(repoID);
                if (e == null) throw new PAException("this entity doesn't exist anymore");
                var ctx = (Db)service.getRepo().getDBContext();
                var docs = ctx.tbl_process_rep_document.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Process_RepositoryID == repoID
                                                            && ((o.tbl_Process_Rep_TaskID == taskID) || (taskID == null && o.tbl_Process_Rep_TaskID == null)));

                input.ID = repoID;
                input.key = repoID.ToString();
                if (docs.Any())
                {
                    foreach (var g in docs)
                    {
                        ctx.Entry(g).Reference(o => o.tbl_process_document).Load();
                        input.selectedOptions.Add((int)g.tbl_process_document.tbl_DocMgr_DocumentID);  // Review and change
                    }
                }
                ViewBag.CalledFrom = 1;
                return View("showDocuments", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult manageDocuments(tbl_process_rep_details input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("showDocuments", input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var e = ctx.tbl_process_repository.Include("tbl_process_rep_document")
                                                  .Include("tbl_process_rep_document.tbl_process_document")
                                                  .Include("tbl_process_rep_document.tbl_process_document.tbl_docmgr_document")
                                                  .Where(o => o.ID == input.ID).SingleOrDefault();
                if (e == null) throw new PAException("this entity doesn't exist anymore");

                using (TransactionScope scope = new TransactionScope())
                {
                    if (input.selectedOptions != null && input.selectedOptions.Any())
                    {
                        int sequence;
                        if (e.tbl_process_rep_document.Any())
                            sequence = e.tbl_process_rep_document.Max(o => o.ID) + 1;
                        else
                            sequence = 1;    // Get the next ID
                        foreach (var g in input.selectedOptions)
                        {
                            if (!(e.tbl_process_rep_document.Where(o => o.tbl_process_document != null && o.tbl_process_document.tbl_DocMgr_DocumentID == g).Any()))
                            {
                                var prsDoc = ctx.tbl_process_document.Where(o => o.tbl_DocMgr_DocumentID == g).SingleOrDefault();
                                if (prsDoc != null)
                                {
                                    // Add this record
                                    e.tbl_process_rep_document.Add(new tbl_process_rep_document()
                                    {
                                        ID = sequence,
                                        ClientID = ((PAIdentity)User.Identity).clientID,
                                        tbl_Process_RepositoryID = input.ID,
                                        tbl_Process_DocumentID = prsDoc.ID
                                    });
                                    sequence++;
                                }
                            }
                        }
                        // check if any of the IDs where deleted
                        foreach (var t in e.tbl_process_rep_document.ToList())
                        {
                            if (t.tbl_process_document != null && !(input.selectedOptions.Contains(t.tbl_process_document.tbl_DocMgr_DocumentID.GetValueOrDefault())))
                            {
                                // This template needs to be deleted
                                service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_DocumentID = " + t.ID); // delete references for activities if any
                                e.tbl_process_rep_document.Remove(t);
                                ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                            }
                        }
                    }
                    else
                    {
                        // As there are no templates selected, delete all templates associated with this repository
                        foreach (var t in e.tbl_process_rep_document.ToList())
                        {
                            service.getRepo().executeStoredCommand("delete from tbl_process_rep_task_ref_docs where tbl_Process_Repository_ID = " + e.ID + " and tbl_Process_Rep_DocumentID = " + t.ID); // delete references for activities if any
                            e.tbl_process_rep_document.Remove(t);
                            ctx.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                    service.Save();
                    scope.Complete();
                }
                // Load document details
                if (e.tbl_process_rep_document.Any())
                {
                    foreach (var t in e.tbl_process_rep_document)
                    {
                        ctx.Entry(t).Reference(o => o.tbl_process_document).Load();
                        ctx.Entry(t.tbl_process_document).Reference(o => o.tbl_docmgr_document).Load();
                    }
                }
                return View("getDocuments", e);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult addNewCheck(int id, int repoID, int sequence)
        {
            if (id == 0 || repoID == 0) throw new PAException("this entity doesn't exist anymore");
            ViewBag.gtID = id;
            ViewBag.repoID = repoID;
            ViewBag.sequence = sequence;
            ViewBag.clientID = ((PAIdentity)User.Identity).clientID;
            return View("NewCheck");
        }

        public ActionResult addNewActivity(int id, int repoID, int sequence)
        {
            if (id == 0 || repoID == 0) throw new PAException("this entity doesn't exist anymore");
            ViewBag.ID = id.ToString();
            ViewBag.repoID = repoID;
            ViewBag.sequence = sequence;
            ViewBag.clientID = ((PAIdentity)User.Identity).clientID;
            return View("NewActivity");
        }

        public ActionResult getRepositoryDocs(int repoID,int selectedItem, string controlName, string excludeIds)
        {
            try
            {
                IEnumerable<string> exclude = new[] { "0" };
                tbl_process_repository entity;
                var ctx = (Db) service.getRepo().getDBContext();

                if (repoID == null | repoID == 0)
                {
                    throw new PAException("Insufficient Input");
                }

                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',');
                }

                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;

                entity = service.Where(rec => rec.ID == repoID && rec.ClientID == ((PAIdentity)User.Identity).clientID).Single();
                
                if (entity == null) throw new PAException("Repository not found");

                entity.tbl_process_rep_document.Where(d => d.tbl_Process_RepositoryID == repoID && (!excludeIds.Contains("D" + d.ID.ToString())));
                if (entity.tbl_process_rep_document.Any())
                {
                    foreach (var d in entity.tbl_process_rep_document)
                    {
                        ctx.Entry(d).Reference(doc => doc.tbl_process_document).Load();
                        ctx.Entry(d.tbl_process_document).Reference(dm => dm.tbl_docmgr_document).Load();
                    }
                }
                entity.tbl_process_rep_chklst.Where(d => d.tbl_Process_RepositoryID == repoID && (!excludeIds.Contains("C" + d.ID.ToString())));
                if (entity.tbl_process_rep_chklst.Any())
                {
                    foreach (var c in entity.tbl_process_rep_chklst)
                    {
                        ctx.Entry(c).Reference(chklst => chklst.tbl_process_checklist).Load();
                    }
                }
                
                entity.tbl_process_rep_procedure.Where(d => d.tbl_Process_RepositoryID == repoID && (!excludeIds.Contains("P" + d.ID.ToString())));
                if (entity.tbl_process_rep_procedure.Any())
                {
                    foreach (var p in entity.tbl_process_rep_procedure)
                    {
                        ctx.Entry(p).Reference(proc => proc.tbl_process_procedure).Load();
                    }
                } 
                
                entity.tbl_process_rep_template.Where(d => d.tbl_Process_RepositoryID == repoID && (!excludeIds.Contains("T" + d.ID.ToString())));
                if (entity.tbl_process_rep_template.Any())
                {
                    foreach (var t in entity.tbl_process_rep_template)
                    {
                        ctx.Entry(t).Reference(tmpl => tmpl.tbl_process_template).Load();
                    }
                } 
                
                return View(entity);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult manageActivityRefDocs(int repoID, int activityID)
        {
            try
            {
                if (repoID == 0 || activityID == 0)
                {
                    throw new PAException("Insufficient Input");
                }
                var ctx = (Db)service.getRepo().getDBContext();

                var entity = ctx.tbl_process_rep_task.Include("tbl_process_rep_task_ref_docs").Where(o => o.tbl_Process_RepositoryID == repoID && o.ID == activityID).Single();

                if (entity == null) throw new PAException("Activity details not found");
                tbl_process_rep_task_ref_docsInput input = new tbl_process_rep_task_ref_docsInput()
                {
                    tbl_Process_Repository_ID = entity.tbl_Process_RepositoryID,
                    tbl_Process_Rep_TaskID = entity.ID,
                    refDocs = new List<refDoc>()
                };

                if (entity.tbl_process_rep_task_ref_docs != null && entity.tbl_process_rep_task_ref_docs.Any())
                {

                    foreach (var doc in entity.tbl_process_rep_task_ref_docs)
                    {
                        var newRec = new refDoc()
                        {
                            ID = doc.ID,
                            ClientID = doc.ClientID,
                            Remarks = doc.Remarks,
                            Mandatory = doc.Mandatory
                        };
                        if (doc.tbl_Process_Rep_DocumentID != null)
                        {
                            newRec.DocType = 1;
                            newRec.referenceID = doc.tbl_Process_Rep_DocumentID.GetValueOrDefault();
                            newRec.refKey = "D" + newRec.referenceID;
                        }
                        else
                        {
                            if (doc.tbl_Process_Rep_ProcID != null)
                            {
                                newRec.DocType = 2;
                                newRec.referenceID = doc.tbl_Process_Rep_ProcID.GetValueOrDefault();
                                newRec.refKey = "P" + newRec.referenceID;
                            }
                            else
                            {
                                if (doc.tbl_Process_Rep_ProcID != null)
                                {
                                    newRec.DocType = 3;
                                    newRec.referenceID = doc.tbl_Process_Rep_ProcID.GetValueOrDefault();
                                    newRec.refKey = "T" + newRec.referenceID;
                                }
                                else
                                {
                                    newRec.DocType = 4;
                                    newRec.referenceID = doc.tbl_Process_Rep_ChklstID.GetValueOrDefault();
                                    newRec.refKey = "C" + newRec.referenceID;
                                }
                            }
                        }
                        input.refDocs.Add(newRec);
                    }
                }
                return View(input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult manageActivityRefDocs(tbl_process_rep_task_ref_docsInput input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View(input);
            }
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();

                var entity = ctx.tbl_process_rep_task.Include("tbl_process_rep_task_ref_docs").Where(o => o.tbl_Process_RepositoryID == input.tbl_Process_Repository_ID && o.ID == input.tbl_Process_Rep_TaskID).Single();

                if (input.refDocs != null && input.refDocs.Any())
                {
                    foreach (var doc in input.refDocs)
                    {
                        var existingDoc = entity.tbl_process_rep_task_ref_docs.Where(o => o.ID == doc.ID).SingleOrDefault();
                        if (existingDoc == null)
                        {
                            // This is a new entry
                            entity.tbl_process_rep_task_ref_docs.Add(new tbl_process_rep_task_ref_docs()
                            {
                                ID = doc.ID,
                                tbl_Process_Repository_ID = input.tbl_Process_Repository_ID,
                                tbl_Process_Rep_TaskID = input.ID,
                                Remarks = doc.Remarks,
                                Mandatory = doc.Mandatory,
                                DocType = doc.DocType,
                                tbl_Process_Rep_DocumentID = (doc.DocType == 1 ? doc.referenceID : (int?)null),
                                tbl_Process_Rep_ProcID = (doc.DocType == 2 ? doc.referenceID : (int?)null),
                                tbl_Process_Rep_TmplID = (doc.DocType == 3 ? doc.referenceID : (int?)null),
                                tbl_Process_Rep_ChklstID = (doc.DocType == 4 ? doc.referenceID : (int?)null),
                                ClientID = ((PAIdentity)User.Identity).clientID
                            });
                        }
                        else
                        {
                            existingDoc.DocType = doc.DocType;
                            existingDoc.Remarks = doc.Remarks;
                            existingDoc.Mandatory = doc.Mandatory;
                            existingDoc.DocType = doc.DocType;
                            existingDoc.tbl_Process_Rep_DocumentID = (doc.DocType == 1 ? doc.referenceID : (int?)null);
                            existingDoc.tbl_Process_Rep_ProcID = (doc.DocType == 2 ? doc.referenceID : (int?)null);
                            existingDoc.tbl_Process_Rep_ProcID = (doc.DocType == 3 ? doc.referenceID : (int?)null);
                            existingDoc.tbl_Process_Rep_ChklstID = (doc.DocType == 4 ? doc.referenceID : (int?)null);
                            existingDoc.ClientID = ((PAIdentity)User.Identity).clientID;
                        }
                    }
                    foreach (var doc in entity.tbl_process_rep_task_ref_docs.ToList())
                    {
                        if (!(input.refDocs.Where(o => o.ID == doc.ID).Any()))
                        {
                            entity.tbl_process_rep_task_ref_docs.Remove(doc);
                        }
                    }
                }
                else
                {
                    if (entity.tbl_process_rep_task_ref_docs != null && entity.tbl_process_rep_task_ref_docs.Any())
                    {
                        foreach (var doc in entity.tbl_process_rep_task_ref_docs.ToList())
                        {
                            entity.tbl_process_rep_task_ref_docs.Remove(doc);
                        }
                    }
                }
                ctx.SaveChanges();
                return Content("Changes Saved Successfully");
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                ModelState.AddModelError("", e.Message);
                return View(input);
            }
        }

        protected override void InitiazeSequence(tbl_process_repositoryInput input) 
        {
            var seq = service.Where(o => o.Level == input.Level && o.StructPath == input.StructPath && o.ClientID == input.ClientID);
            if (seq.Any()) { input.Sequence = seq.Max(o => o.Sequence); input.Sequence++; } else { input.Sequence= 1; }
        }

        protected override void ReSequenceBeforeCreate(tbl_process_repositoryInput input) 
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.Sequence >= input.Sequence &&
                                                o.Level == input.Level && 
                                                o.StructPath == input.StructPath &&
                                                o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(p => p.Sequence);
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

        protected virtual void ReSequenceBeforeEdit(tbl_process_repositoryInput input) 
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.Sequence >= input.Sequence &&
                                                o.Level == input.Level &&
                                                o.StructPath == input.StructPath &&
                                                o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(p => p.Sequence);
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

        public ActionResult GetReferenceData(int id, int repoID, string selectedValue, string controlName, string reload)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();

                var entity = service.Get(repoID);
                if (entity == null)
                {
                    Response.StatusCode = 500;
                    return Content("Record does not exist");
                }
                List<KeyListItem> returnList = new List<KeyListItem>();
                // Document
                ctx.Entry(entity).Collection(o => o.tbl_process_rep_document).Load();
                if (entity.tbl_process_rep_document != null && entity.tbl_process_rep_document.Any())
                {
                    foreach (var doc in entity.tbl_process_rep_document)
                    {
                        ctx.Entry(doc).Reference(o => o.tbl_process_document).Load();
                        ctx.Entry(doc.tbl_process_document).Reference(o => o.tbl_docmgr_document).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = doc.ID,
                            GroupName = "Documents",
                            Key = "D" + doc.ID,
                            DisplayText = doc.tbl_process_document.tbl_docmgr_document.Name
                        });
                    }
                }
                ctx.Entry(entity).Collection(o => o.tbl_process_rep_procedure).Load();
                if (entity.tbl_process_rep_procedure != null && entity.tbl_process_rep_procedure.Any())
                {
                    foreach (var proc in entity.tbl_process_rep_procedure)
                    {
                        ctx.Entry(proc).Reference(o => o.tbl_process_procedure).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = proc.ID,
                            GroupName = "Procedures",
                            Key = "P" + proc.ID,
                            DisplayText = proc.tbl_process_procedure.Name
                        });
                    }
                }
                ctx.Entry(entity).Collection(o => o.tbl_process_rep_template).Load();
                if (entity.tbl_process_rep_template != null && entity.tbl_process_rep_template.Any())
                {
                    foreach (var tmpl in entity.tbl_process_rep_template)
                    {
                        ctx.Entry(tmpl).Reference(o => o.tbl_process_template).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = tmpl.ID,
                            GroupName = "Procedures",
                            Key = "T" + tmpl.ID,
                            DisplayText = tmpl.tbl_process_template.Name
                        });
                    }
                }
                ctx.Entry(entity).Collection(o => o.tbl_process_rep_chklst).Load();
                if (entity.tbl_process_rep_chklst != null && entity.tbl_process_rep_chklst.Any())
                {
                    foreach (var chklst in entity.tbl_process_rep_chklst)
                    {
                        ctx.Entry(chklst).Reference(o => o.tbl_process_checklist).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = chklst.ID,
                            GroupName = "Checklists",
                            Key = "C" + chklst.ID,
                            DisplayText = chklst.tbl_process_checklist.Name
                        });
                    }
                }
                ViewBag.selectedItem = selectedValue;
                ViewBag.itemName = controlName;
                ViewBag.reload = reload;
                return PartialView("ListItems/KeyGroupCombo", returnList.AsEnumerable());
            }
            catch (PAException e)
            {
                e.Raize();
                return Content("Request Failed: " + e.Message);
            }
        }
    }
}

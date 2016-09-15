using System.Web.Mvc;
using System.Linq;
using System;
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
    public class PrjProcessTailorController : Cruder<tbl_org_project_process_mapping, tbl_org_project_process_mappingInput>
    {
        public PrjProcessTailorController(ICrudService<tbl_org_project_process_mapping> service, IMapper<tbl_org_project_process_mapping, tbl_org_project_process_mappingInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLPTLRCF")
        {
            functionID = "PLPTLRCF";
        }

        public ActionResult getMapping(int projectID, int? projPhase)
        {
            try
            {
                if (projectID == 0 && projPhase == null)
                {
                    // At least one parameter should be provided
                    throw new PAException("Mapping not found");
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.vw_prj_process_mapping.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                                  o.tbl_Org_ProjectID == projectID &&
                                                                  ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase)).OrderBy(o => o.Level);

                IQueryable<tbl_tailored_rep_chklst> Rep_Chklsts;
                IQueryable<tbl_tailored_rep_document> Rep_Docs;
                IQueryable<tbl_tailored_rep_procedure> Rep_Procedures;
                IQueryable<tbl_tailored_rep_template> Rep_Templates;
                IQueryable<tbl_tailored_rep_task> Rep_Tasks;
                
                if (entity == null) throw new PAException("this entity doesn't exist anymore");

                List<returnData> returnList = new List<returnData>();

                foreach (var map in entity)
                {
                    returnList.Add(new returnData
                    {
                        ID = map.tbl_Org_ProjectID,
                        levelID = map.levelID,
                        levelName = map.LevelShortName,
                        nodeName = map.Name,
                        Level = (short) map.Level,
                        ParentID = map.ParentID,
                        accessID = map.tbl_Process_RepositoryID.ToString(),
                        Type = "Rep",
                        paramName = "projectID=" + map.tbl_Org_ProjectID + "&repoID=" + map.tbl_Process_RepositoryID + "&projPhase=" + map.mstr_Org_Proj_PhaseID,
                        paramValue = map.tbl_Process_RepositoryID,
                        Exclude = map.Exclude
                    });

                    // Procedures
                    Rep_Procedures = ctx.tbl_tailored_rep_procedure.Include("tbl_process_procedure").Where(o => o.ClientID == map.ClientID && o.tbl_Org_ProjectID == map.tbl_Org_ProjectID && 
                                                                               o.tbl_Process_RepositoryID == map.tbl_Process_RepositoryID &&
                                                                               ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase));
                    foreach (var prc in Rep_Procedures)
                    {
                        returnList.Add(new returnData
                        {
                            ID = prc.ID,
                            levelID = (prc.Exclude ? 1 : 0), // Used to store whether this mapping is excluded
                            levelName = "Changes Saved",
                            nodeName = (prc.TailorName == "" || prc.TailorName == null ? prc.tbl_process_procedure.Name : prc.TailorName),
                            Level = (prc.TailorNew.GetValueOrDefault() ? 1 : 0), // Used to store whether this is a addition to the existing configuration
                            ParentID = prc.tbl_Process_RepositoryID,     // RepositoryID
                            accessID = prc.tbl_Process_RepositoryID.ToString(),
                            Type = "Proc",
                            paramName = (prc.Exclude == true ? Url.Action("IncludeProc", "PrjProcessTailor", new { id = prc.ID }) : Url.Action("ExcludeProc", "PrjProcessTailor", new { id = prc.ID })),
                            paramValue = prc.ID,
                            previewURL = Url.Action("showPreview", "PProcedure", new { id = prc.tbl_Process_ProcedureID }),
                            previewClass = "openDialog ",
                            Exclude = prc.Exclude
                        });
                    }
                    // Templates
                    Rep_Templates = ctx.tbl_tailored_rep_template.Include("tbl_process_template").Where(o => o.ClientID == map.ClientID && o.tbl_Org_ProjectID == map.tbl_Org_ProjectID &&
                                                                               o.tbl_Process_RepositoryID == map.tbl_Process_RepositoryID &&
                                                                               ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase));
                    foreach (var tmpl in Rep_Templates)
                    {
                        returnList.Add(new returnData
                        {
                            ID = tmpl.ID,
                            levelID = (tmpl.Exclude ? 1 : 0), // Used to store whether this mapping is excluded
                            levelName = "",
                            nodeName = (tmpl.TailorName == "" || tmpl.TailorName == null ? tmpl.tbl_process_template.Name : tmpl.TailorName),
                            Level = (tmpl.TailorNew.GetValueOrDefault() ? 1 : 0), // Used to store whether this is a addition to the existing configuration
                            ParentID = tmpl.tbl_Process_RepositoryID,     // RepositoryID
                            accessID = tmpl.tbl_Process_RepositoryID.ToString(),
                            Type = "Template",
                            paramName = (tmpl.Exclude == true ? Url.Action("IncludeTmpl", "PrjProcessTailor", new { id = tmpl.ID }) : Url.Action("ExcludeTmpl", "PrjProcessTailor", new { id = tmpl.ID })),
                            paramValue = tmpl.ID,
                            previewURL = Url.Action("showPreview", "PTemplate", new { id = tmpl.tbl_Process_TemplateID }),
                            previewClass = "openDialog",
                            Exclude = tmpl.Exclude
                        });
                    }
                    // Checklists
                    Rep_Chklsts = ctx.tbl_tailored_rep_chklst.Include("tbl_process_checklist").Where(o => o.ClientID == map.ClientID && o.tbl_Org_ProjectID == map.tbl_Org_ProjectID &&
                                                                               o.tbl_Process_RepositoryID == map.tbl_Process_RepositoryID &&
                                                                               ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase));
                    foreach (var chk in Rep_Chklsts)
                    {
                        returnList.Add(new returnData
                        {
                            ID = chk.ID,
                            levelID = (chk.Exclude ? 1 : 0), // Used to store whether this mapping is excluded
                            levelName = "",
                            nodeName = (chk.TailorName == "" || chk.TailorName == null ? chk.tbl_process_checklist.Name : chk.TailorName),
                            Level = (chk.TailorNew.GetValueOrDefault() ? 1 : 0), // Used to store whether this is a addition to the existing configuration
                            ParentID = chk.tbl_Process_RepositoryID,     // RepositoryID
                            accessID = chk.tbl_Process_RepositoryID.ToString(),
                            Type = "Checklist",
                            paramName = (chk.Exclude == true ? Url.Action("IncludeChklst", "PrjProcessTailor", new { id = chk.ID }) : Url.Action("ExcludeChklst", "PrjProcessTailor", new { id = chk.ID })),
                            paramValue = chk.ID,
                            previewURL = Url.Action("showPreview", "PChecklist", new { id = chk.tbl_Process_ChecklistID }),
                            previewClass =  "openDialog",
                            Exclude = chk.Exclude
                        });
                    }
                    // Documents
                    Rep_Docs = ctx.tbl_tailored_rep_document.Include("tbl_process_document").Include("tbl_process_document.tbl_docmgr_document").Where(o => o.ClientID == map.ClientID && o.tbl_Org_ProjectID == map.tbl_Org_ProjectID &&
                                                                               o.tbl_Process_RepositoryID == map.tbl_Process_RepositoryID &&
                                                                               ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase));
                    foreach (var doc in Rep_Docs)
                    {
                        returnList.Add(new returnData
                        {
                            ID = doc.ID,
                            levelID = (doc.Exclude ? 1 : 0), // Used to store whether this mapping is excluded
                            levelName = "",
                            nodeName = (doc.TailorName == "" || doc.TailorName == null ? doc.tbl_process_document.tbl_docmgr_document.Name : doc.TailorName),
                            Level = (doc.TailorNew.GetValueOrDefault() ? 1 : 0), // Used to store whether this is a addition to the existing configuration
                            ParentID = doc.tbl_Process_RepositoryID,     // RepositoryID
                            accessID = doc.tbl_Process_RepositoryID.ToString(),
                            Type = "Document",
                            paramName = (doc.Exclude == true ? Url.Action("IncludeDoc", "PrjProcessTailor", new { id = doc.ID }) : Url.Action("ExcludeDoc", "PrjProcessTailor", new { id = doc.ID })),
                            paramValue = doc.ID,
                            previewURL = Url.Action("ViewDocument", "DocMgr", new { id = doc.tbl_process_document.tbl_docmgr_document.ID }, Request.Url.Scheme),
                            previewClass = "openDocument",
                            Exclude = doc.Exclude
                        });
                    }

                    // Tasks
                    Rep_Tasks = ctx.tbl_tailored_rep_task.Where(o => o.ClientID == map.ClientID && 
                                                                o.tbl_Org_ProjectID == map.tbl_Org_ProjectID && 
                                                                o.tbl_Process_RepositoryID == map.tbl_Process_RepositoryID &&
                                                                ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase));
                    foreach (var task in Rep_Tasks)
                    {
                        returnList.Add(new returnData
                        {
                            ID = task.ID,
                            levelID = (task.Exclude ? 1 : 0), // Used to store whether this mapping is excluded
                            levelName = "",
                            nodeName = (task.TailorName == "" || task.TailorName == null ? task.Name : task.TailorName) + (task.TailorNew != null && task.TailorNew == true ? " - (Tailored: New)" : (task.TailorName != null && task.TailorName != task.Name ? " - (Tailored: Name Changed)" : "")),
                            Level = (task.TailorNew.GetValueOrDefault() ? 1 : 0), // Used to store whether this is a addition to the existing configuration
                            ParentID = task.tbl_Process_RepositoryID,     // RepositoryID
                            accessID = task.tbl_Process_RepositoryID.ToString(),
                            Type = "Task",
                            paramName = (task.Exclude == true ? Url.Action("IncludeTask", "PrjProcessTailor", new { id = task.ID }) : Url.Action("ExcludeTask", "PrjProcessTailor", new { id = task.ID })),
                            paramValue = task.ID,
                            previewURL = Url.Action("showActivity", "PrjProcessTailor", new { id = task.ID }),
                            previewClass =  "openDialog",
                            Exclude = task.Exclude
                        });
                    }
                }
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult manageProjProcedures(int projectID, int repoID, int? projPhase)
        {
            try
            {
                if (projectID == 0 && repoID == 0)
                {
                    // At least one parameter should be provided
                    throw new PAException("Invalid Request");
                }

                tbl_process_rep_details input = new tbl_process_rep_details();

                input.ID = projectID;
                input.repoID = repoID;
                input.phaseID = projPhase;
                input.key = repoID.ToString();

                var ctx = (Db) service.getRepo().getDBContext();
                var procedures = ctx.tbl_tailored_rep_procedure.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                                      o.tbl_Process_RepositoryID == repoID &&
                                                                      ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &&
                                                                      o.Exclude == false);
                if (procedures.Any())
                {
                    foreach (var proc in procedures)
                    {
                        input.selectedOptions.Add((int)proc.tbl_Process_ProcedureID);
                    }
                }
                ViewBag.postURL = Url.Action("manageProjProcedures","PrjProcessTailor");
                return View("showProcedures", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult manageProjProcedures(tbl_process_rep_details input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    ViewBag.postURL = Url.Action("manageProjProcedures","PrjProcessTailor");
                    return View("showProcedures", input);
                }

                var ctx = (Db)service.getRepo().getDBContext();
                var procedures = ctx.tbl_tailored_rep_procedure.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                      o.tbl_Process_RepositoryID == input.repoID &&
                                                                      ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                var selectedIDs = "";

                using (TransactionScope scope = new TransactionScope())
                {
                    //service.getRepo().executeStoredCommand("delete from tbl_process_rep_procedure where tbl_Process_RepositoryID = " + input.ID); // delete existing mappings

                    if (input.selectedOptions.Any())
                    {
                        foreach (var g in input.selectedOptions)
                        {
                            var existingProc = procedures.Where(o => o.tbl_Process_ProcedureID == g).SingleOrDefault();
                            if (existingProc == null)
                            {
                                ctx.tbl_tailored_rep_procedure.Add(new tbl_tailored_rep_procedure()
                                {
                                    ClientID = ((PAIdentity)User.Identity).clientID,
                                    tbl_Process_RepositoryID = input.repoID,
                                    tbl_Process_ProcedureID = g,
                                    tbl_Org_ProjectID = input.ID,
                                    mstr_Org_Proj_PhaseID = input.phaseID,
                                    TailorNew = true,
                                    Exclude = false
                                });
                            }
                            else
                            {
                                if (existingProc.Exclude == true)
                                {
                                    existingProc.Exclude = false;
                                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Process_RepositoryID = " + input.repoID +
                                        " and tbl_Org_ProjectID = " + input.ID +
                                        "  and DocType = 2 and (TailorNew = 0 or TailorNew is null) and tbl_Process_ProcedureID = " + g); // Exclude existing mappings

                                }
                            }
                            selectedIDs = selectedIDs + (selectedIDs == "" ? "" : ",") + g;
                        }
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_procedure where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and TailorNew = 1 and tbl_Process_ProcedureID not in (" + selectedIDs + ")"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_procedure set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ProcedureID not in (" + selectedIDs + ")"); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 2 and tbl_Org_ProjectID = " + input.ID + 
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ProcedureID not in (" + selectedIDs + ") "); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 2 and tbl_Org_ProjectID = " + input.ID + 
                            " and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1) and tbl_Process_ProcedureID not in (" + selectedIDs + ") "); // Delete new tailored Mappings
                        ctx.SaveChanges();
                    }
                    else
                    {
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_procedure where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and TailorNew = 1"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_procedure set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and (TailorNew = 0 or TailorNew is null)"); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 2 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null)"); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 2 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1)"); // Delete new tailored Mappings
                        ctx.SaveChanges();
                    }
                    scope.Complete();
                }
                procedures = ctx.tbl_tailored_rep_procedure.Include("tbl_process_procedure").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                      o.tbl_Process_RepositoryID == input.repoID &&
                                                                      ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                ViewBag.projectID = input.ID;
                ViewBag.repoID = input.repoID;
                ViewBag.phaseID = input.phaseID;
                ViewBag.key = input.repoID.ToString();
                return View("getProcedures", procedures);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult IncludeProc(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var proc = ctx.tbl_tailored_rep_procedure.Include("tbl_process_procedure").Where(o => o.ID == id).SingleOrDefault();
                if (proc == null)
                {
                    return Content("<li id='LProc'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    proc.Exclude = false;
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Process_RepositoryID = " + proc.tbl_Process_RepositoryID +
                        "  and DocType = 2  and tbl_Org_ProjectID = " + proc.tbl_Org_ProjectID +
                        " and tbl_Process_RepositoryID = " + proc.tbl_Process_RepositoryID +
                        " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ProcedureID = " + proc.tbl_Process_ProcedureID); // Exclude existing mappings
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content("<li id='LProc" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LProc" + id + "' data-source='" + Url.Action("ExcludeProc", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to exclude this procedure'><i class='icon-remove'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-configured'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showPreview", "PProcedure", new { id = proc.tbl_Process_ProcedureID }) + "'>" +
                                            (proc.TailorNew == true ? proc.TailorName + " (Tailored: New)" : (proc.TailorName == null ? proc.tbl_process_procedure.Name : proc.TailorName + " (Tailored: Changed)")) +
                                            "</a></span></li>");
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult ExcludeProc(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var proc = ctx.tbl_tailored_rep_procedure.Include("tbl_process_procedure").Where(o => o.ID == id).SingleOrDefault();
                var returnInfo = "";

                if (proc == null)
                {
                    return Content("<li id='LProc'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    if (proc.TailorNew == true) // This was a new task so physically delete it
                    {
                        ctx.tbl_tailored_rep_procedure.Remove(proc);
                        ctx.Entry(proc).State = System.Data.Entity.EntityState.Deleted;
                    }
                    else
                    {
                        proc.Exclude = true;    // Maintain this record and just logically delete it.
                    }
                    // Exclude all instances of task supporting document where this template is being used
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 2 and tbl_Org_ProjectID = " + proc.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + proc.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ProcedureID = " + proc.tbl_Process_ProcedureID); // Exclude existing mappings
                    service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 2  and tbl_Org_ProjectID = " + proc.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + proc.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 1) and tbl_Process_ProcedureID = " + proc.tbl_Process_ProcedureID); // Delete new tailored Mappings
                    if (proc.TailorNew == null || proc.TailorNew == false)
                    {
                        // Return nothing as this record is now deleted
                        returnInfo = "<li id='LProc" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LProc" + id + "' data-source='" + Url.Action("IncludeProc", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to include this procedure'><i class='icon-ok'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-strikeThrough'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showPreview", "PProcedure", new { id = proc.tbl_Process_ProcedureID }) + "'>" +
                                            (proc.TailorName == null ? proc.tbl_process_procedure.Name : proc.TailorName + " (Tailored: Changed)") +
                                            "</a></span>&nbsp;<span class='label label-danger'>Excluded</span></li>";
                    }
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content(returnInfo);
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult manageProjChecklists(int projectID, int repoID, int? projPhase)
        {
            try
            {
                if (projectID == 0 && repoID == 0)
                {
                    // At least one parameter should be provided
                    throw new PAException("Invalid Request");
                }

                tbl_process_rep_details input = new tbl_process_rep_details();

                input.ID = projectID;
                input.repoID = repoID;
                input.phaseID = projPhase;
                input.key = repoID.ToString();

                var ctx = (Db)service.getRepo().getDBContext();
                var checklists = ctx.tbl_tailored_rep_chklst.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                                      o.tbl_Process_RepositoryID == repoID &&
                                                                      ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &&
                                                                      o.Exclude == false);
                if (checklists.Any())
                {
                    foreach (var chklst in checklists)
                    {
                        input.selectedOptions.Add((int)chklst.tbl_Process_ChecklistID);
                    }
                }
                ViewBag.postURL = Url.Action("manageProjChecklists", "PrjProcessTailor");
                return View("showChecklists", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult manageProjChecklists(tbl_process_rep_details input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    ViewBag.postURL = Url.Action("manageProjChecklists", "PrjProcessTailor");
                    return View("showChecklists", input);
                }

                var ctx = (Db)service.getRepo().getDBContext();
                var checklists = ctx.tbl_tailored_rep_chklst.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                      o.tbl_Process_RepositoryID == input.repoID &&
                                                                      ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                var selectedIDs = "";

                using (TransactionScope scope = new TransactionScope())
                {
                    //service.getRepo().executeStoredCommand("delete from tbl_process_rep_procedure where tbl_Process_RepositoryID = " + input.ID); // delete existing mappings

                    if (input.selectedOptions.Any())
                    {
                        foreach (var g in input.selectedOptions)
                        {
                            var existingChkLsts = checklists.Where(o => o.tbl_Process_ChecklistID == g).SingleOrDefault();
                            if (existingChkLsts == null)
                            {
                                ctx.tbl_tailored_rep_chklst.Add(new tbl_tailored_rep_chklst()
                                {
                                    ClientID = ((PAIdentity)User.Identity).clientID,
                                    tbl_Process_RepositoryID = input.repoID,
                                    tbl_Process_ChecklistID = g,
                                    tbl_Org_ProjectID = input.ID,
                                    mstr_Org_Proj_PhaseID = input.phaseID,
                                    TailorNew = true,
                                    Exclude = false
                                });
                            }
                            else
                            {
                                if (existingChkLsts.Exclude == true)
                                {
                                    existingChkLsts.Exclude = false;
                                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Org_ProjectID = " + input.ID +
                                                                            " and tbl_Process_RepositoryID = " + input.repoID +
                                                                            "  and DocType = 4 and (TailorNew = 0 or TailorNew is null) and tbl_Process_ChecklistID = " + g); // Exclude existing mappings
                                }
                            }
                            selectedIDs = selectedIDs + (selectedIDs == "" ? "" : ",") + g;
                        }
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_chklst where tbl_Org_ProjectID = " + input.ID +
                                        " and tbl_Process_RepositoryID = " + input.repoID +
                                        (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                        " and TailorNew = 1 and tbl_Process_ChecklistID not in (" + selectedIDs + ")"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_chklst set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ChecklistID not in (" + selectedIDs + ")"); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 4 and tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ChecklistID not in (" + selectedIDs + ") "); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 4 and tbl_Org_ProjectID = " + input.ID + 
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1) and tbl_Process_ChecklistID not in (" + selectedIDs + ") "); // Delete new tailored Mappings
                    }
                    else
                    {
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_chklst where tbl_Org_ProjectID = " + input.ID +
                                        " and tbl_Process_RepositoryID = " + input.repoID +
                                        (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                        " and TailorNew = 1"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_chklst set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and (TailorNew = 0 or TailorNew is null)"); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 4 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null)"); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 4 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1)"); // Delete new tailored Mappings

                    }
                    ctx.SaveChanges();
                    scope.Complete();
                }
                checklists = ctx.tbl_tailored_rep_chklst.Include("tbl_process_checklist").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                    o.tbl_Process_RepositoryID == input.repoID &&
                                                                    ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                ViewBag.projectID = input.ID;
                ViewBag.repoID = input.repoID;
                ViewBag.phaseID = input.phaseID;
                ViewBag.key = input.repoID.ToString();
                return View("getChecklists", checklists);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult IncludeChklst(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var chklst = ctx.tbl_tailored_rep_chklst.Include("tbl_process_checklist").Where(o => o.ID == id).SingleOrDefault();
                if (chklst == null)
                {
                    return Content("<li id='LChecklist'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    chklst.Exclude = false;
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Process_RepositoryID = " + chklst.tbl_Process_RepositoryID +
                        "  and DocType = 4  and tbl_Org_ProjectID = " + chklst.tbl_Org_ProjectID +
                        " and tbl_Process_RepositoryID = " + chklst.tbl_Process_RepositoryID +
                        " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ChecklistID = " + chklst.tbl_Process_ChecklistID); // Exclude existing mappings
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content("<li id='LChecklist" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LChecklist" + id + "' data-source='" + Url.Action("ExcludeChklst", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to exclude this checklist'><i class='icon-remove'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-configured'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showPreview", "PChecklist", new { id = chklst.tbl_Process_ChecklistID }) + "'>" +
                                            (chklst.TailorNew == true ? chklst.TailorName + " (Tailored: New)" : (chklst.TailorName == null ? chklst.tbl_process_checklist.Name : chklst.TailorName + " (Tailored: Changed)")) +
                                            "</a></span></li>");
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult ExcludeChklst(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var chklst = ctx.tbl_tailored_rep_chklst.Include("tbl_process_checklist").Where(o => o.ID == id).SingleOrDefault();
                var returnInfo = "";

                if (chklst == null)
                {
                    return Content("<li id='LChecklist'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    if (chklst.TailorNew == true) // This was a new task so physically delete it
                    {
                        ctx.tbl_tailored_rep_chklst.Remove(chklst);
                        ctx.Entry(chklst).State = System.Data.Entity.EntityState.Deleted;
                    }
                    else
                    {
                        chklst.Exclude = true;    // Maintain this record and just logically delete it.
                    }
                    // Exclude all instances of task supporting document where this template is being used
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 4 and tbl_Org_ProjectID = " + chklst.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + chklst.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 0 or TailorNew is null) and tbl_Process_ChecklistID = " + chklst.tbl_Process_ChecklistID); // Exclude existing mappings
                    service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 4  and tbl_Org_ProjectID = " + chklst.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + chklst.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 1) and tbl_Process_ChecklistID = " + chklst.tbl_Process_ChecklistID); // Delete new tailored Mappings
                    if (chklst.TailorNew == null || chklst.TailorNew == false)
                    {
                        // Return nothing as this record is now deleted
                        returnInfo = "<li id='LChecklist" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LChecklist" + id + "' data-source='" + Url.Action("IncludeChklst", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to include this checklist'><i class='icon-ok'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-strikeThrough'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showPreview", "PChecklist", new { id = chklst.tbl_Process_ChecklistID }) + "'>" +
                                            (chklst.TailorName == null ? chklst.tbl_process_checklist.Name : chklst.TailorName + " (Tailored: Changed)") +
                                            "</a></span>&nbsp;<span class='label label-danger'>Excluded</span></li>";
                    }
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content(returnInfo);
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult manageProjTemplates(int projectID, int repoID, int? projPhase)
        {
            try
            {
                if (projectID == 0 && repoID == 0)
                {
                    // At least one parameter should be provided
                    throw new PAException("Invalid Request");
                }

                tbl_process_rep_details input = new tbl_process_rep_details();

                input.ID = projectID;
                input.repoID = repoID;
                input.phaseID = projPhase;
                input.key = repoID.ToString();

                var ctx = (Db)service.getRepo().getDBContext();
                var templates = ctx.tbl_tailored_rep_template.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                                      o.tbl_Process_RepositoryID == repoID &&
                                                                      ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &&
                                                                      o.Exclude == false);
                if (templates.Any())
                {
                    foreach (var tmpl in templates)
                    {
                        input.selectedOptions.Add((int)tmpl.tbl_Process_TemplateID);
                    }
                }
                ViewBag.postURL = Url.Action("manageProjTemplates", "PrjProcessTailor");
                return View("showTemplates", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult manageProjTemplates(tbl_process_rep_details input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    ViewBag.postURL = Url.Action("manageProjTemplates", "PrjProcessTailor");
                    return View("showTemplates", input);
                }

                var ctx = (Db)service.getRepo().getDBContext();
                var templates = ctx.tbl_tailored_rep_template.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                      o.tbl_Process_RepositoryID == input.repoID &&
                                                                      ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                var selectedIDs = "";

                using (TransactionScope scope = new TransactionScope())
                {
                    //service.getRepo().executeStoredCommand("delete from tbl_process_rep_procedure where tbl_Process_RepositoryID = " + input.ID); // delete existing mappings

                    if (input.selectedOptions.Any())
                    {
                        foreach (var g in input.selectedOptions)
                        {
                            var existingTmpl = templates.Where(o => o.tbl_Process_TemplateID == g).SingleOrDefault();
                            if (existingTmpl == null)
                            {
                                ctx.tbl_tailored_rep_template.Add(new tbl_tailored_rep_template()
                                {
                                    ClientID = ((PAIdentity)User.Identity).clientID,
                                    tbl_Process_RepositoryID = input.repoID,
                                    tbl_Process_TemplateID = g,
                                    tbl_Org_ProjectID = input.ID,
                                    mstr_Org_Proj_PhaseID = input.phaseID,
                                    TailorNew = true,
                                    Exclude = false
                                });
                            }
                            else
                            {
                                if (existingTmpl.Exclude == true)
                                {
                                    existingTmpl.Exclude = false;
                                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Org_ProjectID = " + input.ID + 
                                                                            " and tbl_Process_RepositoryID = " + input.repoID +
                                                                            " and DocType = 3 and (TailorNew = 0 or TailorNew is null) and tbl_Process_TemplateID = " + g); // Exclude existing mappings
                                }
                            }
                            selectedIDs = selectedIDs + (selectedIDs == "" ? "" : ",") + g;
                        }
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_template where tbl_Org_ProjectID = " + input.ID +
                                                               " and tbl_Process_RepositoryID = " + input.repoID +
                                                               (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                               " and TailorNew = 1 and tbl_Process_TemplateID not in (" + selectedIDs + ")"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_template set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                               " and tbl_Process_RepositoryID = " + input.repoID +
                                                               (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                               " and (TailorNew = 0 or TailorNew is null) and tbl_Process_TemplateID not in (" + selectedIDs + ")"); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 3 and tbl_Org_ProjectID = " + input.ID + 
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null) and tbl_Process_TemplateID not in (" + selectedIDs + ") "); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 3 and tbl_Org_ProjectID = " + input.ID + 
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1) and tbl_Process_TemplateID not in (" + selectedIDs + ") "); // Delete new tailored Mappings
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // Delete all records
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_template where tbl_Org_ProjectID = " + input.ID +
                                                               " and tbl_Process_RepositoryID = " + input.repoID +
                                                               (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                               " and TailorNew = 1"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_template set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                               " and tbl_Process_RepositoryID = " + input.repoID +
                                                               (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                               " and (TailorNew = 0 or TailorNew is null)" ); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 3 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null) "); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 3 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1)"); // Delete new tailored Mappings
                        ctx.SaveChanges();
                    }
                    scope.Complete();
                }
                templates = ctx.tbl_tailored_rep_template.Include("tbl_process_template").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                      o.tbl_Process_RepositoryID == input.repoID &&
                                                                      ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                ViewBag.projectID = input.ID;
                ViewBag.repoID = input.repoID;
                ViewBag.phaseID = input.phaseID;
                ViewBag.key = input.repoID.ToString();
                return View("getTemplates", templates);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult IncludeTmpl(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var tmpl = ctx.tbl_tailored_rep_template.Include("tbl_process_template").Where(o => o.ID == id).SingleOrDefault();
                if (tmpl == null)
                {
                    return Content("<li id='LTemplate'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    tmpl.Exclude = false;
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Org_ProjectID = " + tmpl.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + tmpl.tbl_Process_RepositoryID +
                                                            " and DocType = 3 and (TailorNew = 0 or TailorNew is null) and tbl_Process_TemplateID = " + tmpl.tbl_Process_TemplateID); // Exclude existing mappings
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content("<li id='LTemplate" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LTemplate" + id + "' data-source='" + Url.Action("ExcludeTmpl", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to exclude this template'><i class='icon-remove'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-configured'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showPreview", "PTemplate", new { id = tmpl.tbl_Process_TemplateID }) + "'>" +
                                            (tmpl.TailorNew == true ? tmpl.TailorName + " (Tailored: New)" : (tmpl.TailorName == null ? tmpl.tbl_process_template.Name : tmpl.TailorName + " (Tailored: Changed)")) +
                                            "</a></span></li>");
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult ExcludeTmpl(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var tmpl = ctx.tbl_tailored_rep_template.Include("tbl_process_template").Where(o => o.ID == id).SingleOrDefault();
                var returnInfo = "";

                if (tmpl == null)
                {
                    return Content("<li id='LTemplate'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    if (tmpl.TailorNew == true) // This was a new task so physically delete it
                    {
                        ctx.tbl_tailored_rep_template.Remove(tmpl);
                        ctx.Entry(tmpl).State = System.Data.Entity.EntityState.Deleted;
                    }
                    else
                    {
                        tmpl.Exclude = true;    // Maintain this record and just logically delete it.
                    }
                    // Exclude all instances of task supporting document where this template is being used
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 3 and tbl_Org_ProjectID = " + tmpl.tbl_Org_ProjectID + 
                                                            " and tbl_Process_RepositoryID = " + tmpl.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 0 or TailorNew is null) and tbl_Process_TemplateID = " + tmpl.tbl_Process_TemplateID); // Exclude existing mappings
                    service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 3 and tbl_Org_ProjectID = " + tmpl.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + tmpl.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 1) and tbl_Process_TemplateID = " + tmpl.tbl_Process_TemplateID); // Delete new tailored Mappings
                    if (tmpl.TailorNew == null || tmpl.TailorNew == false)
                    {
                        // Return nothing as this record is now deleted
                        returnInfo = "<li id='LTemplate" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LTemplate" + id + "' data-source='" + Url.Action("IncludeTmpl", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to include this template'><i class='icon-ok'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-strikeThrough'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showPreview", "PTemplate", new { id = tmpl.tbl_Process_TemplateID }) + "'>" +
                                            (tmpl.TailorName == null ? tmpl.tbl_process_template.Name : tmpl.TailorName + " (Tailored: Changed)") +
                                            "</a></span>&nbsp;<span class='label label-danger'>Excluded</span></li>";
                    }
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content(returnInfo);
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult manageGeneralTasks(int projectID, int repoID, int? projPhase)
        {
            try
            {
                tbl_process_rep_generaltasks input = new tbl_process_rep_generaltasks();

                var ctx = (Db)service.getRepo().getDBContext();

                var generalTasks = ctx.tbl_tailored_general_task.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                                      o.tbl_Process_RepositoryID == repoID &&
                                                                        ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &&
                                                                      o.Exclude == false);
                string[] roles;
                tbl_process_general_taskInput gtInput;

                input.ID = projectID;
                input.repoID = repoID;
                input.phaseID = projPhase;
                input.key = repoID.ToString();

                foreach (var gt in generalTasks)
                {
                    gtInput = new tbl_process_general_taskInput() {
                        ID = gt.ID,
                        ClientID = gt.ClientID,
                        Name = gt.Name,
                        Description = gt.Description,
                        SequenceNo = gt.SequenceNo,
                        tbl_Process_RepositoryID = gt.tbl_Process_RepositoryID,
                    };
                    if (gt.mstr_Process_Role_Ids != null && gt.mstr_Process_Role_Ids != "")
                    {
                        roles = gt.mstr_Process_Role_Ids.Split(',');
                        gtInput.roleIDs = new List<string>();
                        foreach (var rl in roles)       // Add comman seperated roles as a list
                        {
                            gtInput.roleIDs.Add(rl);
                        }
                    }
                    input.tbl_process_general_task.Add(gtInput);
                }
                ViewBag.PostURL = Url.Action("manageGeneralTasks", "PrjProcessTailor");
                ViewBag.deleteURL = Url.Action("deleteGeneralTask", "PrjProcessTailor");
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
                    ViewBag.PostURL = Url.Action("manageGeneralTasks", "PrjProcessTailor");
                    ViewBag.deleteURL = Url.Action("deleteGeneralTask", "PrjProcessTailor");
                    return View("showTasks", input);
                }
                var ctx = (Db)service.getRepo().getDBContext();
                if (input.tbl_process_general_task.Any())
                {
                    tbl_tailored_general_task entity;
                    string roles;

                    foreach (var g in input.tbl_process_general_task)
                    {
                        roles = "";
                        if (g.roleIDs != null && g.roleIDs.Any())
                        {
                            foreach (var rl in g.roleIDs)
                            {
                                roles = roles + (roles == "" ? "" : ",") + rl;
                            }
                        }
                        entity = ctx.tbl_tailored_general_task.Where(o => o.ID == g.ID && o.tbl_Process_RepositoryID == input.repoID).SingleOrDefault();
                        if (entity == null)
                        {
                            entity = ctx.tbl_tailored_general_task.Add(new tbl_tailored_general_task()
                                                                        {
                                                                            ClientID = ((PAIdentity)User.Identity).clientID,
                                                                            Name = g.Name,
                                                                            Description = g.Description,
                                                                            Exclude = false,
                                                                            TailorNew = true,
                                                                            tbl_Org_ProjectID = input.ID,
                                                                            tbl_Process_RepositoryID = input.repoID,
                                                                            mstr_Org_Proj_PhaseID  = input.phaseID,
                                                                            SequenceNo = g.SequenceNo,
                                                                            Ref_General_TaskID = null,
                                                                            mstr_Process_Role_Ids = roles
                                                                        });

                        }
                        else
                        {
                            entity.Name = g.Name;
                            entity.Description = g.Name;
                            entity.mstr_Process_Role_Ids = roles;
                        }
                        ctx.SaveChanges();
                    }
                }
                var gts = ctx.tbl_tailored_general_task.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                                   o.tbl_Process_RepositoryID == input.repoID &&
                                                                   ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID) &&
                                                                   o.tbl_Org_ProjectID == input.ID);
                ViewBag.projectID = input.ID;
                ViewBag.repoID = input.repoID;
                ViewBag.phaseID = input.phaseID;
                ViewBag.key = input.repoID.ToString();
                return View("GetGeneralTasks", gts);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult IncludeGeneralTask(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_tailored_general_task.Where(o => o.ID == id).SingleOrDefault();
            if (entity == null)
            {
                Response.StatusCode = 500;
                return Content("<li>Task No longer exists</li>");
            }
            else
            {
                entity.Exclude = false;
                ctx.SaveChanges();
                return Content("<li id = 'LGT" + entity.ID + "'>&nbsp;&nbsp;<span class='label label-configured'>" + entity.Name + "</span></li>");
            }
        }

        public ActionResult deleteGeneralTask(int id, int repoID)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();

                var gt = ctx.tbl_tailored_general_task.Where(o => o.ID == id).SingleOrDefault();

                var projectID = gt.tbl_Org_ProjectID;
                var phaseID = gt.mstr_Org_Proj_PhaseID;

                if (gt.TailorNew == true)
                {
                    ctx.tbl_tailored_general_task.Remove(gt);
                }
                else
                {
                    gt.Exclude = true;
                }
                ctx.SaveChanges();

                var input = new tbl_tailored_general_task();
                var updatedGT = ctx.tbl_tailored_general_task.Where(o => o.tbl_Org_ProjectID == projectID && o.tbl_Process_RepositoryID == repoID &&
                                                                        ((phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == phaseID));
                ViewBag.projectID = projectID;
                ViewBag.repoID = repoID;
                ViewBag.phaseID = phaseID;
                ViewBag.key = repoID.ToString();
                return View("GetGeneralTasks", updatedGT);
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getSupportingDocuments(int processMapID, int planID, string excludeIDs, int key)
        {
            var entity = service.Get(processMapID);
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Invalid Mapping code";
                return View("ListItems/showError");
            }
            List<tbl_org_plan_documentInput> suppDocs = new List<tbl_org_plan_documentInput>();
            var ctx = (Db)service.getRepo().getDBContext();
            IEnumerable<string> planDocs = new List<string>();
            if (excludeIDs != null & excludeIDs != "")
            {
                planDocs = excludeIDs.Split(',').Select(str => str);
            }
            if (entity.tbl_Process_TaskID != null)
            {
                var documents = ctx.tbl_tailored_rep_task_ref_docs.Include("tbl_tailored_rep_task")
                                                                  .Include("tbl_process_document")
                                                                  .Include("tbl_process_document.tbl_docmgr_document")
                                                                  .Where(o => o.ClientID == entity.ClientID && o.tbl_Org_ProjectID == entity.tbl_Org_ProjectID &&
                                                                        o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID &&
                                                                        o.tbl_tailored_rep_task.Ref_Rep_TaskID == entity.tbl_Process_TaskID && 
                                                                        (o.Exclude == false || o.Exclude == null));
                foreach (var doc in documents)
                {
                    var docKey = doc.DocType + "-" + (doc.DocType == 1 ? doc.tbl_process_document.tbl_DocMgr_DocumentID :
                        (doc.DocType == 2 ? doc.tbl_Process_ProcedureID : (doc.DocType == 3 ? doc.tbl_Process_TemplateID : doc.tbl_Process_ChecklistID)));
                    if (!planDocs.Contains(docKey))
                    {
                        suppDocs.Add(new tbl_org_plan_documentInput()
                        {
                            ClientID = doc.ClientID,
                            tbl_DocMgr_DocumentID = (doc.tbl_process_document != null ? doc.tbl_process_document.tbl_DocMgr_DocumentID : null),
                            tbl_Process_ChecklistID = doc.tbl_Process_ChecklistID,
                            tbl_Process_ProcedureID = doc.tbl_Process_ProcedureID,
                            tbl_Process_TemplateID = doc.tbl_Process_TemplateID,
                            Name = doc.Name,
                            ReferenceType = (byte)doc.Mandatory,
                            Remarks = doc.Remarks,
                            tbl_Org_PlanID = planID,
                            DocType = (byte)doc.DocType,
                            Source = 1
                        });
                    }
                }
            }
            else
            {
                var documents = ctx.tbl_tailored_rep_document.Include("tbl_process_document").Include("tbl_process_document.tbl_docmgr_document").Where(o => o.ClientID == entity.ClientID && o.tbl_Org_ProjectID == entity.tbl_Org_ProjectID &&
                                                                        o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID &&
                                                                        ((o.mstr_Org_Proj_PhaseID == null && entity.mstr_Org_Proj_PhaseID == null) ||
                                                                         (o.mstr_Org_Proj_PhaseID == entity.mstr_Org_Proj_PhaseID)) &&
                                                                         (o.Exclude == false || o.Exclude == null));
                foreach (var doc in documents)
                {
                    var docKey = "1-" + doc.tbl_process_document.tbl_DocMgr_DocumentID;
                    if (!planDocs.Contains(docKey))
                    {
                        suppDocs.Add(new tbl_org_plan_documentInput()
                        {
                            ClientID = doc.ClientID,
                            tbl_DocMgr_DocumentID = doc.tbl_process_document.tbl_DocMgr_DocumentID,
                            Name = doc.tbl_process_document.tbl_docmgr_document.Name,
                            tbl_Org_PlanID = processMapID,
                            DocType = 1,
                            Source = 1
                        });
                    }
                }
                var procedures = ctx.tbl_tailored_rep_procedure.Include("tbl_process_procedure").Where(o => o.ClientID == entity.ClientID && o.tbl_Org_ProjectID == entity.tbl_Org_ProjectID &&
                                                                        o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID &&
                                                                        ((o.mstr_Org_Proj_PhaseID == null && entity.mstr_Org_Proj_PhaseID == null) ||
                                                                         (o.mstr_Org_Proj_PhaseID == entity.mstr_Org_Proj_PhaseID)) &&
                                                                         (o.Exclude == false || o.Exclude == null));
                foreach (var p in procedures)
                {
                    var docKey = "2-" + p.tbl_Process_ProcedureID;
                    if (!planDocs.Contains(docKey))
                    {
                        suppDocs.Add(new tbl_org_plan_documentInput()
                        {
                            ClientID = p.ClientID,
                            tbl_Process_ProcedureID = p.tbl_Process_ProcedureID,
                            Name = ((p.TailorName == null || p.TailorName == "") ? p.tbl_process_procedure.Name : p.TailorName),
                            tbl_Org_PlanID = processMapID,
                            DocType = 2,
                            Source = 1
                        });
                    }
                }
                var planTmplts = ctx.tbl_org_plan_document.Where(o => o.tbl_Org_PlanID == planID && o.DocType == 3 && o.Source == 1).Select(o => o.tbl_Process_TemplateID);
                var templates = ctx.tbl_tailored_rep_template.Include("tbl_process_template").Where(o => o.ClientID == entity.ClientID && o.tbl_Org_ProjectID == entity.tbl_Org_ProjectID &&
                                                                        o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID &&
                                                                        !planTmplts.Contains(o.tbl_Process_TemplateID) &&
                                                                        ((o.mstr_Org_Proj_PhaseID == null && entity.mstr_Org_Proj_PhaseID == null) ||
                                                                            (o.mstr_Org_Proj_PhaseID == entity.mstr_Org_Proj_PhaseID)) &&
                                                                            (o.Exclude == false || o.Exclude == null));
                foreach (var tmp in templates)
                {
                    var docKey = "3-" + tmp.tbl_Process_TemplateID;
                    if (!planDocs.Contains(docKey))
                    {
                        suppDocs.Add(new tbl_org_plan_documentInput()
                        {
                            ClientID = tmp.ClientID,
                            tbl_Process_TemplateID = tmp.tbl_Process_TemplateID,
                            Name = ((tmp.TailorName == null || tmp.TailorName == "") ? tmp.tbl_process_template.Name : tmp.TailorName),
                            tbl_Org_PlanID = processMapID,
                            DocType = 3,
                            Source = 1
                        });
                    }
                }
                var planChkLsts = ctx.tbl_org_plan_document.Where(o => o.tbl_Org_PlanID == planID && o.DocType == 4 && o.Source == 1).Select(o => o.tbl_Process_ChecklistID);
                var checklists = ctx.tbl_tailored_rep_chklst.Include("tbl_process_checklist").Where(o => o.ClientID == entity.ClientID && o.tbl_Org_ProjectID == entity.tbl_Org_ProjectID &&
                                                                        o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID &&
                                                                        !planChkLsts.Contains(o.tbl_Process_ChecklistID) &&
                                                                        ((o.mstr_Org_Proj_PhaseID == null && entity.mstr_Org_Proj_PhaseID == null) ||
                                                                            (o.mstr_Org_Proj_PhaseID == entity.mstr_Org_Proj_PhaseID)) &&
                                                                            (o.Exclude == false || o.Exclude == null));
                foreach (var chk in checklists)
                {
                    var docKey = "4-" + chk.tbl_Process_ChecklistID;
                    if (!planDocs.Contains(docKey))
                    {
                        suppDocs.Add(new tbl_org_plan_documentInput()
                        {
                            ClientID = chk.ClientID,
                            tbl_Process_ChecklistID = chk.tbl_Process_ChecklistID,
                            Name = ((chk.TailorName == null || chk.TailorName == "") ? chk.tbl_process_checklist.Name : chk.TailorName),
                            tbl_Org_PlanID = processMapID,
                            DocType = 4,
                            Source = 1
                        });
                    }
                }
            }
            ViewBag.Key = key;
            return PartialView(suppDocs);
        }

        [HttpPost]
        public ActionResult ResetMapping(int id, int? phaseID)
        {
            try
            {
                service.getRepo().executeStoredCommand("exec sp_mapProjectProcess " + id);  // Reload the mapping
                return RedirectToAction("getMapping", new { projectID = id, projPhase = phaseID });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Data dependent on this mapping (e.g. assigned task, timesheet entries) has been created in the system. Hence this mapping cannot be refreshed.";
                return View("ListItems/showError");
            }
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


        protected override string RowViewName
        {
            get { return "Edit"; }
        }

        public ActionResult addSupportingDoc(int taskID, int projectID, int key)
        {
            ViewBag.taskID = taskID;
            ViewBag.projectID = projectID;
            ViewBag.key = key;
            ViewBag.clientID = ((PAIdentity)User.Identity).clientID;
            return View();
        }

        [HttpPost]
        public ActionResult DeleteActivityDoc(int id)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_tailored_rep_task_ref_docs.Where(o => o.ID == id).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "This record does not exist";
                    return View("ListItems/showError");	// Return error in a dialog box
                }
                refDoc input = new refDoc();
                ViewBag.TaskID = entity.tbl_Tailored_Rep_TaskID;
                ViewBag.Include = false;
                if (entity.TailorNew != true)
                {
                    entity.Exclude = true;
                    input.ID = entity.ID;
                    input.ClientID = entity.ClientID;
                    input.DocType = entity.DocType;
                    input.DocumentName = entity.Name;
                    input.Mandatory = entity.Mandatory;
                    input.Remarks = entity.Remarks;
                    input.referenceID = (entity.DocType == 1 ? entity.tbl_Process_DocumentID.GetValueOrDefault() :
                                       (entity.DocType == 2 ? entity.tbl_Process_ProcedureID.GetValueOrDefault() :
                                       (entity.DocType == 3 ? entity.tbl_Process_TemplateID.GetValueOrDefault() : entity.tbl_Process_ChecklistID.GetValueOrDefault())));
                    input.refKey = (entity.DocType == 1 ? "D" + entity.tbl_Process_DocumentID.GetValueOrDefault() :
                                       (entity.DocType == 2 ? "P" + entity.tbl_Process_ProcedureID.GetValueOrDefault() :
                                       (entity.DocType == 3 ? "T" + entity.tbl_Process_TemplateID.GetValueOrDefault() : "C" + entity.tbl_Process_ChecklistID.GetValueOrDefault())));

                }
                else
                {
                    ctx.tbl_tailored_rep_task_ref_docs.Remove(entity);
                    input.ID = 0;
                }
                ctx.SaveChanges();
                return PartialView("IncludeTaskDoc",input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult showActivity(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_tailored_rep_task.Include("tbl_process_rep_task").Include("tbl_tailored_rep_task_ref_docs").Where(o => o.ID == id).SingleOrDefault();
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Activity does not exist";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            if (entity.Exclude == true) ViewBag.ReadOnly = true;        // Dont allow edits
            tbl_process_rep_task_ref_docsInput input = new tbl_process_rep_task_ref_docsInput()
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Name = entity.Name,
                TailorName = (entity.TailorName == null || entity.TailorName == "" ? entity.Name : entity.TailorName),
                Description = entity.Description,
                Type = (byte) entity.Type,
                tbl_Process_Repository_ID = entity.tbl_Process_RepositoryID,
                tbl_Process_Rep_TaskID = entity.Ref_Rep_TaskID,
                tbl_Org_ProjectID = entity.tbl_Org_ProjectID
            };
            input.refDocs = new List<refDoc>();
            if (entity.tbl_tailored_rep_task_ref_docs != null && entity.tbl_tailored_rep_task_ref_docs.Any())
            {
                foreach (var doc in entity.tbl_tailored_rep_task_ref_docs)
                {
                    if (doc.Exclude == null || doc.Exclude == false)
                    {
                        var refKey = "";
                        int referenceID = 0;

                        switch (doc.DocType)
                        {
                            case 1:
                                refKey = "D" + doc.tbl_Process_DocumentID;
                                referenceID = (int)doc.tbl_Process_DocumentID;
                                break;
                            case 2:
                                refKey = "P" + doc.tbl_Process_ProcedureID;
                                referenceID = (int)doc.tbl_Process_ProcedureID;
                                break;
                            case 3:
                                refKey = "T" + doc.tbl_Process_TemplateID;
                                referenceID = (int)doc.tbl_Process_TemplateID;
                                break;
                            case 4:
                                refKey = "C" + doc.tbl_Process_ChecklistID;
                                referenceID = (int)doc.tbl_Process_ChecklistID;
                                break;
                            default:
                                break;
                        }
                        input.refDocs.Add(new refDoc()
                        {
                            ID = doc.ID,
                            ClientID = doc.ClientID,
                            DocType = doc.DocType,
                            DocumentName = doc.Name,
                            Mandatory = doc.Mandatory,
                            Remarks = doc.Remarks,
                            refKey = refKey,
                            referenceID = referenceID
                        });
                    }
                }
            }
            return View(input);
        }


        [HttpPost]
        public ActionResult showActivity(tbl_process_rep_task_ref_docsInput input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Check for missing input or errors in both tabs");
                Response.StatusCode = 412;
                return View(input);
            }
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();

                var entity = ctx.tbl_tailored_rep_task.Include("tbl_tailored_rep_task_ref_docs").Where(o => o.ID == input.ID).SingleOrDefault();
                entity.TailorName = input.TailorName;
                entity.Description = input.Description;
                
                if (input.refDocs != null && input.refDocs.Any())
                {
                    foreach (var doc in input.refDocs)
                    {
                        var existingDoc = entity.tbl_tailored_rep_task_ref_docs.Where(o => o.ID == doc.ID).SingleOrDefault();
                        if (existingDoc == null)
                        {
                            // This is a new entry
                            entity.tbl_tailored_rep_task_ref_docs.Add(new tbl_tailored_rep_task_ref_docs()
                            {
                                ID = doc.ID,
                                ClientID = ((PAIdentity)User.Identity).clientID,
                                DocType = doc.DocType,
                                Exclude = false,
                                TailorNew = true,
                                tbl_Tailored_Rep_TaskID = entity.ID,
                                tbl_Process_RepositoryID = entity.tbl_Process_RepositoryID,
                                tbl_Org_ProjectID = entity.tbl_Org_ProjectID,
                                tbl_Process_Rep_Task_RefID = entity.Ref_Rep_TaskID,
                                Name = doc.DocumentName.Trim(),
                                TailorName = doc.DocumentName.Trim(),
                                Remarks = doc.Remarks,
                                Mandatory = doc.Mandatory,
                                tbl_Process_DocumentID = (doc.DocType == 1 ? doc.referenceID : (int?)null),
                                tbl_Process_ProcedureID = (doc.DocType == 2 ? doc.referenceID : (int?)null),
                                tbl_Process_TemplateID = (doc.DocType == 3 ? doc.referenceID : (int?)null),
                                tbl_Process_ChecklistID = (doc.DocType == 4 ? doc.referenceID : (int?)null),
                            });
                        }
                        else
                        {
                            if (existingDoc.Exclude != true)
                            {
                                existingDoc.DocType = doc.DocType;
                                existingDoc.Remarks = doc.Remarks;
                                existingDoc.Mandatory = doc.Mandatory;
                                existingDoc.DocType = doc.DocType;
                                existingDoc.Name = doc.DocumentName.Trim();
                                existingDoc.TailorName = doc.DocumentName.Trim();
                                existingDoc.tbl_Process_DocumentID = (doc.DocType == 1 ? doc.referenceID : (int?)null);
                                existingDoc.tbl_Process_ProcedureID = (doc.DocType == 2 ? doc.referenceID : (int?)null);
                                existingDoc.tbl_Process_TemplateID = (doc.DocType == 3 ? doc.referenceID : (int?)null);
                                existingDoc.tbl_Process_ChecklistID = (doc.DocType == 4 ? doc.referenceID : (int?)null);
                                existingDoc.ClientID = ((PAIdentity)User.Identity).clientID;
                            }
                            else doc.followWF = true;   // Used to store the value of Exclude. This will indicate that the document is excluded from the list.

                        }
                    }
                    foreach (var doc in entity.tbl_tailored_rep_task_ref_docs.ToList())
                    {
                        if (!(input.refDocs.Where(o => o.ID == doc.ID).Any()))
                        {
                            if (doc.TailorNew == true)
                            {
                                entity.tbl_tailored_rep_task_ref_docs.Remove(doc);
                            }
                            else
                            {
                                entity.tbl_tailored_rep_task_ref_docs.Where(o => o.ID == doc.ID).Single().Exclude = true;
                            }
                        }
                    }
                }
                else
                {
                    if (entity.tbl_tailored_rep_task_ref_docs != null && entity.tbl_tailored_rep_task_ref_docs.Any())
                    {
                        foreach (var doc in entity.tbl_tailored_rep_task_ref_docs.ToList())
                        {
                            if (doc.TailorNew == true)
                            {
                                entity.tbl_tailored_rep_task_ref_docs.Remove(doc);
                            }
                            else
                            {
                                entity.tbl_tailored_rep_task_ref_docs.Where(o => o.ID == doc.ID).Single().Exclude = true;
                            }
                        }
                    }
                }
                ctx.SaveChanges();
                return PartialView(input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                ModelState.AddModelError("", e.Message);
                return View(input);
            }
        }

        public ActionResult GetReferenceData(int id, int taskID, string selectedValue, string controlName, string reload)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.tbl_tailored_rep_task.Where(o => o.ID == taskID).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 500;
                    return Content("Record does not exist");
                }
                List<KeyListItem> returnList = new List<KeyListItem>();
                // Document
                var documents = ctx.tbl_tailored_rep_document.Where(o => o.tbl_Org_ProjectID == entity.tbl_Org_ProjectID && o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID && o.Exclude != true);
                if (documents != null && documents.Any())
                {
                    foreach (var doc in documents)
                    {
                        ctx.Entry(doc).Reference(o => o.tbl_process_document).Load();
                        ctx.Entry(doc.tbl_process_document).Reference(o => o.tbl_docmgr_document).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = doc.tbl_process_document.ID,
                            GroupName = "Documents",
                            Key = "D" + doc.tbl_process_document.ID,
                            DisplayText = doc.tbl_process_document.tbl_docmgr_document.Name
                        });
                    }
                }
                var procedures = ctx.tbl_tailored_rep_procedure.Where(o => o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID && o.Exclude != true);
                if (procedures != null && procedures.Any())
                {
                    foreach (var proc in procedures)
                    {
                        ctx.Entry(proc).Reference(o => o.tbl_process_procedure).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = proc.tbl_Process_ProcedureID,
                            GroupName = "Procedures",
                            Key = "P" + proc.tbl_Process_ProcedureID,
                            DisplayText = proc.tbl_process_procedure.Name
                        });
                    }
                }
                var templates = ctx.tbl_tailored_rep_template.Where(o => o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID && o.Exclude != true);
                if (templates != null && templates.Any())
                {
                    foreach (var tmpl in templates)
                    {
                        ctx.Entry(tmpl).Reference(o => o.tbl_process_template).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = tmpl.tbl_Process_TemplateID,
                            GroupName = "Templates",
                            Key = "T" + tmpl.tbl_Process_TemplateID,
                            DisplayText = tmpl.tbl_process_template.Name
                        });
                    }
                }
                var checklists = ctx.tbl_tailored_rep_chklst.Where(o => o.tbl_Process_RepositoryID == entity.tbl_Process_RepositoryID && o.Exclude != true);
                if (checklists != null && checklists.Any())
                {
                    foreach (var chklst in checklists)
                    {
                        ctx.Entry(chklst).Reference(o => o.tbl_process_checklist).Load();
                        returnList.Add(new KeyListItem()
                        {
                            ID = chklst.tbl_Process_ChecklistID,
                            GroupName = "Checklists",
                            Key = "C" + chklst.tbl_Process_ChecklistID,
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

        public ActionResult ExcludeRepo(int projectID, int repoID, int? projPhase)
        {
            try
            {
                if (projectID == 0 && repoID == 0)
                {
                    // At least one parameter should be provided
                    throw new PAException("Invalid Request");
                }
                var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                                      o.tbl_Process_RepositoryID == repoID &&
                                                                      ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase));
                if (entity == null || !entity.Any())
                {
                    Response.StatusCode = 500;
                    return Content("This record does not exist anymore");

                }
                foreach (var r in entity)
                {
                    r.Exclude = true;
                }
                service.Save();
                return Content("Process Excluded");
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult IncludeRepo(int projectID, int repoID, int? projPhase)
        {
            try
            {
                if (projectID == 0 && repoID == 0)
                {
                    // At least one parameter should be provided
                    throw new PAException("Invalid Request");
                }
                var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                                      o.tbl_Process_RepositoryID == repoID && 
                                                                      ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase));
                if (entity == null || !entity.Any())
                {
                    Response.StatusCode = 500;
                    return Content("This record does not exist anymore");

                }
                foreach (var r in entity)
                {
                    r.Exclude = false;
                }
                service.Save();
                return Content("Proecss Included");
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult IncludeTaskDoc(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_tailored_rep_task_ref_docs.Where(o => o.ID == id).SingleOrDefault();
            if (entity == null)
            {
                Response.StatusCode = 500;
                return Content("This record does not exist anymore");
            }
            entity.Exclude = false;

            ctx.SaveChanges();
            refDoc input = new refDoc()
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                DocType = entity.DocType,
                Mandatory = entity.Mandatory,
                Remarks = entity.Remarks,
                DocumentName = entity.Name,
                referenceID = (entity.DocType == 1 ? entity.tbl_Process_DocumentID.GetValueOrDefault() : 
                               (entity.DocType == 2 ? entity.tbl_Process_ProcedureID.GetValueOrDefault() : 
                               (entity.DocType == 3 ? entity.tbl_Process_TemplateID.GetValueOrDefault() : entity.tbl_Process_ChecklistID.GetValueOrDefault()))),
                refKey = (entity.DocType == 1 ? "D" + entity.tbl_Process_DocumentID.GetValueOrDefault() :
                               (entity.DocType == 2 ? "P" + entity.tbl_Process_ProcedureID.GetValueOrDefault() :
                               (entity.DocType == 3 ? "T" + entity.tbl_Process_TemplateID.GetValueOrDefault() : "C" + entity.tbl_Process_ChecklistID.GetValueOrDefault())))
            };
            ViewBag.TaskID = entity.tbl_Tailored_Rep_TaskID;
            ViewBag.Include = true;
            return PartialView(input);
        }

        public ActionResult manageDocuments(int projectID, int repoID, int? projPhase)
        {
            try
            {
                if (projectID == 0 && repoID == 0)
                {
                    // At least one parameter should be provided
                    throw new PAException("Invalid Request");
                }

                tbl_process_rep_details input = new tbl_process_rep_details();

                input.ID = projectID;
                input.repoID = repoID;
                input.phaseID = projPhase;
                input.key = repoID.ToString();

                var ctx = (Db)service.getRepo().getDBContext();
                var documents = ctx.tbl_tailored_rep_document.Include("tbl_process_document").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == projectID &&
                                                                      o.tbl_Process_RepositoryID == repoID &&
                                                                      ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &&
                                                                      (o.Exclude == false || o.Exclude == null));
                if (documents.Any())
                {
                    foreach (var doc in documents)
                    {
                        input.selectedOptions.Add((int)doc.tbl_process_document.tbl_DocMgr_DocumentID);
                    }
                }
                // ViewBag.postURL = Url.Action("manageDocuments", "PrjProcessTailor");
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
                    ViewBag.postURL = Url.Action("manageDocuments", "PrjProcessTailor");
                    return View("showDocuments", input);
                }

                var ctx = (Db)service.getRepo().getDBContext();
                var documents = ctx.tbl_tailored_rep_document.Include("tbl_process_document")
                                                             .Include("tbl_process_document.tbl_docmgr_document")
                                                             .Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                    o.tbl_Process_RepositoryID == input.repoID &&
                                                                    ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                var selectedIDs = "";

                using (TransactionScope scope = new TransactionScope())
                {
                    //service.getRepo().executeStoredCommand("delete from tbl_process_rep_procedure where tbl_Process_RepositoryID = " + input.ID); // delete existing mappings

                    if (input.selectedOptions.Any())
                    {
                        foreach (var g in input.selectedOptions)
                        {
                            var existingDocs = documents.Where(o => o.tbl_process_document.tbl_DocMgr_DocumentID == g).SingleOrDefault();
                            var prsDoc = ctx.tbl_process_document.Where(o => o.tbl_DocMgr_DocumentID == g).SingleOrDefault();
                            if (existingDocs == null)
                            {
                                ctx.tbl_tailored_rep_document.Add(new tbl_tailored_rep_document()
                                {
                                    ClientID = ((PAIdentity)User.Identity).clientID,
                                    tbl_Process_RepositoryID = input.repoID,
                                    tbl_Process_DocumentID = prsDoc.ID,
                                    tbl_Org_ProjectID = input.ID,
                                    mstr_Org_Proj_PhaseID = input.phaseID,
                                    TailorNew = true,
                                    Exclude = false
                                });
                            }
                            else
                            {
                                if (existingDocs.Exclude == true)
                                {
                                    existingDocs.Exclude = false;
                                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Org_ProjectID = " + input.ID +
                                        " and tbl_Process_RepositoryID = " + input.repoID +
                                        "  and DocType = 1 and (TailorNew = 0 or TailorNew is null) and tbl_Process_DocumentID = " + prsDoc.ID); // Exclude existing mappings

                                }
                            }
                            selectedIDs = selectedIDs + (selectedIDs == "" ? "" : ",") + prsDoc.ID;
                        }
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_document where tbl_Org_ProjectID = " + input.ID +
                                        " and tbl_Process_RepositoryID = " + input.repoID +
                                        (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                        " and TailorNew = 1 and tbl_Process_DocumentID not in (" + selectedIDs + ")"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_document set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and (TailorNew = 0 or TailorNew is null) and tbl_Process_DocumentID not in (" + selectedIDs + ")"); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 1 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null) and tbl_Process_DocumentID not in (" + selectedIDs + ") "); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 1 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1) and tbl_Process_DocumentID not in (" + selectedIDs + ") "); // Delete new tailored Mappings
                        ctx.SaveChanges();
                    }
                    else
                    {
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_document where tbl_Org_ProjectID = " + input.ID +
                                        " and tbl_Process_RepositoryID = " + input.repoID +
                                        (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                        " and TailorNew = 1"); // delete existing NEW mappings
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_document set Exclude=1 where tbl_Org_ProjectID = " + input.ID +
                                                                " and tbl_Process_RepositoryID = " + input.repoID +
                                                                (input.phaseID == null ? "" : " and mstr_Org_Proj_PhaseID = " + input.phaseID) +
                                                                " and (TailorNew = 0 or TailorNew is null)"); // Exclude existing mappings
                        // Exclude all instances of task supporting document where this template is being used
                        service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 1 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 0 or TailorNew is null)"); // Exclude existing mappings
                        service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 1 and tbl_Process_RepositoryID = " + input.repoID +
                                                                " and (TailorNew = 1)"); // Delete new tailored Mappings
                    }
                    scope.Complete();
                }
                documents = ctx.tbl_tailored_rep_document.Include("tbl_process_document").Include("tbl_process_document.tbl_docmgr_document").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_ProjectID == input.ID &&
                                                                    o.tbl_Process_RepositoryID == input.repoID &&
                                                                    ((input.phaseID == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == input.phaseID));
                ViewBag.projectID = input.ID;
                ViewBag.repoID = input.repoID;
                ViewBag.phaseID = input.phaseID;
                ViewBag.key = input.repoID.ToString();
                return View("getDocuments", documents);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult IncludeDoc(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var doc = ctx.tbl_tailored_rep_document.Include("tbl_process_document").Include("tbl_process_document.tbl_docmgr_document").Where(o => o.ID == id).SingleOrDefault();
                if (doc == null)
                {
                    return Content("<li id='LDocument'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    doc.Exclude = false;
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set RefDeleted = 0 where tbl_Process_RepositoryID = " + doc.tbl_Process_RepositoryID +
                        "  and DocType = 1  and tbl_Org_ProjectID = " + doc.tbl_Org_ProjectID +
                        " and tbl_Process_RepositoryID = " + doc.tbl_Process_RepositoryID +
                        " and (TailorNew = 0 or TailorNew is null) and tbl_Process_DocumentID = " + doc.tbl_Process_DocumentID); // Exclude existing mappings
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content("<li id='LDocument" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LDocument" + id + "' data-source='" + Url.Action("ExcludeDoc", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to exclude this document'><i class='icon-remove'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-configured'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("ViewDocument", "DocMgr", new { id = doc.tbl_process_document.tbl_docmgr_document.ID }) + "'>" +
                                            (doc.TailorNew == true ? doc.TailorName + " (Tailored: New)" : (doc.TailorName == null ? doc.tbl_process_document.tbl_docmgr_document.Name : doc.TailorName + " (Tailored: Changed)")) +
                                            "</a></span></li>");
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult ExcludeDoc(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var doc = ctx.tbl_tailored_rep_document.Include("tbl_process_document").Include("tbl_process_document.tbl_docmgr_document").Where(o => o.ID == id).SingleOrDefault();
                var returnInfo = "";

                if (doc == null)
                {
                    return Content("<li id='LDocument'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    if (doc.TailorNew == true) // This was a new task so physically delete it
                    {
                        ctx.tbl_tailored_rep_document.Remove(doc);
                        ctx.Entry(doc).State = System.Data.Entity.EntityState.Deleted;
                    }
                    else
                    {
                        doc.Exclude = true;    // Maintain this record and just logically delete it.
                    }
                    // Exclude all instances of task supporting document where this template is being used
                    service.getRepo().executeStoredCommand("update tbl_tailored_rep_task_ref_docs set Exclude = 1, RefDeleted = 1 where DocType = 1 and tbl_Org_ProjectID = " + doc.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + doc.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 0 or TailorNew is null) and tbl_Process_DocumentID = " + doc.tbl_Process_DocumentID); // Exclude existing mappings
                    service.getRepo().executeStoredCommand("delete from tbl_tailored_rep_task_ref_docs where DocType = 1  and tbl_Org_ProjectID = " + doc.tbl_Org_ProjectID +
                                                            " and tbl_Process_RepositoryID = " + doc.tbl_Process_RepositoryID +
                                                            " and (TailorNew = 1) and tbl_Process_DocumentID = " + doc.tbl_Process_DocumentID); // Delete new tailored Mappings
                    if (doc.TailorNew == null || doc.TailorNew == false)
                    {
                        // Return nothing as this record is now deleted
                        returnInfo = "<li id='LDocument" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LDocument" + id + "' data-source='" + Url.Action("IncludeDoc", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to include this document'><i class='icon-ok'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-strikeThrough'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("ViewDocument", "DocMgr", new { id = doc.tbl_process_document.tbl_docmgr_document.ID }) + "'>" +
                                            (doc.TailorName == null ? doc.tbl_process_document.tbl_docmgr_document.Name : doc.TailorName + " (Tailored: Changed)") +
                                            "</a></span>&nbsp;<span class='label label-danger'>Excluded</span></li>";
                    }
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content(returnInfo);
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult IncludeTask(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var tsk = ctx.tbl_tailored_rep_task.Include("tbl_process_rep_task").Where(o => o.ID == id).SingleOrDefault();
                if (tsk == null)
                {
                    return Content("<li id='LTask'" + id + "'>This record does not exist</li>");
                }
                tsk.Exclude = false;
                var repoEntry = ctx.tbl_org_project_process_mapping.Where(o => o.tbl_Org_ProjectID == tsk.tbl_Org_ProjectID &&
                                                                          o.tbl_Process_RepositoryID == tsk.tbl_Process_RepositoryID &&
                                                                          ((o.tbl_Process_TaskID == null && tsk.Ref_Rep_TaskID == null) || o.tbl_Process_TaskID == tsk.Ref_Rep_TaskID) &&
                                                                          o.tbl_Process_TaskID == tsk.Ref_Rep_TaskID).SingleOrDefault();
                repoEntry.Exclude = false;
                ctx.SaveChanges();
                return Content("<li id='LTask" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LTask" + id + "' data-source='" + Url.Action("ExcludeTask", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to exclude this task'><i class='icon-remove'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-configured'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showActivity", "PrjProcessTailor", new { id = tsk.ID }) + "'>" +
                                            (tsk.TailorNew == true ? tsk.TailorName + " (Tailored: New)" : (tsk.TailorName == null ? tsk.Name : tsk.TailorName + " (Tailored: Changed)")) +
                                            "</a></span></li>");
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult ExcludeTask(int id)
        {
            try
            {
                // Check if this record exists
                var ctx = (Db)service.getRepo().getDBContext();
                var tsk = ctx.tbl_tailored_rep_task.Include("tbl_process_rep_task").Where(o => o.ID == id).SingleOrDefault();
                var returnInfo = "";

                if (tsk == null)
                {
                    return Content("<li id='LTask'" + id + "'>This record does not exist</li>");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    if (tsk.TailorNew == true) // This was a new task so physically delete it
                    {
                        // Search repository for this record
                        var repoEntry = ctx.tbl_org_project_process_mapping.Where(o => o.tbl_Org_ProjectID == tsk.tbl_Org_ProjectID &&
                                                                                  o.tbl_Process_RepositoryID == tsk.tbl_Process_RepositoryID &&
                                                                                  ((o.tbl_Process_TaskID == null && tsk.Ref_Rep_TaskID == null) || o.tbl_Process_TaskID == tsk.Ref_Rep_TaskID) &&
                                                                                  o.tbl_Process_TaskID == tsk.Ref_Rep_TaskID).SingleOrDefault();
                        if (tsk.tbl_process_rep_task != null)
                        {
                            var task = tsk.tbl_process_rep_task;
                            tsk.Ref_Rep_TaskID = null;
                            tsk.tbl_process_rep_task = null;
                            ctx.tbl_process_rep_task.Remove(task);
                            ctx.Entry(task).State = System.Data.Entity.EntityState.Deleted;
                        }
                        ctx.tbl_tailored_rep_task.Remove(tsk);
                        ctx.Entry(tsk).State = System.Data.Entity.EntityState.Deleted;
                        ctx.tbl_org_project_process_mapping.Remove(repoEntry);
                        ctx.Entry(repoEntry).State = System.Data.Entity.EntityState.Deleted;
                    }
                    else
                    {
                        tsk.Exclude = true;    // Maintain this record and just logically delete it.
                        // Reflect changes in mapping table
                        var repoEntry = ctx.tbl_org_project_process_mapping.Where(o => o.tbl_Org_ProjectID == tsk.tbl_Org_ProjectID &&
                                                                                  o.tbl_Process_RepositoryID == tsk.tbl_Process_RepositoryID &&
                                                                                  ((o.tbl_Process_TaskID == null && tsk.Ref_Rep_TaskID == null) || o.tbl_Process_TaskID == tsk.Ref_Rep_TaskID) &&
                                                                                  o.tbl_Process_TaskID == tsk.Ref_Rep_TaskID).SingleOrDefault();
                        repoEntry.Exclude = true;
                    }
                    if (tsk.TailorNew == null || tsk.TailorNew == false)
                    {
                        // Return nothing as this record is now deleted
                        returnInfo = "<li id='LTask" + id + "'><a href='javascript:;' class='process' data-mode='edit' data-id='LTask" + id + "' data-source='" + Url.Action("IncludeTask", "PrjProcessTailor", new { id = id }) +
                                            "' data-param='" + id + "' data-message='Changes Saved' title='Click to include this task'><i class='icon-ok'></i></a>" +
                                            "&nbsp;&nbsp;<span class='label label-strikeThrough'><a href='javascript:' class='label-configured openDialog' data-id='' data-source='" +
                                            Url.Action("showActivity", "PrjProcessTailor", new { id = id }) + "'>" +
                                            (tsk.TailorName == null ? tsk.Name : (tsk.TailorName == tsk.Name ? tsk.Name : tsk.TailorName + " (Tailored: Changed)")) +
                                            "</a></span>&nbsp;<span class='label label-danger'>Excluded</span></li>";
                    }
                    ctx.SaveChanges();
                    scope.Complete();
                }
                return Content(returnInfo);
            }
            catch (PAException e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult addNewTask(int projectID, int repoID, int? projPhase)
        {
            tbl_process_rep_task_ref_docsInput input = new tbl_process_rep_task_ref_docsInput()
            {
                ClientID = ((PAIdentity)User.Identity).clientID,
                tbl_Org_ProjectID = projectID,
                tbl_Process_Repository_ID = repoID,
                mstr_Org_Proj_PhaseID = projPhase
            };
            return View(input);
        }

        [HttpPost]
        public ActionResult addNewTask(tbl_process_rep_task_ref_docsInput input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View(input);
            }
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var taskMaster = ctx.tbl_process_rep_task.Add(new tbl_process_rep_task()
                {
                    ClientID = input.ClientID,
                    CreateDate = System.DateTime.Now.Date,
                    CreatedBy = WebSecurity.CurrentUserId,
                    DefaultDurationDays = 8,
                    tbl_Process_RepositoryID = input.tbl_Process_Repository_ID,
                    SequenceNo = 0,
                    Name = input.Name,
                    Description = input.Description,
                    Type = input.Type,
                    Tailored = true,
                });
                var newRepositoryEntry = ctx.tbl_org_project_process_mapping.Add(new tbl_org_project_process_mapping()
                {
                    ClientID = input.ClientID,
                    tbl_Org_ProjectID = input.tbl_Org_ProjectID,
                    mstr_Org_Proj_PhaseID = input.mstr_Org_Proj_PhaseID,
                    tbl_Process_RepositoryID = input.tbl_Process_Repository_ID,
                    tbl_Process_TaskID = taskMaster.ID,
                    Exclude = false,
                    TreatAsTask = false,
                    TailorName = input.Name,
                    TailorNew = true
                });
                var newTask = ctx.tbl_tailored_rep_task.Add(new tbl_tailored_rep_task()
                    {
                        ClientID = input.ClientID,
                        tbl_Org_ProjectID = input.tbl_Org_ProjectID,
                        tbl_Process_RepositoryID = input.tbl_Process_Repository_ID,
                        Ref_Rep_TaskID = taskMaster.ID,
                        Name = input.Name,
                        TailorName = input.Name,
                        Description = input.Description,
                        Type = input.Type,
                        TailorNew = true,
                        Exclude = false
                    });
                ctx.SaveChanges();
                return PartialView("showNewTailorActivity", newTask);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                ModelState.AddModelError("", e.Message);
                return View(input);
            }
        }
    }
}

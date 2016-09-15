﻿using System.Web.Mvc;
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
    public class PProcedureController : Cruder<tbl_process_procedure, tbl_process_procedureInput>
    {
        protected Mapper<tbl_process_proc_section, tbl_process_proc_sectionInput> sectionMapper = new Mapper<tbl_process_proc_section, tbl_process_proc_sectionInput>();
        protected Mapper<tbl_process_proc_revision, tbl_process_proc_revisionInput> revisionMapper = new Mapper<tbl_process_proc_revision, tbl_process_proc_revisionInput>();
        protected Mapper<tbl_process_proc_group, tbl_process_proc_groupInput> groupMapper = new Mapper<tbl_process_proc_group, tbl_process_proc_groupInput>();

        public PProcedureController(ICrudService<tbl_process_procedure> service, IMapper<tbl_process_procedure, tbl_process_procedureInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSMTRPA")
        {
            functionID = "DFPRSMTRPA";
        }

        public override ActionResult GetItems()
        {
            IEnumerable<tbl_process_procedure> list;

            Db dbc = (Db)service.getRepo().getDBContext();
           
            if (Request.Form["srchByProcName"] != null && Request.Form["srchByProcName"] != "")
            {
                string searchParam = Request.Form["srchByProcName"].ToString();
                list = dbc.tbl_process_procedure.Include("UserProfile").Include("UserProfile1").Include("mstr_process_lc_status").Include("mstr_process_type").Where(rec => rec.Name.Contains(searchParam) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                list = dbc.tbl_process_procedure.Include("UserProfile").Include("UserProfile1").Include("mstr_process_lc_status").Include("mstr_process_type").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            return PartialView(list);
        }

        public override ActionResult Create()
        {
            tbl_process_procedureInput newProc = new tbl_process_procedureInput();

            Db dbc = (Db)service.getRepo().getDBContext();

            newProc.CreatedBy = WebSecurity.CurrentUserId;
            newProc.CreateDate = System.DateTime.Now.Date;
            newProc.CreatedByName = User.Identity.Name;
            var procStatus = dbc.mstr_process_lc_status.Where(s => s.Type == 1 && s.IsDefault == true).FirstOrDefault();
            newProc.mstr_Process_LC_StatusID = procStatus.ID;

            newProc.tbl_process_proc_revision.Add(new tbl_process_proc_revisionInput());
            newProc.tbl_process_proc_revision.FirstOrDefault().Comments = "Created First Version";
            newProc.tbl_process_proc_revision.FirstOrDefault().RevisionDate = System.DateTime.Now.Date;
            newProc.tbl_process_proc_revision.FirstOrDefault().RevisionUser = WebSecurity.CurrentUserId;
            newProc.tbl_process_proc_revision.FirstOrDefault().mstr_Process_LC_StatusID = procStatus.ID;

            return View(newProc);
        } 

        [HttpPost]
        public override ActionResult Create(tbl_process_procedureInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("Create", input);
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    // Save the procedure
                    var entity = createMapper.MapToEntity(input, new tbl_process_procedure());
                    entity.ClientID = ((PAIdentity)User.Identity).clientID;
                    var id = service.Create(entity);
                    var e = service.Get(id);

                    // Save the groups
                    foreach (var grp in input.tbl_process_proc_group)
                    {
                        grp.tbl_Process_ProcedureID = e.ID;
                        e.tbl_process_proc_group.Add(groupMapper.MapToEntity(grp, new tbl_process_proc_group()));
                    }


                    // Save the sections
                    foreach (var section in input.tbl_process_proc_section)
                    {
                        section.tbl_Process_ProcedureID = e.ID;
                        e.tbl_process_proc_section.Add(sectionMapper.MapToEntity(section, new tbl_process_proc_section()));
                    }

                    // Save revision
                    foreach (var revision in input.tbl_process_proc_revision)
                    {
                        revision.tbl_Process_ProcedureID = e.ID;
                        revision.version = "1";
                        e.tbl_process_proc_revision.Add(revisionMapper.MapToEntity(revision, new tbl_process_proc_revision()));
                    }
                    service.Save();  // Save related data
                    scope.Complete();
                }
                input.tbl_process_proc_revision.Add(new tbl_process_proc_revisionInput()
                {
                    mstr_Process_LC_StatusID = input.tbl_process_proc_revision.LastOrDefault().mstr_Process_LC_StatusID,
                    RevisionDate = System.DateTime.Now.Date,
                    RevisionUser = (int)input.CreatedBy,
                    version = input.tbl_process_proc_revision.LastOrDefault().version
                });  // Add a revision record ready for editing purposes

                input.UpdateDate = System.DateTime.Now.Date;    // Any updates should be saved as the current date
                input.UpdatedBy = input.CreatedBy;
                input.UpdatedByName = input.CreatedByName;

                return View("Edit", input );  // Show this newly saved added procedure for editing
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        } 

        public override ActionResult Edit(int id)
        {
            try
            {
                var entity = (tbl_process_procedure)service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_proc_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_proc_section).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_proc_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_procedureInput)editMapper.MapToInput(entity);  // Procedure details
                var currentUser = dbc.UserProfile.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                input.UpdatedBy = currentUser.ID;
                input.UpdatedByName = currentUser.UserName;
                input.UpdateDate = System.DateTime.Now.Date;

                input.tbl_process_proc_group = new List<tbl_process_proc_groupInput>();
                foreach (var grp in entity.tbl_process_proc_group)
                {
                    input.tbl_process_proc_group.Add((tbl_process_proc_groupInput) groupMapper.MapToInput(grp));
                }

                input.tbl_process_proc_section = new List<tbl_process_proc_sectionInput>();
                foreach (var section in entity.tbl_process_proc_section)
                {
                    input.tbl_process_proc_section.Add((tbl_process_proc_sectionInput)sectionMapper.MapToInput(section));
                }

                input.tbl_process_proc_revision = new List<tbl_process_proc_revisionInput>();

                foreach (var revision in entity.tbl_process_proc_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_proc_revision.Add((tbl_process_proc_revisionInput)revisionMapper.MapToInput(revision));
                }
                input.tbl_process_proc_revision.Add(new tbl_process_proc_revisionInput() 
                                    { mstr_Process_LC_StatusID = input.tbl_process_proc_revision.LastOrDefault().mstr_Process_LC_StatusID,
                                        RevisionDate = System.DateTime.Now.Date,
                                        RevisionUser = (int) input.UpdatedBy,
                                        version =  input.tbl_process_proc_revision.LastOrDefault().version });  // Add a revision record ready for editing purposes

                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_process_procedureInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View(input);
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    var e = editMapper.MapToEntity(input, service.Get(input.ID));

                    // Save groups
                    e.tbl_process_proc_group = new List<tbl_process_proc_group>();
                    foreach (var grp in input.tbl_process_proc_group)
                    {
                        grp.tbl_Process_ProcedureID = e.ID;
                        e.tbl_process_proc_group.Add(groupMapper.MapToEntity(grp, new tbl_process_proc_group()));
                    }

                    // Save the sections
                    e.tbl_process_proc_section = new List<tbl_process_proc_section>();
                    foreach (var section in input.tbl_process_proc_section)
                    {
                        section.tbl_Process_ProcedureID = e.ID;
                        e.tbl_process_proc_section.Add(sectionMapper.MapToEntity(section, new tbl_process_proc_section()));
                    }

                    
                    // Save revision
                    if (input.tbl_process_proc_revision.FirstOrDefault().Comments != null && input.tbl_process_proc_revision.FirstOrDefault().Comments != "")
                    {
                        e.tbl_process_proc_revision = new List<tbl_process_proc_revision>();
                        foreach (var revision in input.tbl_process_proc_revision)
                        {
                            revision.tbl_Process_ProcedureID = e.ID;
                            e.tbl_process_proc_revision.Add(revisionMapper.MapToEntity(revision, new tbl_process_proc_revision()));
                        }
                    }
                    // Delete related groups and sections for this procedure
                    service.getRepo().executeStoredCommand("delete from tbl_process_proc_section where tbl_Process_ProcedureID = " + input.ID);
                    service.getRepo().executeStoredCommand("delete from tbl_process_proc_group where tbl_Process_ProcedureID = " + input.ID);
                    service.Save();  // Save related data
                    scope.Complete();   // Reaching this point means everything has worked out fine.
                }
                input.tbl_process_proc_revision.Add(new tbl_process_proc_revisionInput() 
                                                    { mstr_Process_LC_StatusID = input.tbl_process_proc_revision.LastOrDefault().mstr_Process_LC_StatusID,
                                                      RevisionDate = System.DateTime.Now.Date,
                                                      RevisionUser = (int) input.UpdatedBy });  // Add a revision record ready for editing purposes

                return View("Edit", input);  // Show this newly saved added procedure for editing
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public virtual ActionResult showPreview(int id)
        {
            try
            {
                var entity = (tbl_process_procedure)service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_proc_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_proc_section).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_proc_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_procedureInput)editMapper.MapToInput(entity);  // Procedure details

                input.tbl_process_proc_group = new List<tbl_process_proc_groupInput>();
                foreach (var grp in entity.tbl_process_proc_group)
                {
                    input.tbl_process_proc_group.Add((tbl_process_proc_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_proc_section = new List<tbl_process_proc_sectionInput>();
                foreach (var section in entity.tbl_process_proc_section)
                {
                    input.tbl_process_proc_section.Add((tbl_process_proc_sectionInput)sectionMapper.MapToInput(section));
                }

                input.tbl_process_proc_revision = new List<tbl_process_proc_revisionInput>();

                foreach (var revision in entity.tbl_process_proc_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_proc_revision.Add((tbl_process_proc_revisionInput)revisionMapper.MapToInput(revision));
                }
                return View("Preview", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // Delete dependent data first
                    service.getRepo().executeStoredCommand("delete from tbl_process_proc_section where tbl_Process_ProcedureID = " + id);
                    service.getRepo().executeStoredCommand("delete from tbl_process_proc_group where tbl_Process_ProcedureID = " + id);
                    service.getRepo().executeStoredCommand("delete from tbl_process_proc_revision where tbl_Process_ProcedureID = " + id);

                    service.Delete(id);
                    scope.Complete();
                }
                return Json(new { Id = id, Type = typeof(tbl_process_procedure).Name.ToLower() });
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Json(new { Content = "Error" });
            }
        }

        [HttpPost]
        public ActionResult Preview(tbl_process_procedureInput input)
        {
            try
            {
                var ctx = (Db) service.getRepo().getDBContext();
                var client = ctx.mstr_client.Where(o => o.ID == ((PAIdentity)User.Identity).clientID).SingleOrDefault();

                if (client == null) throw new PAException("Client not found");

                input.companyName = client.ClientName;
                input.companyShortName = client.ShortName;
                input.companyLogo = client.Logo;

                return View("Preview", input);  // Show how this procedure will look
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult GetSections(int id)
        {
            try
            {
                Db dbc = (Db)service.getRepo().getDBContext();

                var list = dbc.vw_procedure.Where(o => o.ID == id).OrderBy(o => o.level).ThenBy(o => o.groupSequence).ThenBy(o => o.sectionSequence);

                var returnList = from node in list
                                 select new
                                 {
                                     ID = node.ID,
                                     GroupID = node.GroupID,
                                     GroupName = node.GroupName,
                                     Level = node.level,
                                     GroupSequence = node.groupSequence,
                                     ParentGroup = node.ParentGroup,
                                     sectionID = node.sectionID,
                                     sectionSequence = node.sectionSequence,
                                     Title = node.Title,
                                     Detail = node.Detail
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_process_procedure o)
        {
            return (o.Name == null) ? "" : o.Name;
        }
    }
}

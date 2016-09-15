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
    public class PChecklistController : Cruder<tbl_process_checklist, tbl_process_checklistInput>
    {
        protected tbl_process_chklst_itemMapper itemMapper = new tbl_process_chklst_itemMapper();
        protected Mapper<tbl_process_chklst_revision, tbl_process_chklst_revisionInput> revisionMapper = new Mapper<tbl_process_chklst_revision, tbl_process_chklst_revisionInput>();
        protected Mapper<tbl_process_chklst_group, tbl_process_chklst_groupInput> groupMapper = new Mapper<tbl_process_chklst_group, tbl_process_chklst_groupInput>();

        public PChecklistController(ICrudService<tbl_process_checklist> service, IMapper<tbl_process_checklist, tbl_process_checklistInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSMTRCKL")
        {
            functionID = "DFPRSMTRCKL";
        }

        public override ActionResult GetItems()
        {
            IEnumerable<tbl_process_checklist> list;

            Db dbc = (Db)service.getRepo().getDBContext();

            if (Request.Form["srchByChklstName"] != null && Request.Form["srchByChklstName"] != "")
            {
                list = dbc.tbl_process_checklist.Include("UserProfile").Include("UserProfile1").Include("mstr_process_lc_status").Where(rec => rec.Name.Contains(Request.Form["srchByChklstName"]) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                list = dbc.tbl_process_checklist.Include("UserProfile").Include("UserProfile1").Include("mstr_process_lc_status").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            return PartialView(list);
        }

        public override ActionResult Create()
        {
            tbl_process_checklistInput newChkLst = new tbl_process_checklistInput();

            Db dbc = (Db)service.getRepo().getDBContext();

            newChkLst.CreatedBy = WebSecurity.CurrentUserId;
            newChkLst.CreateDate = System.DateTime.Now.Date;
            newChkLst.CreatedByName = User.Identity.Name;
            var Status = dbc.mstr_process_lc_status.Where(s => s.Type == 1 && s.IsDefault == true).FirstOrDefault();
            newChkLst.mstr_Process_LC_StatusID = Status.ID;

            newChkLst.tbl_process_chklst_revision.Add(new tbl_process_chklst_revisionInput());
            newChkLst.tbl_process_chklst_revision.FirstOrDefault().Comments = "Created First Version";
            newChkLst.tbl_process_chklst_revision.FirstOrDefault().RevisionDate = System.DateTime.Now.Date;
            newChkLst.tbl_process_chklst_revision.FirstOrDefault().RevisionUser = WebSecurity.CurrentUserId;
            newChkLst.tbl_process_chklst_revision.FirstOrDefault().mstr_Process_LC_StatusID = Status.ID;
            // Load default values for custom labels and width
            newChkLst.C_SNO = "SNO";
            newChkLst.C_ChkPt = "Checkpoint";
            newChkLst.C_Result = "Result";
            newChkLst.C_Remarks = "Remarks";
            newChkLst.C_SNO_Len = 5;
            newChkLst.C_ChkPt_Len = 50;
            newChkLst.C_Result_Len = 20;
            newChkLst.C_Remarks_Len = 25;

            return View(newChkLst);
        }

        [HttpPost]
        public override ActionResult Create(tbl_process_checklistInput input)
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
                    // Save the checklist
                    var entity = createMapper.MapToEntity(input, new tbl_process_checklist());
                    entity.ClientID = ((PAIdentity)User.Identity).clientID;
                    var id = service.Create(entity);
                    var e = service.Get(id);

                    // Save the groups
                    foreach (var grp in input.tbl_process_chklst_group)
                    {
                        grp.tbl_Process_ChecklistID = e.ID;
                        e.tbl_process_chklst_group.Add(groupMapper.MapToEntity(grp, new tbl_process_chklst_group()));
                    }


                    // Save the items
                    foreach (var item in input.tbl_process_chklst_item)
                    {
                        item.tbl_Process_ChecklistID = e.ID;
                        e.tbl_process_chklst_item.Add(itemMapper.MapToEntity(item, new tbl_process_chklst_item()));
                    }

                    // Save revision
                    foreach (var revision in input.tbl_process_chklst_revision)
                    {
                        revision.tbl_Process_ChecklistID = e.ID;
                        revision.version = "1";
                        e.tbl_process_chklst_revision.Add(revisionMapper.MapToEntity(revision, new tbl_process_chklst_revision()));
                    }
                    service.Save();  // Save related data
                    scope.Complete();
                }
                input.tbl_process_chklst_revision.Add(new tbl_process_chklst_revisionInput()
                {
                    mstr_Process_LC_StatusID = input.tbl_process_chklst_revision.LastOrDefault().mstr_Process_LC_StatusID,
                    RevisionDate = System.DateTime.Now.Date,
                    RevisionUser = (int)input.CreatedBy,
                    version = input.tbl_process_chklst_revision.LastOrDefault().version
                });  // Add a revision record ready for editing purposes

                input.UpdateDate = System.DateTime.Now.Date;    // Any updates should be saved as the current date
                input.UpdatedBy = input.CreatedBy;
                input.UpdatedByName = input.CreatedByName;

                return View("Edit", input);  // Show this newly saved added checklist for editing
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
                var entity = (tbl_process_checklist)service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_item).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_checklistInput)editMapper.MapToInput(entity);  // Checklist details
                var currentUser = dbc.UserProfile.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                input.UpdatedBy = currentUser.ID;
                input.UpdatedByName = currentUser.UserName;
                input.UpdateDate = System.DateTime.Now.Date;

                input.tbl_process_chklst_group = new List<tbl_process_chklst_groupInput>();
                foreach (var grp in entity.tbl_process_chklst_group)
                {
                    input.tbl_process_chklst_group.Add((tbl_process_chklst_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_chklst_item = new List<tbl_process_chklst_itemInput>();
                foreach (var item in entity.tbl_process_chklst_item)
                {
                    input.tbl_process_chklst_item.Add((tbl_process_chklst_itemInput)itemMapper.MapToInput(item));
                }

                input.tbl_process_chklst_revision = new List<tbl_process_chklst_revisionInput>();

                foreach (var revision in entity.tbl_process_chklst_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_chklst_revision.Add((tbl_process_chklst_revisionInput)revisionMapper.MapToInput(revision));
                }
                input.tbl_process_chklst_revision.Add(new tbl_process_chklst_revisionInput()
                {
                    mstr_Process_LC_StatusID = input.tbl_process_chklst_revision.LastOrDefault().mstr_Process_LC_StatusID,
                    RevisionDate = System.DateTime.Now.Date,
                    RevisionUser = (int)input.UpdatedBy,
                    version = input.tbl_process_chklst_revision.LastOrDefault().version
                });  // Add a revision record ready for editing purposes

                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_process_checklistInput input)
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
                    e.ClientID = ((PAIdentity)User.Identity).clientID;
                    // Save groups
                    e.tbl_process_chklst_group = new List<tbl_process_chklst_group>();
                    foreach (var grp in input.tbl_process_chklst_group)
                    {
                        grp.tbl_Process_ChecklistID = e.ID;
                        e.tbl_process_chklst_group.Add(groupMapper.MapToEntity(grp, new tbl_process_chklst_group()));
                    }

                    // Save the items
                    e.tbl_process_chklst_item = new List<tbl_process_chklst_item>();
                    foreach (var item in input.tbl_process_chklst_item)
                    {
                        item.tbl_Process_ChecklistID = e.ID;
                        e.tbl_process_chklst_item.Add(itemMapper.MapToEntity(item, new tbl_process_chklst_item()));
                    }

                    // Save revision
                    if (input.tbl_process_chklst_revision.FirstOrDefault().Comments != null && input.tbl_process_chklst_revision.FirstOrDefault().Comments != "")
                    {
                        e.tbl_process_chklst_revision = new List<tbl_process_chklst_revision>();
                        foreach (var revision in input.tbl_process_chklst_revision)
                        {
                            revision.tbl_Process_ChecklistID = e.ID;
                            e.tbl_process_chklst_revision.Add(revisionMapper.MapToEntity(revision, new tbl_process_chklst_revision()));
                        }
                    }
                    // Delete related groups and items for this checklist
                    service.getRepo().executeStoredCommand("delete from tbl_process_chklst_item where tbl_Process_ChecklistID = " + input.ID);
                    service.getRepo().executeStoredCommand("delete from tbl_process_chklst_group where tbl_Process_ChecklistID = " + input.ID);
                    service.Save();  // Save related data
                    scope.Complete();   // Reaching this point means everything has worked out fine.
                }
                input.tbl_process_chklst_revision.Add(new tbl_process_chklst_revisionInput()
                {
                    mstr_Process_LC_StatusID = input.tbl_process_chklst_revision.LastOrDefault().mstr_Process_LC_StatusID,
                    RevisionDate = System.DateTime.Now.Date,
                    RevisionUser = (int)input.UpdatedBy
                });  // Add a revision record ready for editing purposes

                return View("Edit", input);  // Show this newly saved added checklist for editing
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
                    service.getRepo().executeStoredCommand("delete from tbl_process_chklst_item where tbl_Process_ChecklistID = " + id);
                    service.getRepo().executeStoredCommand("delete from tbl_process_chklst_group where tbl_Process_ChecklistID = " + id);
                    service.getRepo().executeStoredCommand("delete from tbl_process_chklst_revision where tbl_Process_ChecklistID = " + id);

                    service.Delete(id);
                    scope.Complete();
                }
                return Json(new { Id = id, Type = typeof(tbl_process_checklist).Name.ToLower() });
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Json(new { Content = "Error" });
            }
        }

        public virtual ActionResult showPreview(int id)
        {
            try
            {
                var entity = (tbl_process_checklist)service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_item).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_checklistInput)editMapper.MapToInput(entity);  // Procedure details

                input.tbl_process_chklst_group = new List<tbl_process_chklst_groupInput>();
                foreach (var grp in entity.tbl_process_chklst_group)
                {
                    input.tbl_process_chklst_group.Add((tbl_process_chklst_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_chklst_item = new List<tbl_process_chklst_itemInput>();
                foreach (var section in entity.tbl_process_chklst_item)
                {
                    input.tbl_process_chklst_item.Add((tbl_process_chklst_itemInput)itemMapper.MapToInput(section));
                }

                input.tbl_process_chklst_revision = new List<tbl_process_chklst_revisionInput>();

                foreach (var revision in entity.tbl_process_chklst_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_chklst_revision.Add((tbl_process_chklst_revisionInput)revisionMapper.MapToInput(revision));
                }
                return View("Preview", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Preview(tbl_process_checklistInput input)
        {
            try
            {
                return View("Preview", input);  // Show how this checklist will look
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

        protected override string listDisplayName(tbl_process_checklist o)
        {
            return (o.Name == null) ? "" : o.Name;
        }

        public virtual ActionResult FillChecklist(int planID, int checklistID, int docID, int resourceID)
        {
            try
            {
                var entity = (tbl_process_checklist)service.Get(checklistID);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_item).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_checklistInput)editMapper.MapToInput(entity);  // Procedure details

                input.tbl_process_chklst_group = new List<tbl_process_chklst_groupInput>();
                foreach (var grp in entity.tbl_process_chklst_group)
                {
                    input.tbl_process_chklst_group.Add((tbl_process_chklst_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_chklst_item = new List<tbl_process_chklst_itemInput>();
                foreach (var section in entity.tbl_process_chklst_item)
                {
                    input.tbl_process_chklst_item.Add((tbl_process_chklst_itemInput)itemMapper.MapToInput(section));
                }

                input.tbl_process_chklst_revision = new List<tbl_process_chklst_revisionInput>();

                foreach (var revision in entity.tbl_process_chklst_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_chklst_revision.Add((tbl_process_chklst_revisionInput)revisionMapper.MapToInput(revision));
                }
                ViewBag.PlanID = planID;
                ViewBag.DocID = docID;
                ViewBag.ResourceID = resourceID;
                ViewBag.Editable = true;

                return View("FillChecklist", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public virtual ActionResult FillProjectChecklist(int projectID, int checklistID)
        {
            try
            {
                var entity = (tbl_process_checklist)service.Get(checklistID);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_item).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_chklst_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_checklistInput)editMapper.MapToInput(entity);  // Procedure details

                input.tbl_process_chklst_group = new List<tbl_process_chklst_groupInput>();
                foreach (var grp in entity.tbl_process_chklst_group)
                {
                    input.tbl_process_chklst_group.Add((tbl_process_chklst_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_chklst_item = new List<tbl_process_chklst_itemInput>();
                foreach (var section in entity.tbl_process_chklst_item)
                {
                    input.tbl_process_chklst_item.Add((tbl_process_chklst_itemInput)itemMapper.MapToInput(section));
                }

                input.tbl_process_chklst_revision = new List<tbl_process_chklst_revisionInput>();

                foreach (var revision in entity.tbl_process_chklst_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_chklst_revision.Add((tbl_process_chklst_revisionInput)revisionMapper.MapToInput(revision));
                }
                ViewBag.ProjectID = projectID;
                ViewBag.Editable = true;

                return View("FillChecklist", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }


    }
}

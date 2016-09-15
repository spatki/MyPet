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
    public class PTemplateController : Cruder<tbl_process_template, tbl_process_templateInput>
    {
        protected Mapper<tbl_process_tmpl_section, tbl_process_tmpl_sectionInput> sectionMapper = new Mapper<tbl_process_tmpl_section, tbl_process_tmpl_sectionInput>();
        protected Mapper<tbl_process_tmpl_revision, tbl_process_tmpl_revisionInput> revisionMapper = new Mapper<tbl_process_tmpl_revision, tbl_process_tmpl_revisionInput>();
        protected Mapper<tbl_process_tmpl_group, tbl_process_tmpl_groupInput> groupMapper = new Mapper<tbl_process_tmpl_group, tbl_process_tmpl_groupInput>();

        public PTemplateController(ICrudService<tbl_process_template> service, IMapper<tbl_process_template, tbl_process_templateInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSMTRTMP")
        {
            functionID = "DFPRSMTRTMP";
        }

        public override ActionResult GetItems()
        {
            IEnumerable<tbl_process_template> list;

            Db dbc = (Db)service.getRepo().getDBContext();

            if (Request.Form["srchByTmplName"] != null && Request.Form["srchByTmplName"] != "")
            {
                list = dbc.tbl_process_template.Include("UserProfile").Include("UserProfile1").Include("mstr_process_lc_status").Where(rec => rec.Name.Contains(Request.Form["srchByTmplName"]) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            else
            {
                list = dbc.tbl_process_template.Include("UserProfile").Include("UserProfile1").Include("mstr_process_lc_status").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
            }
            return PartialView(list);
        }

        public override ActionResult Create()
        {
            tbl_process_templateInput newTmplt = new tbl_process_templateInput();

            Db dbc = (Db)service.getRepo().getDBContext();

            newTmplt.CreatedBy = WebSecurity.CurrentUserId;
            newTmplt.CreateDate = System.DateTime.Now.Date;
            newTmplt.CreatedByName = User.Identity.Name;
            var procStatus = dbc.mstr_process_lc_status.Where(s => s.Type == 1 && s.IsDefault == true).FirstOrDefault();
            newTmplt.mstr_Process_LC_StatusID = procStatus.ID;

            newTmplt.tbl_process_tmpl_revision.Add(new tbl_process_tmpl_revisionInput());
            newTmplt.tbl_process_tmpl_revision.FirstOrDefault().Comments = "Created First Version";
            newTmplt.tbl_process_tmpl_revision.FirstOrDefault().RevisionDate = System.DateTime.Now.Date;
            newTmplt.tbl_process_tmpl_revision.FirstOrDefault().RevisionUser = WebSecurity.CurrentUserId;
            newTmplt.tbl_process_tmpl_revision.FirstOrDefault().mstr_Process_LC_StatusID = procStatus.ID;

            return View(newTmplt);
        }

        [HttpPost]
        public override ActionResult Create(tbl_process_templateInput input)
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
                    // Save the template
                    var entity = createMapper.MapToEntity(input, new tbl_process_template());
                    entity.ClientID = ((PAIdentity)User.Identity).clientID;
                    entity.CreateDate = System.DateTime.Now.Date;
                    var id = service.Create(entity);
                    var e = service.Get(id);

                    // Save the groups
                    foreach (var grp in input.tbl_process_tmpl_group)
                    {
                        grp.tbl_Process_TemplateID = e.ID;
                        e.tbl_process_tmpl_group.Add(groupMapper.MapToEntity(grp, new tbl_process_tmpl_group()));
                    }


                    // Save the sections
                    foreach (var section in input.tbl_process_tmpl_section)
                    {
                        section.tbl_Process_TemplateID = e.ID;
                        e.tbl_process_tmpl_section.Add(sectionMapper.MapToEntity(section, new tbl_process_tmpl_section()));
                    }

                    // Save revision
                    foreach (var revision in input.tbl_process_tmpl_revision)
                    {
                        revision.tbl_Process_TemplateID = e.ID;
                        revision.version = "1";
                        e.tbl_process_tmpl_revision.Add(revisionMapper.MapToEntity(revision, new tbl_process_tmpl_revision()));
                    }
                    service.Save();  // Save related data
                    scope.Complete();
                }
                input.tbl_process_tmpl_revision.Add(new tbl_process_tmpl_revisionInput()
                {
                    mstr_Process_LC_StatusID = input.tbl_process_tmpl_revision.LastOrDefault().mstr_Process_LC_StatusID,
                    RevisionDate = System.DateTime.Now.Date,
                    RevisionUser = (int)input.CreatedBy,
                    version = input.tbl_process_tmpl_revision.LastOrDefault().version
                });  // Add a revision record ready for editing purposes

                input.UpdateDate = System.DateTime.Now.Date;    // Any updates should be saved as the current date
                input.UpdatedBy = input.CreatedBy;
                input.UpdatedByName = input.CreatedByName;

                return View("Edit", input);  // Show this newly saved added template for editing
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
                var entity = (tbl_process_template)service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_section).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_templateInput)editMapper.MapToInput(entity);  // Template details
                var currentUser = dbc.UserProfile.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                input.UpdatedBy = currentUser.ID;
                input.UpdatedByName = currentUser.UserName;
                input.UpdateDate = System.DateTime.Now.Date;

                input.tbl_process_tmpl_group = new List<tbl_process_tmpl_groupInput>();
                foreach (var grp in entity.tbl_process_tmpl_group)
                {
                    input.tbl_process_tmpl_group.Add((tbl_process_tmpl_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_tmpl_section = new List<tbl_process_tmpl_sectionInput>();
                foreach (var section in entity.tbl_process_tmpl_section)
                {
                    input.tbl_process_tmpl_section.Add((tbl_process_tmpl_sectionInput)sectionMapper.MapToInput(section));
                }

                input.tbl_process_tmpl_revision = new List<tbl_process_tmpl_revisionInput>();

                foreach (var revision in entity.tbl_process_tmpl_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_tmpl_revision.Add((tbl_process_tmpl_revisionInput)revisionMapper.MapToInput(revision));
                }
                input.tbl_process_tmpl_revision.Add(new tbl_process_tmpl_revisionInput()
                {
                    mstr_Process_LC_StatusID = input.tbl_process_tmpl_revision.LastOrDefault().mstr_Process_LC_StatusID,
                    RevisionDate = System.DateTime.Now.Date,
                    RevisionUser = (int)input.UpdatedBy,
                    version = input.tbl_process_tmpl_revision.LastOrDefault().version
                });  // Add a revision record ready for editing purposes

                return View("Edit", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Edit(tbl_process_templateInput input)
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
                    e.tbl_process_tmpl_group = new List<tbl_process_tmpl_group>();
                    foreach (var grp in input.tbl_process_tmpl_group)
                    {
                        grp.tbl_Process_TemplateID = e.ID;
                        grp.ClientID = e.ClientID;
                        e.tbl_process_tmpl_group.Add(groupMapper.MapToEntity(grp, new tbl_process_tmpl_group()));
                    }

                    // Save the sections
                    e.tbl_process_tmpl_section = new List<tbl_process_tmpl_section>();
                    foreach (var section in input.tbl_process_tmpl_section)
                    {
                        section.tbl_Process_TemplateID = e.ID;
                        section.ClientID = e.ClientID;
                        //section.Detail = section.Detail.Replace("&nbsp;", "&#160;");        // Handling &nbsp; which is missing in ckeditor XML Schema...
                        e.tbl_process_tmpl_section.Add(sectionMapper.MapToEntity(section, new tbl_process_tmpl_section()));
                    }

                    // Save revision
                    if (input.tbl_process_tmpl_revision.FirstOrDefault().Comments != null && input.tbl_process_tmpl_revision.FirstOrDefault().Comments != "")
                    {
                        e.tbl_process_tmpl_revision = new List<tbl_process_tmpl_revision>();
                        foreach (var revision in input.tbl_process_tmpl_revision)
                        {
                            revision.tbl_Process_TemplateID = e.ID;
                            revision.ClientID = e.ClientID;
                            e.tbl_process_tmpl_revision.Add(revisionMapper.MapToEntity(revision, new tbl_process_tmpl_revision()));
                        }
                    }
                    // Delete related groups and sections for this template
                    service.getRepo().executeStoredCommand("delete from tbl_process_tmpl_section where tbl_Process_TemplateID = " + input.ID);
                    service.getRepo().executeStoredCommand("delete from tbl_process_tmpl_group where tbl_Process_TemplateID = " + input.ID);
                    service.Save();  // Save related data
                    scope.Complete();   // Reaching this point means everything has worked out fine.
                }
                input.tbl_process_tmpl_revision.Add(new tbl_process_tmpl_revisionInput()
                {
                    mstr_Process_LC_StatusID = input.tbl_process_tmpl_revision.LastOrDefault().mstr_Process_LC_StatusID,
                    RevisionDate = System.DateTime.Now.Date,
                    RevisionUser = (int)input.UpdatedBy
                });  // Add a revision record ready for editing purposes

                return View("Edit", input);  // Show this newly saved added template for editing
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
                    service.getRepo().executeStoredCommand("delete from tbl_process_tmpl_group where tbl_Process_TemplateID = " + id);
                    service.getRepo().executeStoredCommand("delete from tbl_process_tmpl_section where tbl_Process_TemplateID = " + id);
                    service.getRepo().executeStoredCommand("delete from tbl_process_tmpl_revision where tbl_Process_TemplateID = " + id);

                    service.Delete(id);
                    scope.Complete();
                }
                return Json(new { Id = id, Type = typeof(tbl_process_template).Name.ToLower() });
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
                var entity = (tbl_process_template)service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_section).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_templateInput)editMapper.MapToInput(entity);  // Procedure details

                input.tbl_process_tmpl_group = new List<tbl_process_tmpl_groupInput>();
                foreach (var grp in entity.tbl_process_tmpl_group)
                {
                    input.tbl_process_tmpl_group.Add((tbl_process_tmpl_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_tmpl_section = new List<tbl_process_tmpl_sectionInput>();
                foreach (var section in entity.tbl_process_tmpl_section)
                {
                    input.tbl_process_tmpl_section.Add((tbl_process_tmpl_sectionInput)sectionMapper.MapToInput(section));
                }

                input.tbl_process_tmpl_revision = new List<tbl_process_tmpl_revisionInput>();

                foreach (var revision in entity.tbl_process_tmpl_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_tmpl_revision.Add((tbl_process_tmpl_revisionInput)revisionMapper.MapToInput(revision));
                }
                return View("Preview", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Preview(tbl_process_templateInput input)
        {
            try
            {
                return View("Preview", input);  // Show how this template will look
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

        protected override string listDisplayName(tbl_process_template o)
        {
            return (o.Name == null) ? "" : o.Name;
        }

        public ActionResult FillTemplate(int planID, int templateID, int docID, int resourceID)
        {
            try
            {
                var entity = (tbl_process_template)service.Get(templateID);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_section).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_templateInput)editMapper.MapToInput(entity);  // Procedure details

                input.tbl_process_tmpl_group = new List<tbl_process_tmpl_groupInput>();
                foreach (var grp in entity.tbl_process_tmpl_group)
                {
                    input.tbl_process_tmpl_group.Add((tbl_process_tmpl_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_tmpl_section = new List<tbl_process_tmpl_sectionInput>();
                foreach (var section in entity.tbl_process_tmpl_section)
                {
                    input.tbl_process_tmpl_section.Add((tbl_process_tmpl_sectionInput)sectionMapper.MapToInput(section));
                }

                input.tbl_process_tmpl_revision = new List<tbl_process_tmpl_revisionInput>();

                foreach (var revision in entity.tbl_process_tmpl_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_tmpl_revision.Add((tbl_process_tmpl_revisionInput)revisionMapper.MapToInput(revision));
                }
                ViewBag.PlanID = planID;
                ViewBag.DocID = docID;
                ViewBag.ResourceID = resourceID;
                ViewBag.Editable = true;

                return View("FillTemplate", input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult FillProjectTemplate(int projectID, int templateID)
        {
            try
            {
                var entity = (tbl_process_template)service.Get(templateID);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                // Map to input

                // Load related details
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_group).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_section).Load();
                service.getRepo().getDBContext().Entry(entity).Collection(x => x.tbl_process_tmpl_revision).Load();

                Db dbc = (Db)service.getRepo().getDBContext();

                var input = (tbl_process_templateInput)editMapper.MapToInput(entity);  // Procedure details

                input.tbl_process_tmpl_group = new List<tbl_process_tmpl_groupInput>();
                foreach (var grp in entity.tbl_process_tmpl_group)
                {
                    input.tbl_process_tmpl_group.Add((tbl_process_tmpl_groupInput)groupMapper.MapToInput(grp));
                }

                input.tbl_process_tmpl_section = new List<tbl_process_tmpl_sectionInput>();
                foreach (var section in entity.tbl_process_tmpl_section)
                {
                    input.tbl_process_tmpl_section.Add((tbl_process_tmpl_sectionInput)sectionMapper.MapToInput(section));
                }

                input.tbl_process_tmpl_revision = new List<tbl_process_tmpl_revisionInput>();

                foreach (var revision in entity.tbl_process_tmpl_revision.OrderBy(o => o.ID))
                {
                    input.tbl_process_tmpl_revision.Add((tbl_process_tmpl_revisionInput)revisionMapper.MapToInput(revision));
                }
                ViewBag.ProjectID = projectID;
                ViewBag.Editable = true;

                return View("FillTemplate", input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

    }
}

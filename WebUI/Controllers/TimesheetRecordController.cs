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
using System.Data;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class TimesheetRecordController : Cruder<tbl_org_plan_filled_document, tbl_org_plan_filled_documentInput>
    {
        public TimesheetRecordController(ICrudService<tbl_org_plan_filled_document> service, IMapper<tbl_org_plan_filled_document, tbl_org_plan_filled_documentInput> v, IWorkflowService wf)
            : base(service, v, wf, "RECPRTSK")
        {
            functionID = "RECPRTSK";
        }
       
        protected override string RowViewName
        {
            get { return "Edit"; }
        }

        public ActionResult RecordingIndex()
        {
            return View();
        }

        public ActionResult ViewProcessDocuments(int id)
        {
            if (id == 0) return null;
            var ctx = (Db)service.getRepo().getDBContext();

            var entity = ctx.vw_task_recording.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.projectID == id && o.filled_DocID != null);
            return View(entity);
        }

        public ActionResult ViewRecord(int id)
        {
            var entity = service.Get(id);
            return View(entity);
        }

        public ActionResult ProjectRecording(int Project)
        {
            if (Project == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Select a project and try again.";
                return View("ListItems/showError");
            }
            int empID;

            var ctx = (Db)service.getRepo().getDBContext();
            var UserDetails = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            if (UserDetails.EmployeeID == null)    // Check if user is an employee.
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "This user is not an employee. Hence timesheets are not available for this user.";
                return View("ListItems/showError");
            }
            else
            {
                empID = UserDetails.EmployeeID.GetValueOrDefault();
            }

            ViewBag.ProjectID = Project;
            ViewBag.EmployeeID = empID;
            var entity = ctx.vw_task_recording.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.projectID == Project && o.tbl_Org_EmployeeID == empID
                                                     && (o.DocType == 3 || o.DocType == 4));
            return View(entity);
        }

        public ActionResult FillRecord(int docID, int resourceID, int? fillID)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var planDocument = ctx.tbl_org_plan_document.Include("tbl_org_proj_plan").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == docID).SingleOrDefault();
                if (planDocument == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Document does not exist";
                    return View("ListItems/showError");
                }
                ViewBag.ProjectID = planDocument.tbl_org_proj_plan.tbl_Org_ProjectID;
                fillID = fillID.GetValueOrDefault();
                var filledDoc = service.Get((int)fillID);
                if (filledDoc == null)
                {
                    if (planDocument.DocType == 3)      // Template
                    {
                        return RedirectToAction("FillTemplate", "PTemplate", new { planID = planDocument.tbl_Org_PlanID, templateID = planDocument.tbl_Process_TemplateID, docID = planDocument.ID, resourceID = resourceID });
                    }
                    else
                    {
                        return RedirectToAction("FillChecklist", "PChecklist", new { planID = planDocument.tbl_Org_PlanID, checklistID = planDocument.tbl_Process_ChecklistID, docID = planDocument.ID, resourceID = resourceID });
                    }
                }
                return View("FillRecord", filledDoc);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public ActionResult saveRecord(tbl_org_plan_filled_documentInput input)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_PlanID == input.tbl_Org_PlanID
                                                                    && o.tbl_Org_Plan_ResourceID == input.tbl_Org_Plan_ResourceID && o.tbl_Org_Plan_DocumentID == input.tbl_Org_Plan_DocumentID).SingleOrDefault();

                input.ClientID = ((PAIdentity)User.Identity).clientID;
                Mapper<tbl_org_plan_filled_document, tbl_org_plan_filled_documentInput> planDocMapper = new Mapper<tbl_org_plan_filled_document, tbl_org_plan_filled_documentInput>();
                if (input.ID == 0)
                {
                    // New record
                    entity = ctx.tbl_org_plan_filled_document.Add(planDocMapper.MapToEntity(input, new tbl_org_plan_filled_document()));
                }
                else
                {
                    // Existing record
                    entity = planDocMapper.MapToEntity(input, entity);
                }
                ctx.SaveChanges();
                var vw_entity = ctx.vw_task_recording.Where(o => o.filled_DocID == entity.ID);
                return View("ProjectRecording", vw_entity);
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

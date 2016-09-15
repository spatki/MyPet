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
using System.IO;
using System.Web;
using System;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class AuditFindingController : Cruder<tbl_audit_schedule, auditRecordingInput>
    {

        public AuditFindingController(AuditFindingService service, audit_recordingMapper v, IWorkflowService wf)
            : base(service, v, wf, "ADTPRJCD")
        {
            functionID = "ADTPRJCD";
        }

        protected override string RowViewName
        {
            get { return "LandingPage"; }
        }

        public ActionResult GetAudits(int id)
        {
            // Input parameter id is the project id
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_audit_schedule.Include("tbl_audit_plan").Include("tbl_audit_participant").Include("tbl_audit_observation").Include("tbl_audit_checklist").Where(o => o.tbl_Org_ProjectID == id);

            return View("GetItems",entity);
        }

        public ActionResult GetAuditChecklist(int id, int auditScheduleID)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.vw_audit_pci_list.Where(o => o.tbl_Org_ProjectID == id);
            ViewBag.ProjectID = id;
            ViewBag.AuditScheduleID = auditScheduleID;
            return View(entity);
        }

        public ActionResult ProjectIndex(int id, string viewName)
        {
            ViewBag.ProjectID = id;
            return View(viewName);
        }

        public ActionResult AddNC(int id, int key)
        {
            ViewBag.Key = key;

            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_audit_schedule.Include("tbl_audit_participant").Include("tbl_audit_participant.tbl_org_employee").Where(o => o.ID == id).SingleOrDefault();
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "This audit schedule does not exist";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            return View(entity);
        }

        public ActionResult AddObs(int id, int key)
        {
            ViewBag.Key = key;

            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_audit_schedule.Include("tbl_audit_participant").Include("tbl_audit_participant.tbl_org_employee").Where(o => o.ID == id).SingleOrDefault();
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "This audit schedule does not exist";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            return View(entity);
        }

        public ActionResult DeleteObservation(int id, int scheduleID)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var obs = ctx.tbl_audit_observation.Where(o => o.ID == id && o.tbl_Audit_ScheduleID == scheduleID).SingleOrDefault();
                if (obs != null)
                {
                    ctx.tbl_audit_observation.Remove(obs);
                    ctx.SaveChanges();
                }
                return Content("Succcessful");
            }
            catch (Exception e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Application Error: " + e.Message;
                return View("ListItems/showError");	// Return error in a dialog box
            }
        }

        protected override void ReSequenceBeforeEdit(auditRecordingInput input) 
        {
            if (input.Status == 2) input.Status = 3;
        }

        public override ActionResult ProcessView(IEnumerable<tbl_audit_schedule> entity)
        {
            var e = entity.FirstOrDefault();
            if (e == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Audit Finding does not exist";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            return View("Edit", editMapper.MapToInput(e));
        }

        public ActionResult ShowReadOnlyReport(int id)
        {
            // Here id is the schedule id
            try
            {
                //var entity = new TEntity();
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                ViewBag.Mode = 2;
                return View("ShowReport",editMapper.MapToInput(entity));
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult ShowReport(int id)
        {
            // Here id is the schedule id
            try
            {
                //var entity = new TEntity();
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                ViewBag.Mode = 1;
                return View(editMapper.MapToInput(entity));
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult GetAuditsForClosure(int id)
        {
            // here id is the project id
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_audit_schedule.Include("tbl_audit_plan").Include("tbl_audit_participant").Include("tbl_audit_observation").Include("tbl_audit_checklist").Where(o => o.tbl_Org_ProjectID == id && o.Status >= 3);

            return View(entity);
        }

        public ActionResult AuditCloseIndex()
        {
            return View();
        }

        public ActionResult CloseObs(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_audit_schedule.Include("tbl_audit_plan").Include("tbl_audit_participant").Include("tbl_audit_participant.tbl_org_employee").Include("tbl_audit_observation").Where(o => o.ID == id).SingleOrDefault();

            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Audit Finding does not exist";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            var user = ctx.UserProfile.Include("tbl_org_employee").Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();

            if (entity.tbl_audit_participant != null && entity.tbl_audit_participant.Where(o => o.Type == 1 && o.tbl_Org_EmployeeID == user.EmployeeID.GetValueOrDefault()).Any())
            {
                reviewCommentsInput input = new reviewCommentsInput();
                input.ID = entity.ID;
                input.ClosedBy = WebSecurity.CurrentUserId;
                input.AuditorName = user.tbl_org_employee.GivenName + " " + user.tbl_org_employee.FamilyName;
                input.CloseDate = System.DateTime.Now.Date;
                return View(input);
            }
            else
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "You are not authorised to close this audit. You must be the auditor of this audit to close the observations.";
                return View("ListItems/showError");	// Return error in a page
            }
        }

        [HttpPost]
        public ActionResult CloseObs(reviewCommentsInput input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View(input);
            }
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = service.Get(input.ID);
            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Audit Findings do not exist.";
                return View("ListItems/showError");	// Return error in a page
            }
            entity.ClientID = ((PAIdentity)User.Identity).clientID;
            entity.CloseDate = System.DateTime.Now.Date;
            entity.ClosedBy = WebSecurity.CurrentUserId;
            entity.ClosureComments = input.ReviewComments;
            entity.Status = 4;
            if (entity.tbl_audit_observation != null && entity.tbl_audit_observation.Any())
            {
                foreach (var obs in entity.tbl_audit_observation)
                {
                    obs.ClientID = entity.ClientID;
                    obs.Status = 3;
                    obs.ActualCloseDate = System.DateTime.Now.Date;
                }
            }
            ctx.SaveChanges();
            ViewBag.Mode = 1;
            return View("ShowReport",editMapper.MapToInput(entity));
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.WebUI.BAL.AccessControl;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core;
using System.Transactions;
using WebMatrix.WebData;
using System.Globalization;
using System;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class OrgAuditFindingController : Cruder<tbl_org_audit_schedule, orgAuditRecordingInput>
    {

        public OrgAuditFindingController(OrgAuditFindingService service, org_audit_recordingMapper v, IWorkflowService wf)
            : base(service, v, wf, "ADTORGCD")
        {
            functionID = "ADTORGCD";
        }

        protected override string RowViewName
        {
            get { return "LandingPage"; }
        }

        public ActionResult GetAudits()
        {
            // Input parameter id is the project id
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_org_audit_schedule.Include("tbl_org_audit_plan").Include("tbl_org_audit_participant").Include("tbl_org_audit_observation").Include("tbl_org_audit_addln_obs").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);

            return View("GetItems", entity);
        }

        public ActionResult AddAddln(int id, int key, byte type)
        {
            ViewBag.Key = key;
            ViewBag.ScheduleID = id;
            ViewBag.ClientID = ((PAIdentity)User.Identity).clientID;
            ViewBag.Type = type;
            return View();
        }

        public ActionResult AddNC(int id, int key)
        {
            ViewBag.Key = key;
            ViewBag.ScheduleID = id;
            ViewBag.ClientID = ((PAIdentity)User.Identity).clientID;
            return View();
        }

        public ActionResult DeleteAddlnObs(int id, int scheduleID, byte type)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var obs = ctx.tbl_org_audit_addln_obs.Where(o => o.ID == id && o.tbl_Org_Audit_ScheduleID == scheduleID && o.Type == type).SingleOrDefault();
                if (obs != null)
                {
                    ctx.tbl_org_audit_addln_obs.Remove(obs);
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

        public ActionResult DeleteObservation(int id, int scheduleID)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var obs = ctx.tbl_org_audit_observation.Where(o => o.ID == id && o.tbl_Org_Audit_ScheduleID == scheduleID).SingleOrDefault();
                if (obs != null)
                {
                    ctx.tbl_org_audit_observation.Remove(obs);
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

        protected override void ReSequenceBeforeEdit(orgAuditRecordingInput input)
        {
            if (input.Status == 2) input.Status = 3;
        }

        public override ActionResult ProcessView(IEnumerable<tbl_org_audit_schedule> entity)
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
                return View("ShowReport", editMapper.MapToInput(entity));
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

        public ActionResult BackToIndex(string viewName)
        {
            return View(viewName);
        }

        public ActionResult GetAuditsForClosure()
        {
            // here id is the project id
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_org_audit_schedule.Include("tbl_org_audit_plan").Include("tbl_org_audit_participant").Include("tbl_org_audit_observation").Include("tbl_org_audit_addln_obs").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.Status >= 3);
            return View(entity);
        }

        public ActionResult AuditCloseIndex()
        {
            return View();
        }

        public ActionResult CloseObs(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_org_audit_schedule.Include("tbl_org_audit_plan").Include("tbl_org_audit_participant").Include("tbl_org_audit_participant.tbl_org_employee").Include("tbl_org_audit_observation").Where(o => o.ID == id).SingleOrDefault();

            if (entity == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Audit Finding does not exist";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            var user = ctx.UserProfile.Include("tbl_org_employee").Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();

            if (entity.tbl_org_audit_participant != null && entity.tbl_org_audit_participant.Where(o => o.Type == 1 && o.tbl_Org_EmployeeID == user.EmployeeID.GetValueOrDefault()).Any())
            {
                reviewOrgCommentsInput input = new reviewOrgCommentsInput();
                input.ID = entity.ID;
                input.ClosedBy = WebSecurity.CurrentUserId;
                input.AuditorName = user.tbl_org_employee.GivenName + " " + user.tbl_org_employee.FamilyName;
                input.CloseDate = System.DateTime.Now.Date;
                input.ClosureComments = entity.ClosureComments;
                input.Status = entity.Status;
                input.NextAuditOn = entity.NextAuditOn;
                input.EvaluationCAPA = entity.EvaluationCAPA;
                return View(input);
            }
            else
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "You are not authorised to close this audit. You must be the auditor of this audit to close the observations.";
                return View("ListItems/showError");	// Return error in a pag
            }
        }

        [HttpPost]
        public ActionResult CloseObs(reviewOrgCommentsInput input)
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
            entity.CloseDate = System.DateTime.Now.Date;
            entity.ClosedBy = WebSecurity.CurrentUserId;
            entity.ClosureComments = input.ClosureComments;
            entity.NextAuditOn = input.NextAuditOn;
            entity.Status = 4;
            entity.EvaluationCAPA = input.EvaluationCAPA;
            if (entity.tbl_org_audit_observation != null && entity.tbl_org_audit_observation.Any())
            {
                foreach (var obs in entity.tbl_org_audit_observation)
                {
                    obs.Status = input.Status;
                    obs.ActualCloseDate = System.DateTime.Now.Date;
                }
            }
            ctx.SaveChanges();
            ViewBag.Mode = 1;
            return View("ShowReport", editMapper.MapToInput(entity));
        }
    }
}

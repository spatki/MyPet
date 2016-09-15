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
    public class OrgAuditController : Cruder<tbl_org_audit_plan, tbl_org_audit_planInput>
    {
        public OrgAuditController(OrgAuditService service, tbl_org_audit_planMapper v, IWorkflowService wf)
            : base(service, v, wf, "ADTORGPL")
        {
            functionID = "ADTORGPL";
        }

        protected override string RowViewName
        {
            get { return "ShowAddedPlan"; }
        }

        public ActionResult GetAuditPlan()
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var entity = ctx.tbl_org_audit_plan.Include("tbl_org_audit_schedule").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(o => o.Start);
            return View(entity);
        }

        public ActionResult NewAuditPlan()
        {
            var ctx = (Db)service.getRepo().getDBContext();
            tbl_org_audit_planInput input = new tbl_org_audit_planInput()
            {
                ClientID = ((PAIdentity)User.Identity).clientID,
                Duration = 1,
                AuditorRoles = new List<int>(),
            };
            return View(input);
        }

        [HttpPost]
        public ActionResult NewAuditPlan(tbl_org_audit_planInput input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View(input);
            }
            var checkFailed = false;
            if (input.Start > input.Finish)
            {
                ModelState.AddModelError("Start", "This date cannot be later than Finish date");
                checkFailed = true;
            }
            if (input.AuditType == 2)
            {
                // Check whether dependent information is present
                if (input.Frequency == null)
                {
                    ModelState.AddModelError("Frequency", "Ener how often will this audit recurr");
                    checkFailed = true;
                }
                if (input.Period == null || input.Period == 0)
                {
                    ModelState.AddModelError("Period", "Enter Recurring Period");
                    checkFailed = true;
                }
                if (input.Duration == 0)
                {
                    ModelState.AddModelError("Duration", "Enter Duration");
                    checkFailed = true;
                }
            }
            else
            {
                input.Duration = (byte)(input.Finish - input.Start).TotalDays;
                input.Frequency = null;
                input.Period = null;
            }
            if (checkFailed)
            {
                Response.StatusCode = 412;
                return View(input);
            }
            tbl_org_audit_plan entity;
            var clientID = ((PAIdentity)User.Identity).clientID;
            using (TransactionScope scope = new TransactionScope())
            {
                input.RefID = "O" + input.ClientID;
                var newID = service.Create(createMapper.MapToEntity(input, new tbl_org_audit_plan()));
                entity = service.Get(newID);
                entity.RefID = entity.RefID + "-A" + newID.ToString();
                // Create Schedule Entries
                entity.tbl_org_audit_schedule = new List<tbl_org_audit_schedule>();
                if (input.AuditType == 2)   // Recurring. Hence create multiple entries
                {
                    var startDate = input.Start;
                    AuditFrequency AuditFreqType = new AuditFrequency();
                    var frequencyDays = input.Period * AuditFreqType.Days(input.Frequency.GetValueOrDefault());
                    while (startDate < input.Finish)
                    {
                        entity.tbl_org_audit_schedule.Add(new tbl_org_audit_schedule()
                        {
                            ClientID = clientID,
                            Duration = input.Duration,
                            Planned_Start = startDate,
                            Planned_Finish = startDate.AddDays(input.Duration),
                            Start = null,
                            Finish = null,
                            Status = 1,
                            tbl_Org_Audit_PlanID = newID,
                        });
                        startDate = startDate.AddDays(frequencyDays.GetValueOrDefault());
                    }
                }
                else
                {
                    entity.tbl_org_audit_schedule.Add(new tbl_org_audit_schedule()
                    {
                        ClientID = clientID,

                        Duration = (byte)((input.Finish - input.Start).TotalDays + 1),
                        Planned_Start = input.Start,
                        Planned_Finish = input.Finish,
                        Start = null,
                        Finish = null,
                        Status = 1,
                        tbl_Org_Audit_PlanID = newID,
                    });
                }
                service.Save();
                scope.Complete();
            }
            return View("ShowAddedPlan", new[] { entity });
        }

        [HttpPost]
        public override ActionResult Edit(tbl_org_audit_planInput input)
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
                var checkFailed = false;
                if (input.Start > input.Finish)
                {
                    ModelState.AddModelError("Start", "This date cannot be later than Finish date");
                    checkFailed = true;
                }
                if (input.AuditType == 2)
                {
                    // Check whether dependent information is present
                    if (input.Frequency == null)
                    {
                        ModelState.AddModelError("Frequency", "Ener how often will this audit recurr");
                        checkFailed = true;
                    }
                    if (input.Period == null || input.Period == 0)
                    {
                        ModelState.AddModelError("Period", "Enter Recurring Period");
                        checkFailed = true;
                    }
                    if (input.Duration == 0)
                    {
                        ModelState.AddModelError("Duration", "Enter Duration");
                        checkFailed = true;
                    }
                }
                else
                {
                    input.Duration = (byte)(input.Finish - input.Start).TotalDays;
                    input.Frequency = null;
                    input.Period = null;
                }
                if (checkFailed)
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
                using (TransactionScope scope = new TransactionScope())
                {
                    var entity = service.Get(input.ID);
                    var ctx = (Db)service.getRepo().getDBContext();
                    if (entity.tbl_org_audit_schedule.Where(o => o.Status != 1).Any())
                    {
                        // Some parameters of this Audit cannot be changed
                        var checkResult = false;
                        if (entity.Start != input.Start)
                        {
                            ModelState.AddModelError("Start", "Cannot be changed");
                            checkResult = true;
                        }
                        if (entity.Finish != input.Finish)
                        {
                            ModelState.AddModelError("Finish", "Cannot be changed");
                            checkResult = true;
                        }
                        if (entity.AuditType != input.AuditType)
                        {
                            ModelState.AddModelError("AuditType", "Cannot be changed");
                            checkResult = true;
                        }
                        if (entity.Period != input.Period)
                        {
                            ModelState.AddModelError("Period", "Cannot be changed");
                            checkResult = true;
                        }
                        if (entity.Frequency != input.Frequency)
                        {
                            ModelState.AddModelError("Frequency", "Cannot be changed");
                            checkResult = true;
                        }
                        if (entity.Duration != input.Duration)
                        {
                            ModelState.AddModelError("Duration", "Cannot be changed");
                            checkResult = true;
                        }
                        if (checkResult)
                        {
                            Response.StatusCode = 412;
                            ModelState.AddModelError("", "Some audits are either scheduled or completed as per this plan, hence some details of this plan cannot be modified");
                            return View(input);
                        }
                        // Critical details impacting individual audit schedules have not been changed, so only one table can be updated.
                        var e = editMapper.MapToEntity(input, entity);
                        e.ClientID = ((PAIdentity)User.Identity).clientID;
                        // Remove roles that may have been removed as a part of edit
                        var roles = e.tbl_org_audit_role.ToArray<tbl_org_audit_role>();
                        foreach (var rl in roles)
                        {
                            rl.ClientID = entity.ClientID;
                            if (!input.AuditorRoles.Contains(rl.tbl_Org_RoleID) && rl.Type == 1)
                            {
                                // Delete this entry
                                e.tbl_org_audit_role.Remove(rl);
                                ctx.Entry(rl).State = System.Data.Entity.EntityState.Deleted;
                            }
                        }
                        service.Save();
                    }
                    else
                    {
                        if (entity.Start != input.Start || entity.Finish != input.Finish || entity.Period != input.Period || entity.Duration != input.Duration || entity.AuditType != input.AuditType)
                        {
                            var schedule = entity.tbl_org_audit_schedule.ToArray<tbl_org_audit_schedule>();
                            foreach (var sch in schedule)
                            {
                                entity.tbl_org_audit_schedule.Remove(sch);
                                ctx.Entry(sch).State = System.Data.Entity.EntityState.Deleted;
                            }
                            entity = editMapper.MapToEntity(input, entity);
                            entity.ClientID = ((PAIdentity)User.Identity).clientID;
                            // Remove roles that may have been removed as a part of edit
                            var roles = entity.tbl_org_audit_role.ToArray<tbl_org_audit_role>();
                            foreach (var rl in roles)
                            {
                                rl.ClientID = entity.ClientID;
                                if (!input.AuditorRoles.Contains(rl.tbl_Org_RoleID) && rl.Type == 1)
                                {
                                    // Delete this entry
                                    entity.tbl_org_audit_role.Remove(rl);
                                    ctx.Entry(rl).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                            service.Save();
                            // Create Schedule Entries
                            if (input.AuditType == 2)   // Recurring. Hence create multiple entries
                            {
                                var startDate = input.Start;
                                AuditFrequency AuditFreqType = new AuditFrequency();
                                var frequencyDays = input.Period * AuditFreqType.Days(input.Frequency.GetValueOrDefault());
                                while (startDate < input.Finish)
                                {
                                    entity.tbl_org_audit_schedule.Add(new tbl_org_audit_schedule()
                                    {
                                        ClientID = entity.ClientID,
                                        Duration = input.Duration,
                                        Planned_Start = startDate,
                                        Planned_Finish = startDate.AddDays(input.Duration),
                                        Start = null,
                                        Finish = null,
                                        Status = 1,
                                        tbl_Org_Audit_PlanID = entity.ID,
                                    });
                                    startDate = startDate.AddDays(frequencyDays.GetValueOrDefault());
                                }
                            }
                            else
                            {
                                entity.tbl_org_audit_schedule.Add(new tbl_org_audit_schedule()
                                {
                                    ClientID = entity.ClientID,
                                    Duration = (byte)(input.Finish - input.Start).TotalDays,
                                    Planned_Start = input.Start,
                                    Planned_Finish = input.Finish,
                                    Start = null,
                                    Finish = null,
                                    Status = 1,
                                    tbl_Org_Audit_PlanID = entity.ID,
                                });
                            }
                            service.Save();
                        }
                        else
                        {
                            entity = editMapper.MapToEntity(input, entity);
                            entity.ClientID = ((PAIdentity)User.Identity).clientID;
                            entity.tbl_org_audit_schedule = null;
                            foreach (var rl in entity.tbl_org_audit_role)
                            {
                                rl.ClientID = entity.ClientID;
                                if (rl.Type == 1) entity.tbl_org_audit_role.Remove(rl);
                            }
                            if (input.AuditorRoles != null && input.AuditorRoles.Any())
                            {
                                entity.tbl_org_audit_role = new List<tbl_org_audit_role>();
                                foreach (var rl in input.AuditorRoles)
                                {
                                    entity.tbl_org_audit_role.Add(new tbl_org_audit_role()
                                    {
                                        ClientID = entity.ClientID,
                                        tbl_Org_Audit_PlanID = entity.ID,
                                        tbl_Org_RoleID = rl,
                                        Type = 1,
                                    });
                                }
                            }
                            service.Save();
                        }
                    }
                    scope.Complete();
                    //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                    return View(RowViewName, new[] { entity });
                }
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult ScheduleAudit(int id)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            try
            {
                var entity = ctx.tbl_org_audit_schedule.Include("tbl_org_audit_plan").Include("tbl_org_audit_plan.tbl_org_audit_role").Include("tbl_org_audit_participant").Where(o => o.ID == id).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Audit plan does not exist.";
                    return View("ListItems/showError");
                }
                tbl_org_audit_scheduleMapper mapper = new tbl_org_audit_scheduleMapper();
                return View(mapper.MapToInput(entity));
            }
            catch (Exception e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Application Error: " + e.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public ActionResult ScheduleAudit(tbl_org_audit_scheduleInput input)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            try
            {
                var notValid = false;
                if (!ModelState.IsValid)
                {
                    notValid = true;
                }

                var entity = ctx.tbl_org_audit_schedule.Include("tbl_org_audit_plan").Include("tbl_org_audit_plan.tbl_org_audit_role").Include("tbl_org_audit_participant").Where(o => o.ID == input.ID).SingleOrDefault();
                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Audit plan does not exist.";
                    return View("ListItems/showError");
                }
                if (notValid)
                {
                    Response.StatusCode = 412;
                    return View(input);
                }
                tbl_org_audit_scheduleMapper mapper = new tbl_org_audit_scheduleMapper();
                input.Status = 2;
                entity = mapper.MapToEntity(input, entity);
                if (entity.tbl_org_audit_participant.Any())
                {
                    var participants = entity.tbl_org_audit_participant.ToArray<tbl_org_audit_participant>();
                    foreach (var pt in participants)
                    {
                        if (!input.Auditors.Contains(pt.tbl_Org_EmployeeID) && pt.Type == 1)
                        {
                            entity.tbl_org_audit_participant.Remove(pt);
                            ctx.Entry(pt).State = System.Data.Entity.EntityState.Deleted;
                        }
                        if (!input.Auditees.Contains(pt.tbl_Org_EmployeeID) && pt.Type == 2)
                        {
                            entity.tbl_org_audit_participant.Remove(pt);
                            ctx.Entry(pt).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }
                ctx.SaveChanges();
                ViewBag.RefID = entity.tbl_org_audit_plan.RefID;
                return View("GetSchedule", input);
            }
            catch (Exception e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Application Error: " + e.Message;
                return View("ListItems/showError");
            }
        }
    }
}
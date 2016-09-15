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
using System.Linq.Expressions;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class TimesheetController : Cruder<tbl_org_timesheet, tbl_org_timesheetInput>
    {
        public TimesheetController(ICrudService<tbl_org_timesheet> service, IMapper<tbl_org_timesheet, tbl_org_timesheetInput> v, IWorkflowService wf)
            : base(service, v, wf, "RECTSENT")
        {
            functionID = "RECTSENT";
        }

        protected override string RowViewName
        {
            get { return "Edit"; }
        }

        public override ActionResult Index()
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var UserDetails = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            if (UserDetails.EmployeeID == null)    // Check if user is an employee.
            {
                ViewBag.ErrorMessage = "This user is not an employee. Hence timesheets are not available for this user.";
                return View("ListItems/showErrorPage");
            }
            else
            {
                System.DateTime fromDate = System.DateTime.Now.Date;
                int weekDay = (int)fromDate.DayOfWeek;
                if (weekDay > 1)
                {
                    fromDate = fromDate.AddDays((-1) * (weekDay - 1));
                }
                else
                {
                    if (weekDay == 0) fromDate = fromDate.AddDays(-6);
                }
                ViewBag.startDate = fromDate;
                return View();
            }
        }

        public ActionResult WeeklyView(System.DateTime startDate)
        {
            System.DateTime fromDate = startDate;
            System.DateTime toDate = fromDate.AddDays(6);
            int empID;

            var ctx = (Db)service.getRepo().getDBContext();
            var UserDetails = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            if (UserDetails.EmployeeID == null)    // Check if user is an employee.
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "This user is not an employee. Hence timesheets are not available for this user.";
                return View("ListItems/showErrorPage");
            }
            else
            {
                empID = UserDetails.EmployeeID.GetValueOrDefault();
            }
            var status = ctx.TS_Status(empID, fromDate, toDate, 0);    // Get status based on individual statues
            timesheetEntry input = new timesheetEntry()
            {
                TS_StartDate = fromDate,
                TS_EndDate = toDate,
                tbl_Org_EmployeeID = empID,
                StatusID = (status == null ? (int?) null : status.ID),
                StatusName = (status == null ? null : status.Status),
                ViewType = 2    // WeeklyView
            };
            input.tbl_org_timesheet = new List<tbl_org_timesheetInput>();
            // Populate relevant records
            var vw_assigned_tasks = ctx.vw_assigned_tasks.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_EmployeeID == empID
                                                            && ((o.res_PlannedStart <= toDate && o.res_PlannedEnd >= fromDate) ||
                                                                (fromDate >= o.res_PlannedStart && o.res_ActualEnd == null)));
            var assigned_timeentry = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                            o.tbl_Org_EmployeeID == empID &&
                                                            o.TSDate >= fromDate && o.TSDate <= toDate && o.tbl_Org_Proj_PlanID != null);
            System.DateTime processDate;
            System.DateTime? startTime;
            System.DateTime? endTime;
            decimal processHours = 0;
            int TS_ID = 0;
            int newCounter = 0;
            int? TS_Status = null;
            var keepProcessing = false;
            if (vw_assigned_tasks != null && vw_assigned_tasks.Any()) keepProcessing = true;

            for (var loops = 1; loops <= 2; loops++)
            {
                if (keepProcessing)
                {
                    foreach (var t in vw_assigned_tasks)
                    {
                        processDate = startDate;
                        for (var i = 1; i <= 7; i++)     // Loop for 7 days
                        {
                            var tsEntry = assigned_timeentry.Where(o => o.TSDate == processDate && o.tbl_Org_Proj_PlanID == t.PlanID && o.tbl_Org_Plan_ResourceID == t.Plan_ResourceID).SingleOrDefault();
                            if (tsEntry == null) // No time entered for this day
                            {
                                newCounter = newCounter + 1;
                                TS_ID = newCounter;
                                processHours = 0;
                                TS_Status = (status == null ? (int?)null : status.ID);
                                startTime = null;
                                endTime = null;
                            }
                            else
                            {
                                TS_ID = tsEntry.ID;
                                processHours = tsEntry.Duration;
                                TS_Status = tsEntry.mstr_Process_LC_StatusID;
                                startTime = tsEntry.StartTime;
                                endTime = tsEntry.EndTime;
                            }
                            input.tbl_org_timesheet.Add(new tbl_org_timesheetInput()
                            {
                                ID = TS_ID,
                                tbl_Org_EmployeeID = empID,
                                tbl_Org_ProjectID = t.tbl_Org_ProjectID,
                                GroupName = t.ProjectName,
                                SubGroupName = t.GroupName,
                                TaskName = t.Plan_TaskName,
                                PlannedStart = t.res_PlannedStart,
                                PlannedEnd = t.res_PlannedEnd,
                                ActualStart = t.res_ActualStart,
                                ActualEnd = t.res_ActualEnd,
                                TaskStatus = t.res_StatusID,
                                mstr_Org_FunctionID = null,
                                tbl_Org_Proj_PlanID = t.PlanID,
                                tbl_Org_Plan_ResourceID = t.Plan_ResourceID,
                                tbl_Org_Proj_GroupID = t.GroupID,
                                mstr_Org_Sub_FunctionID = null,
                                Type = 1,
                                tbl_Mapped_Proj_ProcessID = t.tbl_Mapped_Proj_ProcessID,
                                tbl_Process_RepositoryID = t.RepositoryID,
                                tbl_Process_Rep_TaskID = t.tbl_Process_Rep_TaskID,
                                tbl_General_TaskID = null,
                                Duration = processHours,
                                PlannedDuration = t.res_PlannedDuration,
                                ActualDuration = t.res_ActualDuration,
                                IsDefault = t.IsDefault,
                                IsComplete = t.IsComplete,
                                IsPublish = t.IsPublish,
                                IsReview = t.IsReview,
                                res_StatusID = t.res_StatusID,
                                res_TaskStatus = t.res_TaskStatus,
                                TSDate = processDate,
                                StartTime = startTime,
                                EndTime = endTime,
                                BillableDuration = (tsEntry == null ? 0 : tsEntry.BillableDuration),
                                OvertimeDuration = (tsEntry == null ? 0 : tsEntry.OvertimeDuration),
                                Billable = (tsEntry == null ? null : tsEntry.Billable),
                                mstr_Process_LC_StatusID = TS_Status,
                                Comments = (tsEntry == null ? null : tsEntry.Comments)
                            });
                            processDate = processDate.AddDays(1);
                        }
                    }
                }
                if (loops == 1)
                {
                    var resourceIDs = vw_assigned_tasks.Select(o => o.Plan_ResourceID);
                    assigned_timeentry = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                                o.tbl_Org_EmployeeID == empID &&
                                                                o.TSDate >= fromDate && o.TSDate <= toDate && o.tbl_Org_Proj_PlanID != null &&
                                                                !(resourceIDs.Contains((int)o.tbl_Org_Plan_ResourceID)));
                    var closedResourceIDs = assigned_timeentry.Select(l => l.tbl_Org_Plan_ResourceID);
                    vw_assigned_tasks = ctx.vw_assigned_tasks.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_EmployeeID == empID
                                                                && closedResourceIDs.Contains(o.Plan_ResourceID));
                    if (vw_assigned_tasks != null && vw_assigned_tasks.Any()) keepProcessing = true;
                    else keepProcessing = false;
                }
                else keepProcessing = false;
            }                                   
            // Timesheet for general tasks
            var tailored_general_tasks = ctx.TS_GeneralTasks(empID, fromDate, toDate);
            var general_timeentry = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                            o.tbl_Org_EmployeeID == empID &&
                                                            o.TSDate >= fromDate && o.TSDate <= toDate && o.tbl_Org_Proj_PlanID == null);
            // General Tasks
            if (tailored_general_tasks != null && tailored_general_tasks.Any())
            {
                mstr_process_lc_status statusDetails;
                foreach (var t in tailored_general_tasks)
                {
                    processDate = startDate;
                    for (var i = 1; i <= 7; i++)     // Loop for 7 days
                    {
                        var tsEntry = general_timeentry.Where(o => o.TSDate == processDate && o.tbl_General_TaskID == t.ID).SingleOrDefault();
                        if (tsEntry == null) // No time entered for this day
                        {
                            newCounter = newCounter + 1;
                            TS_ID = newCounter;
                            processHours = 0;
                            TS_Status = (status == null ? (int?) null : status.ID);
                            startTime = null;
                            endTime = null;
                            statusDetails = null;
                        }
                        else
                        {
                            TS_ID = tsEntry.ID;
                            processHours = tsEntry.Duration;
                            TS_Status = tsEntry.mstr_Process_LC_StatusID;
                            startTime = tsEntry.StartTime;
                            endTime = tsEntry.EndTime;
                            statusDetails = ctx.mstr_process_lc_status.Where(o => o.ID == tsEntry.mstr_Process_LC_StatusID).SingleOrDefault();
                        }
                        input.tbl_org_timesheet.Add(new tbl_org_timesheetInput()
                        {
                            ID = TS_ID,
                            tbl_Org_EmployeeID = empID,
                            tbl_Org_ProjectID = null,
                            GroupName = "",
                            SubGroupName = "",
                            TaskName = t.Name,
                            PlannedStart = processDate.AddDays(1),
                            PlannedEnd = processDate.AddDays(1),
                            ActualStart = null,
                            ActualEnd = null,
                            TaskStatus = TS_Status,
                            mstr_Org_FunctionID = null,
                            tbl_Org_Proj_PlanID = null,
                            tbl_Org_Plan_ResourceID = null,
                            tbl_Org_Proj_GroupID = null,
                            mstr_Org_Sub_FunctionID = null,
                            Type = 3,
                            tbl_Mapped_Proj_ProcessID = null,
                            tbl_Process_RepositoryID = null,
                            tbl_Process_Rep_TaskID = null,
                            tbl_General_TaskID = t.ID,
                            Duration = processHours,
                            PlannedDuration = 0,
                            ActualDuration = null,
                            IsDefault = (statusDetails == null ? (bool?) null : statusDetails.IsDefault),
                            IsComplete = (statusDetails == null ? (bool?)null : statusDetails.IsComplete),
                            IsPublish = (statusDetails == null ? (bool?)null : statusDetails.IsPublish),
                            IsReview = (statusDetails == null ? (bool?)null : statusDetails.IsReview),
                            res_StatusID = (statusDetails == null ? (int?)null : statusDetails.ID),
                            res_TaskStatus = (statusDetails == null ? null : statusDetails.Status),
                            TSDate = processDate,
                            StartTime = startTime,
                            EndTime = endTime,
                            BillableDuration = null,
                            OvertimeDuration = null,
                            Billable = null,
                            mstr_Process_LC_StatusID = TS_Status,
                            Comments = (tsEntry == null ? null : tsEntry.Comments)
                        });
                        processDate = processDate.AddDays(1);
                    }
                }
            }                    

            // Check for any workflow 
            var user = ((PAIdentity)User.Identity);
            var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, null, user.IsAdmin(), (status == null ? (int?)null : status.ID), null, user.clientID.GetValueOrDefault());
            if (wf.Any())
            {
                ViewBag.WF = true;
                ViewBag.workflow = wf;
            }

            return PartialView("WeeklyView",input);
        }

        public ActionResult DailyView(System.DateTime startDate)
        {
            System.DateTime fromDate = startDate;
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
            var status = ctx.TS_Status(empID, fromDate, fromDate, 0);    // Get status based on individual statues
            timesheetEntry input = new timesheetEntry()
            {
                TS_StartDate = fromDate,
                TS_EndDate = null,
                tbl_Org_EmployeeID = empID,
                StatusID = (status == null ? (int?) null : status.ID),
                StatusName = (status == null ? null : status.Status),
                ViewType = 1    // DailyView
            };
            input.tbl_org_timesheet = new List<tbl_org_timesheetInput>();
            var vw_assigned_tasks = ctx.vw_assigned_tasks.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_EmployeeID == empID
                                                && ((fromDate >= o.res_PlannedStart && fromDate <= o.res_PlannedEnd) ||
                                                    (fromDate >= o.res_PlannedStart && o.res_ActualEnd == null)));
            var assigned_timeentry = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                            o.tbl_Org_EmployeeID == empID &&
                                                            o.TSDate == fromDate && o.tbl_Org_Proj_PlanID != null);
            int TS_ID = 0;
            int newCounter = 0;
            int? TS_Status = null;
            System.DateTime? startTime;
            System.DateTime? endTime;
            decimal processHours = 0;

            for (var loops = 1; loops <= 2; loops++)
            {
                if (vw_assigned_tasks != null && vw_assigned_tasks.Any())
                {
                    foreach (var t in vw_assigned_tasks)
                    {
                        var tsEntry = assigned_timeentry.Where(o => o.TSDate == fromDate && o.tbl_Org_Proj_PlanID == t.PlanID && o.tbl_Org_Plan_ResourceID == t.Plan_ResourceID).SingleOrDefault();
                        if (tsEntry == null) // No time entered for this day
                        {
                            newCounter = newCounter + 1;
                            TS_ID = newCounter;
                            processHours = 0;
                            TS_Status = (status == null ? (int?) null : status.ID);
                            startTime = null;
                            endTime = null;
                        }
                        else
                        {
                            TS_ID = tsEntry.ID;
                            processHours = tsEntry.Duration;
                            TS_Status = tsEntry.mstr_Process_LC_StatusID;
                            startTime = tsEntry.StartTime;
                            endTime = tsEntry.EndTime;
                        }
                        input.tbl_org_timesheet.Add(new tbl_org_timesheetInput()
                        {
                            ID = TS_ID,
                            tbl_Org_EmployeeID = empID,
                            tbl_Org_ProjectID = t.tbl_Org_ProjectID,
                            GroupName = t.ProjectName,
                            SubGroupName = t.GroupName,
                            TaskName = t.Plan_TaskName,
                            PlannedStart = t.res_PlannedStart,
                            PlannedEnd = t.res_PlannedEnd,
                            ActualStart = t.res_ActualStart,
                            ActualEnd = t.res_ActualEnd,
                            TaskStatus = t.res_StatusID,
                            mstr_Org_FunctionID = null,
                            tbl_Org_Proj_PlanID = t.PlanID,
                            tbl_Org_Plan_ResourceID = t.Plan_ResourceID,
                            tbl_Org_Proj_GroupID = t.GroupID,
                            mstr_Org_Sub_FunctionID = null,
                            Type = 1,
                            tbl_Mapped_Proj_ProcessID = t.tbl_Mapped_Proj_ProcessID,
                            tbl_Process_RepositoryID = t.RepositoryID,
                            tbl_Process_Rep_TaskID = t.tbl_Process_Rep_TaskID,
                            tbl_General_TaskID = null,
                            Duration = processHours,
                            PlannedDuration = t.res_PlannedDuration,
                            ActualDuration = t.res_ActualDuration,
                            TSDate = fromDate,
                            StartTime = startTime,
                            EndTime = endTime,
                            BillableDuration = (tsEntry == null ? 0 : tsEntry.BillableDuration),
                            OvertimeDuration = (tsEntry == null ? 0 : tsEntry.OvertimeDuration),
                            Billable = (tsEntry == null ? null : tsEntry.Billable),
                            mstr_Process_LC_StatusID = TS_Status,
                            Comments = (tsEntry == null ? null : tsEntry.Comments)
                        });
                    }
                }
                var resourceIDs = vw_assigned_tasks.Select(o => o.Plan_ResourceID);
                assigned_timeentry = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                            o.tbl_Org_EmployeeID == empID &&
                                                            o.TSDate == fromDate && o.tbl_Org_Proj_PlanID != null &&
                                                            !(resourceIDs.Contains((int)o.tbl_Org_Plan_ResourceID)));
                var closedResourceIDs = assigned_timeentry.Select(l => l.tbl_Org_Plan_ResourceID);
                vw_assigned_tasks = ctx.vw_assigned_tasks.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_EmployeeID == empID
                                                            && closedResourceIDs.Contains(o.Plan_ResourceID));

            }
            // Timesheet for general tasks
            var tailored_general_tasks = ctx.TS_GeneralTasks(empID, fromDate, fromDate);
            var general_timeentry = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                            o.tbl_Org_EmployeeID == empID &&
                                                            o.TSDate == fromDate && o.tbl_Org_Proj_PlanID == null);
            // General Tasks
            if (tailored_general_tasks != null && tailored_general_tasks.Any())
            {
                foreach (var t in tailored_general_tasks)
                {
                    var tsEntry = general_timeentry.Where(o => o.TSDate == fromDate && o.tbl_General_TaskID == t.ID).SingleOrDefault();
                    if (tsEntry == null) // No time entered for this day
                    {
                        newCounter = newCounter + 1;
                        TS_ID = newCounter;
                        processHours = 0;
                        TS_Status = (status == null ? (int?) null : status.ID);
                        startTime = null;
                        endTime = null;
                    }
                    else
                    {
                        TS_ID = tsEntry.ID;
                        processHours = tsEntry.Duration;
                        TS_Status = tsEntry.mstr_Process_LC_StatusID;
                        startTime = tsEntry.StartTime;
                        endTime = tsEntry.EndTime;
                    }
                    input.tbl_org_timesheet.Add(new tbl_org_timesheetInput()
                    {
                        ID = TS_ID,
                        tbl_Org_EmployeeID = empID,
                        tbl_Org_ProjectID = null,
                        GroupName = "",
                        SubGroupName = "",
                        TaskName = t.Name,
                        PlannedStart = fromDate.AddDays(1),
                        PlannedEnd = fromDate.AddDays(1),
                        ActualStart = null,
                        ActualEnd = null,
                        TaskStatus = TS_Status,
                        mstr_Org_FunctionID = null,
                        tbl_Org_Proj_PlanID = null,
                        tbl_Org_Plan_ResourceID = null,
                        tbl_Org_Proj_GroupID = null,
                        mstr_Org_Sub_FunctionID = null,
                        Type = 3,
                        tbl_Mapped_Proj_ProcessID = null,
                        tbl_Process_RepositoryID = null,
                        tbl_Process_Rep_TaskID = null,
                        tbl_General_TaskID = t.ID,
                        Duration = processHours,
                        PlannedDuration = 0,
                        ActualDuration = null,
                        TSDate = fromDate,
                        StartTime = startTime,
                        EndTime = endTime,
                        BillableDuration = null,
                        OvertimeDuration = null,
                        Billable = null,
                        mstr_Process_LC_StatusID = TS_Status,
                        Comments = (tsEntry == null ? null : tsEntry.Comments)
                    });
                }
            }
            // Check for any workflow 
            var user = ((PAIdentity)User.Identity);
            var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, null, user.IsAdmin(), (status == null ? (int?)null : status.ID), null, user.clientID.GetValueOrDefault());
            if (wf.Any())
            {
                ViewBag.WF = true;
                ViewBag.workflow = wf;
            }

            return PartialView("DailyView", input);
        }

        [HttpPost]
        public ActionResult DailyView(timesheetEntry input)
        {
            return null;
        }

        [HttpPost]
        public ActionResult MonthlyView(timesheetEntry input)
        {
            return null;
        }

        public ActionResult MonthlyView(System.DateTime startDate)
        {
            return null;
        }

        [HttpPost]
        public ActionResult ProcessTimesheet(timesheetEntry input)
        {
            if (!CheckAccess("Edit"))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                switch (input.ViewType)
                {
                    case 1:
                        return View("DailyView", input);
                    case 2:
                        return View("WeeklyView", input);
                    case 3:
                        return View("MonthlyView", input);
                    default:
                        return View("Index");
                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                var entity = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_EmployeeID == input.tbl_Org_EmployeeID
                                           && o.TSDate >= input.TS_StartDate && o.TSDate <= (input.TS_EndDate == null ? o.TSDate : input.TS_EndDate));
                foreach (var t in input.tbl_org_timesheet)
                {
                    t.ClientID = ((PAIdentity)User.Identity).clientID;
                    if (input.followWF.GetValueOrDefault() == true)
                    {
                        t.mstr_Process_LC_StatusID = input.statusWF;
                    }
                    var TSentry = entity.Where(o => ((o.tbl_Org_ProjectID == null && t.tbl_Org_ProjectID == null) || (o.tbl_Org_ProjectID == t.tbl_Org_ProjectID))
                                                && ((o.tbl_Org_Proj_PlanID == null && t.tbl_Org_Proj_PlanID == null) || (o.tbl_Org_Proj_PlanID == t.tbl_Org_Proj_PlanID))
                                                && ((o.tbl_Org_Plan_ResourceID == null && t.tbl_Org_Plan_ResourceID == null) || (o.tbl_Org_Plan_ResourceID == t.tbl_Org_Plan_ResourceID))
                                                && ((o.tbl_Mapped_Proj_ProcessID == null && t.tbl_Mapped_Proj_ProcessID == null) || (o.tbl_Mapped_Proj_ProcessID == t.tbl_Mapped_Proj_ProcessID))
                                                && ((o.tbl_General_TaskID == null && t.tbl_General_TaskID == null) || (o.tbl_General_TaskID == t.tbl_General_TaskID))
                                                &&  o.Type == t.Type
                                                && ((o.TSDate == t.TSDate))).SingleOrDefault();
                    if (TSentry != null)
                    {
                        t.ID = TSentry.ID;
                        TSentry = editMapper.MapToEntity(t, TSentry);
                    }
                    else
                    {
                        var id = service.Create(createMapper.MapToEntity(t,new tbl_org_timesheet()));
                        t.ID = id;
                    }
                }
                service.Save();
                service.getRepo().executeStoredCommand("Exec sp_recalculate_TaskHours " + ((PAIdentity)User.Identity).clientID);

                scope.Complete();
            }
            // Check for any workflow 
            var user = ((PAIdentity)User.Identity);
            var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, null, user.IsAdmin(), input.statusWF, null, user.clientID.GetValueOrDefault());
            if (wf.Any())
            {
                ViewBag.WF = true;
                ViewBag.workflow = wf;
            }
            switch (input.ViewType)
            {
                case 1:
                    return View("DailyView", input);
                case 2:
                    return View("WeeklyView", input);
                case 3:
                    return View("MonthlyView", input);
                default:
                    return View("Index");
            }
        }

        public ActionResult CompleteTask(int id, byte viewType, System.DateTime viewDate)
        {
            try
            {
                var ctx = (Db) service.getRepo().getDBContext();
                var entity = ctx.tbl_org_plan_resource.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == id).SingleOrDefault();

                if (entity == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Task does not exist";
                    return View("ListItems/showError");
                }
                System.DateTime? tsEntry = ctx.tbl_org_timesheet.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_Plan_ResourceID == id).Max(a => a.TSDate);
                if (tsEntry == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Timesheet entry for this task not found. Enter the timesheet and then mark this task complete";
                    return View("ListItems/showError");
                }
                var completionStatus = ctx.mstr_process_lc_status.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                                           o.Type == 5 && o.IsComplete == true).FirstOrDefault();
                if (completionStatus == null)
                {
                    Response.StatusCode = 403;
                    ViewBag.ErrorMessage = "Timesheet Completion Status not found. Pl. define appropriate master data";
                    return View("ListItems/showError");
                }
                entity.ActualEnd = tsEntry;
                entity.mstr_Process_LC_StatusID = completionStatus.ID;
                ctx.SaveChanges();
                switch (viewType)
                {
                    case 1:
                        return RedirectToAction("DailyView", new { startDate = viewDate.ToString(Globals.DateFormatString) });
                    case 2:
                        return RedirectToAction("WeeklyView", new { startDate = viewDate.ToString(Globals.DateFormatString) });
                    case 3:
                        return RedirectToAction("MonthlyView", new { startDate = viewDate.ToString(Globals.DateFormatString) });
                    default:
                        return View("Index");
                }
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }


        public ActionResult ReOpenTask(int id, byte viewType, System.DateTime viewDate)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var ctx = (Db)service.getRepo().getDBContext();
                    var entity = ctx.tbl_org_plan_resource.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.ID == id).SingleOrDefault();

                    if (entity == null)
                    {
                        Response.StatusCode = 403;
                        ViewBag.ErrorMessage = "Task does not exist";
                        return View("ListItems/showError");
                    }
                    var completionStatus = ctx.mstr_process_lc_status.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID &&
                                                                               o.Type == 5 && o.IsPublish == true).FirstOrDefault();
                    var approvedStatusID = (completionStatus == null ? 0 : completionStatus.ID);
                    var tsEntry = ctx.tbl_org_timesheet.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_Plan_ResourceID == id
                                                                                  && o.mstr_Process_LC_StatusID == approvedStatusID).FirstOrDefault();
                    if (tsEntry != null)
                    {
                        Response.StatusCode = 403;
                        ViewBag.ErrorMessage = "Whole or part of timesheet entry for this task has been approved. This task cannot be re-opened.";
                        return View("ListItems/showError");
                    }
                    entity.ActualEnd = null;
                    ctx.SaveChanges();
                    scope.Complete();
                }
                switch (viewType)
                {
                    case 1:
                        return RedirectToAction("DailyView", new { startDate = viewDate.ToString(Globals.DateFormatString) });
                    case 2:
                        return RedirectToAction("WeeklyView", "Timesheet", new { startDate = viewDate.ToString(Globals.DateFormatString) });
                    case 3:
                        return RedirectToAction("MonthlyView", new { startDate = viewDate.ToString(Globals.DateFormatString) });
                    default:
                        return View("Index");
                }
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        protected override bool checkForDuplication(tbl_org_timesheetInput input)
        {
            /*
            var entity = service.Where(o => ((o.tbl_Process_Rep_TaskID == null && input.tbl_Process_Rep_TaskID == null) || o.tbl_Process_Rep_TaskID == input.tbl_Process_Rep_TaskID)
                                       && ((o.tbl_Process_RepositoryID == null && input.tbl_Process_RepositoryID == null) || o.tbl_Process_RepositoryID == input.tbl_Process_RepositoryID)
                                       && o.PlanID == input.PlanID && o.PlannedStartDate == input.PlannedStartDate && o.PlannedEndDate == input.PlannedEndDate && o.PlannedDuration == input.PlannedDuration);
            if (entity.Any()) return true;
            else    */
                return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_timesheetInput input)
        {
            /*
            var entity = service.Where(o => ((o.tbl_Process_Rep_TaskID == null && input.tbl_Process_Rep_TaskID == null) || o.tbl_Process_Rep_TaskID == input.tbl_Process_Rep_TaskID)
                                       && ((o.tbl_Process_RepositoryID == null && input.tbl_Process_RepositoryID == null) || o.tbl_Process_RepositoryID == input.tbl_Process_RepositoryID)
                                       && o.PlanID == input.PlanID && o.PlannedStartDate == input.PlannedStartDate && o.PlannedEndDate == input.PlannedEndDate
                                       && o.PlannedDuration == input.PlannedDuration && o.ID != input.ID);
            if (entity.Any()) return true;
            else    */
                return false;
        }

        public ActionResult EmployeeTimesheet (int employeeID, System.DateTime startDate, System.DateTime endDate, int? resourceID)
        {
            try
            {
                var input = new timesheetEntry()
                {
                    TS_StartDate = startDate,
                    TS_EndDate = endDate,
                    tbl_Org_EmployeeID = employeeID,
                    StatusID = null,
                    ViewType = 1
                };
                input.tbl_org_timesheet = new List<tbl_org_timesheetInput>();
                var ctx = (Db)service.getRepo().getDBContext();
                var TSEntry = ctx.vw_timesheetEntry.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_EmployeeID == employeeID
                                                         && o.TSDate >= startDate && o.TSDate <= endDate
                                                         && (resourceID == null || o.tbl_Org_Plan_ResourceID == resourceID)
                                                         && o.tbl_Org_EmployeeID == employeeID
                                                         && o.Duration > 0);
                Mapper<vw_timesheetEntry, tbl_org_timesheetInput> tsMapper = new Mapper<vw_timesheetEntry, tbl_org_timesheetInput>();
                foreach (var ts in TSEntry)
                {
                    input.tbl_org_timesheet.Add(tsMapper.MapToInput(ts));
                }
                return View(input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult EmployeeTimesheetForTask(int resourceID)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var TSEntry = ctx.vw_timesheetEntry.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID && o.tbl_Org_Plan_ResourceID == resourceID
                                                         && o.Duration > 0);
                var input = new timesheetEntry()
                {
                    TS_StartDate = System.DateTime.Now,
                    TS_EndDate = System.DateTime.Now,
                    tbl_Org_EmployeeID = (TSEntry.FirstOrDefault() == null ? 0 : TSEntry.FirstOrDefault().tbl_Org_EmployeeID),
                    StatusID = null,
                    ViewType = 1
                };
                input.tbl_org_timesheet = new List<tbl_org_timesheetInput>();
                Mapper<vw_timesheetEntry, tbl_org_timesheetInput> tsMapper = new Mapper<vw_timesheetEntry, tbl_org_timesheetInput>();
                foreach (var ts in TSEntry)
                {
                    input.tbl_org_timesheet.Add(tsMapper.MapToInput(ts));
                }
                return View("EmployeeTimesheet",input);
            }
            catch (PAException e)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = e.Message;
                return View("ListItems/showError");
            }
        }

        public ActionResult ApproveTS(int? type)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            if (type == null) type = 1;
            var employee = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            if (employee.EmployeeID == null)
            {
                ViewBag.ErrorMessage = "You are not an employee, hence cannot approve timesheets";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            var entity = ctx.TS_Review(employee.EmployeeID.GetValueOrDefault(), type.GetValueOrDefault());   // Second parameter is type indicating that TS's to be reviewed have to be extracted
            ViewBag.Type = type;
            return View(entity);
        }

        public ActionResult ShowTSForApproval(int? type, int empID, System.DateTime startDate)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            if (type == null) type = 1;

            int weekDay = (int)startDate.DayOfWeek;
            if (weekDay > 1)
            {
                startDate = startDate.AddDays((-1) * (weekDay - 1));
            }
            else
            {
                if (weekDay == 0) startDate = startDate.AddDays(-6);
            }

            var employee = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            
            if (employee.EmployeeID == null)
            {
                ViewBag.ErrorMessage = "You are not an employee, hence cannot approve timesheets";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            var employeeName = ctx.tbl_org_employee.Where(o => o.ID == empID).SingleOrDefault();
            if (employeeName == null)
            {
                ViewBag.ErrorMessage = "The employee whose timesheet is requested for review is not an employee of this company anymore.";
                return View("ListItems/showErrorPage");	// Return error in a page
            }
            else
            {
                ViewBag.Type = type;
                ViewBag.EmployeeName = employeeName.GivenName + " " + employeeName.FamilyName;
                ViewBag.EmployeeID = employeeName.ID;
                ViewBag.ApproverID = employee.EmployeeID;
                ViewBag.StartDate = startDate;
            }
            var entity = ctx.GetApproverTSDetails(employee.EmployeeID.GetValueOrDefault(), type.GetValueOrDefault(), empID, startDate);   // Second parameter is type indicating that TS's to be reviewed have to be extracted
            return View(entity);
        }

        public ActionResult ApproveTSHours(int empID, System.DateTime startDate, System.DateTime endDate)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var approvalStatus = ctx.mstr_process_lc_status.Where(o => o.Type == 5 && o.IsPublish == true).FirstOrDefault();
            if (approvalStatus == null)
            {
                ViewBag.ErrorMessage = "Approval Status not configured. Pl. contact the system administrator.";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            var approverID = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            if (approverID == null || (approverID != null && approverID.EmployeeID == null))
            {
                ViewBag.ErrorMessage = "You are not an authorised approver. Pl. contact the system administrator.";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            var result = ctx.reviewEmployeeTS(approverID.EmployeeID.GetValueOrDefault(), 1, empID, startDate, endDate, approvalStatus.ID);
            if (result <= 0)
            {
                ViewBag.ErrorMessage = "Timesheet not approved";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            else
            {
                return RedirectToAction("ShowTSForApproval", new { type = 2, empID = empID, startDate = startDate.ToString(Globals.DateFormatString) }); 
            }
        }
        
        public ActionResult RejectTSHours(int empID, System.DateTime startDate, System.DateTime endDate)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var approvalStatus = ctx.mstr_process_lc_status.Where(o => o.Type == 5 && o.IsPublish == true).FirstOrDefault();
            if (approvalStatus == null || (approvalStatus != null && approvalStatus.Prev_Status == null))
            {
                ViewBag.ErrorMessage = "Approval Status not configured. Pl. contact the system administrator.";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            var approverID = ctx.UserProfile.Where(o => o.ID == WebSecurity.CurrentUserId).SingleOrDefault();
            if (approverID == null || approverID.EmployeeID == null)
            {
                ViewBag.ErrorMessage = "You are not an authorised approver. Pl. contact the system administrator.";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            var result = ctx.reviewEmployeeTS(approverID.EmployeeID.GetValueOrDefault(), 2, empID, startDate, endDate, approvalStatus.Prev_Status.GetValueOrDefault());
            if (result <= 0)
            {
                ViewBag.ErrorMessage = "Timesheet not rejected. Pl. try again.";
                return View("ListItems/showError");	// Return error in a dialog box
            }
            else
            {
                return RedirectToAction("ApproveTS", new { type = 1 });
            }
        }
    
    }
}

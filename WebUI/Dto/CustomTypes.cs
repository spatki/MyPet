using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcessAccelerator.WebUI.Dto
{
    public class CustomType
    {
        public IEnumerable<SelectListItem> data { get; set; }

        public CustomType()
        {
            data = new List<SelectListItem>();
        }

        public SelectList getList(short selectedItem)
        {
            return new SelectList(data,"Value","Text",selectedItem.ToString());
        }

        public String getTypeName(short typeId)
        {
            var rec = data.Where(v => v.Value == typeId.ToString());
            if (rec.Any()) return rec.First().Text;
            else return "";
        }
    }

    public class RoleTypes : CustomType
    {
        public RoleTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Job",Value="1"},
                new SelectListItem {Text="Process",Value="2"},
            };
        }
    }

    public class ClientTypes : CustomType
    {
        public ClientTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Internal",Value="1"},
                new SelectListItem {Text="External",Value="2"},
            };
        }
    }

    public class ReferenceTypes : CustomType
    {
        public ReferenceTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Reference",Value="1"},
                new SelectListItem {Text="Mandatory",Value="2"},
                new SelectListItem {Text="Optional",Value="3"},
            };
        }
    }

    public class StatusType : CustomType    // Used in mstr_process_lc_type which is a generic master table to store statuses
    {
        public StatusType()     
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Level Status",Value="1"},
                new SelectListItem {Text="Activity Status",Value="2"},
                new SelectListItem {Text="Process Status",Value="3"},
                new SelectListItem {Text="Document Status",Value="4"},
                new SelectListItem {Text="Timesheet Status",Value="5"},
                new SelectListItem {Text="Project Status",Value="6"},
                new SelectListItem {Text="Employment Status",Value="7"},
                new SelectListItem {Text="Audit Status",Value="8"},
                new SelectListItem {Text="Defect Status",Value="9"},
                new SelectListItem {Text="Issue Status",Value="10"}
            };
        }
    }

    public class AuditActivityType : CustomType    // Used in mstr_process_lc_type which is a generic master table to store statuses
    {
        public AuditActivityType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="QA Observation",Value="1"},
                new SelectListItem {Text="Audit",Value="2"},
            };
        }
    }

    public class AuditFrequencyType : CustomType    
    {
        public AuditFrequencyType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="One Time",Value="1"},
                new SelectListItem {Text="Recurring",Value="2"},
            };
        }

        public string TrackingStatus (DateTime planStart, DateTime planEnd, DateTime? scheduleStart, DateTime? conductStart, short scheduleStatus)
        {
            string returnValue = "";
            var today = System.DateTime.Now.Date;
            var classLabel = "";
            var trackStatus = "";

            if (scheduleStart == null)
            {
                // No Audit is yet scheduled.
                // Check if it should be scheduled, which is true when the plan start has passed.
                if (planStart <= today)
                {
                    classLabel = "label-danger";
                    trackStatus = "Delayed to Schedule";
                }
                else
                {
                    if (planStart <= today.AddDays(7))      // Due in the coming week
                    {
                        classLabel = "label-warning";
                        trackStatus = "Due to Schedule";
                    }
                    else
                    {
                        classLabel = "label-success";
                        trackStatus = "To be Scheduled";
                    }
                }
            }
            else
            {
                if (conductStart == null)
                {
                    // Scheduled but not yet conducted
                    if (scheduleStart <= today)
                    {
                        classLabel = "label-danger";
                        trackStatus = "Audit delayed";
                    }
                    else
                    {
                        classLabel = "label-success";
                        trackStatus = "Scheduled";
                    }
                }
                else
                {
                    // Scheduled and audited
                    classLabel = "label-success";
                    AuditPlanStatus sts = new AuditPlanStatus();
                    trackStatus = sts.getTypeName(scheduleStatus);
                }
            }
            returnValue = "<span class='label " + classLabel + "'><strong>" + trackStatus + "</strong></span>";   
            return returnValue;
        }
    }

    public class AuditFrequency : CustomType    // Used in mstr_process_lc_type which is a generic master table to store statuses
    {
        public AuditFrequency()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Day (s)",Value="1"},
                new SelectListItem {Text="Week (s)",Value="2"},
                new SelectListItem {Text="Month (s)",Value="3"},
                new SelectListItem {Text="Year (s)",Value="4"},
            };
        }

        public int Days(byte frequency)
        {
            switch (frequency)
            {
                case 1:
                    return 1;
                case 2:
                    return 7;
                case 3:
                    return 30;
                case 4:
                    return 365;
                default:
                    return 0;
            }
        }
    }

    public class AuditPlanStatus : CustomType    // Used in mstr_process_lc_type which is a generic master table to store statuses
    {
        public AuditPlanStatus()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Planned",Value="1"},
                new SelectListItem {Text="Scheduled",Value="2"},
                new SelectListItem {Text="Audited",Value="3"},
                new SelectListItem {Text="Closed",Value="4"},
            };
        }
    }

    public class ActionItemStatus : CustomType    // Used in mstr_process_lc_type which is a generic master table to store statuses
    {
        public ActionItemStatus()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Open",Value="1"},
                new SelectListItem {Text="On Hold",Value="2"},
                new SelectListItem {Text="Closed",Value="3"},
            };
        }
    }

    public class ActivityType : CustomType      // Used while defining activities in a process repository
    {
        public ActivityType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Requirement",Value="1"},
                new SelectListItem {Text="Design",Value="2"},
                new SelectListItem {Text="Implementation",Value="3"},
                new SelectListItem {Text="Review",Value="4"},
                new SelectListItem {Text="Audit",Value="5"},
                new SelectListItem {Text="Other",Value="6"},
            };
        }
    }

    public class GenderType : CustomType      // Used while defining activities in a process repository
    {
        public GenderType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Male",Value="1"},
                new SelectListItem {Text="Female",Value="2"}
            };
        }
    }

    public class EmploymentStatusType : CustomType      // Used while defining activities in a process repository
    {
        public EmploymentStatusType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Probation",Value="1"},
                new SelectListItem {Text="Temporary",Value="2"},
                new SelectListItem {Text="On Contract",Value="3"},
                new SelectListItem {Text="On Hold",Value="4"},
                new SelectListItem {Text="Permanent ",Value="5"},
                new SelectListItem {Text="Serving Notice Period",Value="6"},
                new SelectListItem {Text="Resigned",Value="7"},
            };
        }
    }

    public class UserTypes : CustomType
    {
        public UserTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Employee",Value="1"},
                new SelectListItem {Text="Client",Value="2"},
                new SelectListItem {Text="Special",Value="3"},
            };
        }
    }

    public class SpecialUserTypes : CustomType
    {
        public SpecialUserTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Administrator",Value="1"},
                new SelectListItem {Text="Guest",Value="2"},
            };
        }
    }

    public class ProjectLocationTypes : CustomType
    {
        public ProjectLocationTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Internal",Value="1"},
                new SelectListItem {Text="Client",Value="2"},
                new SelectListItem {Text="On-Site",Value="3"},
            };
        }
    }

    public class DurationUnitTypes : CustomType
    {
        public DurationUnitTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Hour (s)",Value="1"},
                new SelectListItem {Text="Day (s)",Value="2"},
                new SelectListItem {Text="Month (s)",Value="3"},
            };
        }

        public string TrackingStatus(DateTime? plannedStart, DateTime? plannedEnd, DateTime? ActualStart, DateTime? ActualEnd, bool completionStatus, DateTime periodStart, DateTime periodEnd)
        {   
            string returnValue = "";
            if (plannedStart == null) return returnValue;
            var classLabel = "";
            var trackStatus = "";


            if (ActualStart == null)
            {
                if (plannedStart >= periodStart && plannedStart <= periodEnd)
                {
                    classLabel = "label-warning";
                    trackStatus = "Due to start";
                }
                else
                {
                    if (plannedStart < periodStart)
                    {
                        classLabel = "label-danger";
                        trackStatus = "Delayed to start";
                    }
                    else
                    {
                        classLabel = "label-success";
                        trackStatus = "Planned";
                    }
                }
            }
            else
            {
                if (plannedStart == ActualStart)
                {
                    classLabel = "label-success";
                    trackStatus = "Started on time";
                }
                else
                {
                    if (ActualStart > plannedStart)
                    {
                        classLabel = "label-warning";
                        trackStatus = "Started late";
                    }
                    else
                    {
                        classLabel = "label-success";
                        trackStatus = "Started early";
                    }
                }
            }
            if (ActualEnd == null)      // Check for tasks that have not yet ended
            {
                if (plannedEnd >= periodStart && plannedEnd <= periodEnd)
                {
                    classLabel = "label-warning";
                    trackStatus = "Due to Complete";
                }
                else
                {
                    if (plannedEnd < periodStart)
                    {
                        classLabel = "label-danger";
                        trackStatus = "Delayed to Complete";
                    }
                }
            }
            returnValue = "<span class='label " + classLabel + "'><strong>" + trackStatus + "</strong></span>";   
            return returnValue;
        }
    }

    public class DocType : CustomType      // Used while defining activities in a process repository
    {
        public DocType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Document",Value="1"},
                new SelectListItem {Text="Process Artifact",Value="2"},
                new SelectListItem {Text="Template",Value="3"},
                new SelectListItem {Text="Checklist",Value="4"},
            };
        }
    }

    public class DocReferenceType : CustomType      // Used while defining activities in a process repository
    {
        public DocReferenceType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Mandatory",Value="1"},
                new SelectListItem {Text="Optional",Value="2"},
                new SelectListItem {Text="Reference Only",Value="3"},
            };
        }
    }

    public class TSTaskType : CustomType      // Used while defining activities in a process repository
    {
        public TSTaskType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Assigned Tasks",Value="1"},
                new SelectListItem {Text="Review Tasks",Value="2"},
                new SelectListItem {Text="General Tasks",Value="3"},
            };
        }
    }

    public class RecordingType : CustomType      // Used while defining activities in a process repository
    {
        public RecordingType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Assigned",Value="1"},
                new SelectListItem {Text="Additional",Value="2"},
            };
        }
    }

    public class NCCategory : CustomType
    {
        public NCCategory()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Major",Value="1"},
                new SelectListItem {Text="Minor",Value="2"},
                new SelectListItem {Text="OFI",Value="3"},
            };
        }
    }

    public class WorkflowRoleTypes : CustomType
    {
        public WorkflowRoleTypes()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="Organisation Role",Value="1"},
                new SelectListItem {Text="System Role",Value="2"},
                new SelectListItem {Text="Reporting Manager",Value="3"},
                new SelectListItem {Text="Reviewer",Value="4"},
                new SelectListItem {Text="Project Approver",Value="5"},
                new SelectListItem {Text="Project Reviewer",Value="6"},
                new SelectListItem {Text="Project Role",Value="7"},
                new SelectListItem {Text="Project Member",Value="8"},
                new SelectListItem {Text="Individual",Value="9"}
            };
        }
    }
    
    public class WFActionType : CustomType
    {
        public WFActionType()
        {
            data = new List<SelectListItem>
            {
                new SelectListItem {Text="General",Value="1"},
                new SelectListItem {Text="Assign",Value="2"},
                new SelectListItem {Text="Review",Value="3"},
                new SelectListItem {Text="Complete",Value="4"},
                new SelectListItem {Text="Publish",Value="5"},
            };
        }
    }

}
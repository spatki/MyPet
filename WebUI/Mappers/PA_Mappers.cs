using System.Collections.Generic;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.Data;
using System.Data.Objects;
using System.Data.Entity;
using ProcessAccelerator.Core;
using System.Linq;
using System.Data;

namespace ProcessAccelerator.WebUI.Mappers
{
    public class mstr_process_levelMapper : Mapper <mstr_process_level,mstr_process_levelInput>
    {
        public override mstr_process_level MapToEntity(mstr_process_levelInput input, mstr_process_level e)
        {
            var entity = base.MapToEntity(input, e);

            if (entity.CreateDate == null) { entity.CreateDate = System.DateTime.Now; }
            if (entity.CreatedBy == null) { entity.CreatedBy = 1; }
            entity.UpdateDate = System.DateTime.Now;
            if (entity.UpdatedBy == null) { entity.UpdatedBy = 1; }
            return entity;
        }
    }

    public class mstr_process_roleMapper : Mapper<mstr_process_role, mstr_process_roleInput>
    {
        public override mstr_process_role MapToEntity(mstr_process_roleInput input, mstr_process_role e)
        {
            var entity = base.MapToEntity(input, e);

            if (entity.CreateDate == null) { entity.CreateDate = System.DateTime.Now; }
            if (entity.CreatedBy == null) { entity.CreatedBy = 1; }
            entity.UpdateDate = System.DateTime.Now;
            if (entity.UpdatedBy == null) { entity.UpdatedBy = 1; }
            return entity;
        }
    }

    public class mstr_org_levelMapper : Mapper<mstr_org_level, mstr_org_levelInput>
    {
        public override mstr_org_level MapToEntity(mstr_org_levelInput input, mstr_org_level e)
        {
            var entity = base.MapToEntity(input, e);

            if (entity.CreateDate == null) { entity.CreateDate = System.DateTime.Now; }
            if (entity.CreatedBy == null) { entity.CreatedBy = 1; }
            entity.UpdateDate = System.DateTime.Now;
            if (entity.UpdatedBy == null) { entity.UpdatedBy = 1; }
            return entity;
        }
    }

    public class tbl_process_procedureMapper : Mapper<tbl_process_procedure, tbl_process_procedureInput>
    {
        public override tbl_process_procedure MapToEntity(tbl_process_procedureInput input, tbl_process_procedure e)
        {
            return base.MapToEntity(input, e);
        }

        public override tbl_process_procedureInput MapToInput(tbl_process_procedure entity)
        {
            return base.MapToInput(entity);
        }
    }

    public class tbl_process_chklst_itemMapper : Mapper<tbl_process_chklst_item, tbl_process_chklst_itemInput>
    {
        public override tbl_process_chklst_item MapToEntity(tbl_process_chklst_itemInput input, tbl_process_chklst_item e)
        {
            string chklstOptions = "";

            foreach (var o in input.ChklstOptionList)
            {
                // Prepare a comma separated list of ids
                chklstOptions = chklstOptions + (chklstOptions == "" ? "" : ",") + o;
            }
            e = base.MapToEntity(input, e);
            e.Chklst_Options = chklstOptions;
            return e;
        }

        public override tbl_process_chklst_itemInput MapToInput(tbl_process_chklst_item entity)
        {
            var input =  base.MapToInput(entity);
            if (entity.Chklst_Options != null && entity.Chklst_Options != "")
            {
                var chklstOptions = entity.Chklst_Options.Split(',');
                input.ChklstOptionList = new List<string>();
                foreach (var o in chklstOptions)
                {
                    input.ChklstOptionList.Add(o);
                }
            }
            return input;
        }
    }

    public class tbl_org_proj_planMapper : Mapper<tbl_org_proj_plan, tbl_org_proj_planInput>
    {
        public override tbl_org_proj_plan MapToEntity(tbl_org_proj_planInput input, tbl_org_proj_plan e)
        {
            var entity = base.MapToEntity(input, e);
            Db ctx = new Db();

            if (input.tbl_org_plan_resource != null && input.tbl_org_plan_resource.Any())
            {
                var fixedActions = ctx.tbl_workflow_fixed_actions.Where(o => o.ClientID == input.ClientID &&
                                                                        o.FunctionID == input.FunctionID && o.PreStatusID == e.mstr_Process_LC_StatusID).FirstOrDefault();
                int subStatus = 0;
                if (fixedActions == null)
                {
                    subStatus = input.mstr_Process_LC_StatusID;
                }
                else
                {
                    subStatus = fixedActions.StatusID;
                    if (fixedActions.CascadeUpdate == true)
                    {
                        input.mstr_Process_LC_StatusID = subStatus;
                        entity.mstr_Process_LC_StatusID = subStatus;
                    }
                }

                tbl_org_plan_resource res;
                Mapper<tbl_org_plan_resource, tbl_org_plan_resourceInput> resMapper = new Mapper<tbl_org_plan_resource, tbl_org_plan_resourceInput>();
                if (e.tbl_org_plan_resource == null) e.tbl_org_plan_resource = new List<tbl_org_plan_resource>();
                // As this is s fixed function, check the workflow for any configuration
                foreach (var r in input.tbl_org_plan_resource)
                {
                    res = e.tbl_org_plan_resource.Where(o => o.ID == r.ID).SingleOrDefault();
                    if (res == null)
                    {
                        // this is a new entry
                        e.tbl_org_plan_resource.Add(new tbl_org_plan_resource()
                        {
                            ID = r.ID,
                            tbl_Org_Proj_PlanID = e.ID,
                            ClientID = e.ClientID,
                            tbl_Org_EmployeeID = r.tbl_Org_EmployeeID,
                            PlannedDuration = r.PlannedDuration,
                            PlannedStart = r.PlannedStart,
                            PlannedEnd = r.PlannedEnd,
                            AllocationPercent = r.AllocationPercent,
                            mstr_Process_LC_StatusID = subStatus,
                            OrgResource_AllocationID = r.OrgResource_AllocationID
                        });
                    }
                    else
                    {
                        // Update existing entry
                        r.mstr_Process_LC_StatusID = subStatus;
                        res = resMapper.MapToEntity(r, res);
                        res.tbl_Org_Proj_PlanID = e.ID;
                    }
                }
            }

            tbl_org_plan_document suppDoc;
            Mapper<tbl_org_plan_document, tbl_org_plan_documentInput> docMapper = new Mapper<tbl_org_plan_document, tbl_org_plan_documentInput>();
            if (e.tbl_org_plan_document == null) e.tbl_org_plan_document = new List<tbl_org_plan_document>();
            // check if there are supporting documents
            if (input.tbl_org_plan_document != null && input.tbl_org_plan_document.Any())
            {
                foreach (var doc in input.tbl_org_plan_document)
                {
                    suppDoc = e.tbl_org_plan_document.Where(o => o.ID == doc.ID).FirstOrDefault();
                    if (suppDoc == null)
                    {
                        if (doc.Include)
                        {
                            // this is a new entry
                            e.tbl_org_plan_document.Add(new tbl_org_plan_document()
                            {
                                tbl_Org_PlanID = e.ID,
                                ClientID = e.ClientID,
                                Name = doc.Name,
                                DocType = doc.DocType,
                                tbl_DocMgr_DocumentID = doc.tbl_DocMgr_DocumentID,
                                tbl_Process_ChecklistID = doc.tbl_Process_ChecklistID,
                                tbl_Process_ProcedureID = doc.tbl_Process_ProcedureID,
                                tbl_Process_TemplateID = doc.tbl_Process_TemplateID,
                                Source = doc.Source,
                                Remarks = doc.Remarks,
                                ReferenceType = doc.ReferenceType
                            });
                        }
                    }
                    else
                    {
                        if (doc.Include)
                        {
                            // update this entry
                            suppDoc.Remarks = doc.Remarks;
                            suppDoc.ReferenceType = doc.ReferenceType;
                        }
                    }
                }
            }
            ctx = null;
            return entity;
        }

        public override tbl_org_proj_planInput MapToInput(tbl_org_proj_plan entity)
        {
            var input = base.MapToInput(entity);

            tbl_org_plan_resourceMapper resMapper = new tbl_org_plan_resourceMapper();
            Mapper<tbl_org_plan_document, tbl_org_plan_documentInput> docMapper = new Mapper<tbl_org_plan_document, tbl_org_plan_documentInput>();

            if (entity.tbl_org_plan_resource != null && entity.tbl_org_plan_resource.Any())
            {
                foreach (var r in entity.tbl_org_plan_resource)
                {
                    input.tbl_org_plan_resource.Add(resMapper.MapToInput(r));
                }
            }
            if (entity.tbl_org_plan_document != null && entity.tbl_org_plan_document.Any())
            {
                foreach (var d in entity.tbl_org_plan_document)
                {
                    input.tbl_org_plan_document.Add(docMapper.MapToInput(d));
                    input.tbl_org_plan_document.Last().Include = true;
                }
            }

            var ctx = new Db();

            var projectID = entity.tbl_Org_ProjectID;
            var prj = ctx.tbl_org_project.Where(o => o.ID == projectID).SingleOrDefault();
            if (prj == null) throw new PAException("Project Not Found");
            input.ProjectType = prj.mstr_Org_Project_TypeID;
            ctx = null;
            return input;
        }
    }

    public class tbl_org_plan_resourceMapper : Mapper<tbl_org_plan_resource, tbl_org_plan_resourceInput>
    {
        public override tbl_org_plan_resourceInput MapToInput(tbl_org_plan_resource entity)
        {
            var input = base.MapToInput(entity);
            input.AllocationStart = input.PlannedStart;
            input.AllocationEnd = input.PlannedEnd;
            return input;
        }
    }

    public class tbl_org_plan_documentMapper : Mapper<tbl_org_plan_document, tbl_org_plan_documentInput>
    {
        public override tbl_org_plan_documentInput MapToInput(tbl_org_plan_document entity)
        {
            var input =  base.MapToInput(entity);
            input.Include = true;
            return input;
        }
    }

    public class tbl_process_general_taskMapper : Mapper<tbl_process_general_task, tbl_process_general_taskInput>
    {
        public override tbl_process_general_task MapToEntity(tbl_process_general_taskInput input, tbl_process_general_task e)
        {
            string rolIDs = "";

            if (input.roleIDs != null && input.roleIDs.Count > 0)
            {
                foreach (var o in input.roleIDs)
                {
                    // Prepare a comma separated list of ids
                    rolIDs = rolIDs + (rolIDs == "" ? "" : ",") + o;
                }
            }
            e = base.MapToEntity(input, e);
            e.mstr_Process_Role_Ids = rolIDs;
            return e;
        }

        public override tbl_process_general_taskInput MapToInput(tbl_process_general_task entity)
        {
            var input = base.MapToInput(entity);
            if (entity.mstr_Process_Role_Ids != null && entity.mstr_Process_Role_Ids != "")
            {
                var rolIDs = entity.mstr_Process_Role_Ids.Split(',');
                input.roleIDs = new List<string>();
                foreach (var o in rolIDs)
                {
                    input.roleIDs.Add(o);
                }
            }
            return input;
        }
    }

    public class tbl_org_level_organisationMapper : Mapper<tbl_org_level_organisation, tbl_org_level_organisationInput>
    {
        public override tbl_org_level_organisationInput MapToInput(tbl_org_level_organisation entity)
        {
            var input = base.MapToInput(entity);
            if (entity.mstr_org_level != null)
            {
                input.levelName = entity.mstr_org_level.ShortName;
            }
            if (entity.mstr_org_level_master != null)
            {
                input.masterDataName = entity.mstr_org_level_master.LongName;
            }
            return input;
        }
    }

    public class resourceWiseTaskMapper : Mapper<tbl_org_proj_allocation, resourceWiseTasks>
    {
        public override resourceWiseTasks MapToInput(tbl_org_proj_allocation entity)
        {
            var input = base.MapToInput(entity);
            input.EmployeeName = entity.tbl_org_employee.GivenName + " " + entity.tbl_org_employee.FamilyName;
            return input;
        }
    }

    public class tbl_audit_planMapper : Mapper<tbl_audit_plan, tbl_audit_planInput>
    {
        public override tbl_audit_plan MapToEntity(tbl_audit_planInput input, tbl_audit_plan e)
        {
            var entity = e;
            entity.ClientID = input.ClientID;
            entity.Comments = input.Comments;
            entity.AuditType = input.AuditType;
            entity.Duration = input.Duration;
            entity.DurationUnit = input.DurationUnit;
            entity.Finish = input.Finish;
            entity.Frequency = input.Frequency;
            entity.ID = input.ID;
            entity.Period = input.Period;
            entity.RefID = input.RefID;
            entity.Start = input.Start;
            entity.Status = input.Status;
            entity.tbl_Org_ProjectID = input.tbl_Org_ProjectID;
            entity.tbl_Mstr_Org_FunctionID = input.tbl_Mstr_Org_FunctionID;
            entity.Type = input.Type;
            if (entity.tbl_audit_role == null) entity.tbl_audit_role = new List<tbl_audit_role>();
            if (input.AuditorRoles != null && input.AuditorRoles.Any())
            {
                foreach (var rl in input.AuditorRoles)
                {
                    if (!entity.tbl_audit_role.Where(o => o.tbl_Org_RoleID == rl && o.Type == 1).Any())
                    {
                        entity.tbl_audit_role.Add(new tbl_audit_role()
                        {
                            tbl_Audit_PlanID = entity.ID,
                            tbl_Org_RoleID = rl,
                            Type = 1
                        });
                    }
                }
            }
            return entity;
        }

        public override tbl_audit_planInput MapToInput(tbl_audit_plan entity)
        {
            var input = base.MapToInput(entity);
            input.AuditorRoles = new List<int>();
            if (entity.tbl_audit_role != null && entity.tbl_audit_role.Any())
            {
                foreach (var rl in entity.tbl_audit_role)
                {
                    input.AuditorRoles.Add(rl.tbl_Org_RoleID);
                }
            }
            return input;
        }
    }

    public class tbl_audit_scheduleMapper : Mapper<tbl_audit_schedule, tbl_audit_scheduleInput>
    {
        public override tbl_audit_scheduleInput MapToInput(tbl_audit_schedule entity)
        {
            var input = base.MapToInput(entity);
            input.Audit_Roles = new List<int>();
            input.Auditors = new List<int>();
            input.Auditees = new List<int>();

            if (entity.tbl_audit_plan.tbl_audit_role != null && entity.tbl_audit_plan.tbl_audit_role.Any())
            {
                foreach (var rl in entity.tbl_audit_plan.tbl_audit_role)
                {
                    input.Audit_Roles.Add(rl.tbl_Org_RoleID);
                }
            }
            if (entity.tbl_audit_participant != null && entity.tbl_audit_participant.Any())
            {
                foreach (var pt in entity.tbl_audit_participant.Where(o => o.Type == 1))
                {
                    input.Auditors.Add(pt.tbl_Org_EmployeeID);
                }
                foreach (var at in entity.tbl_audit_participant.Where(o => o.Type == 2))
                {
                    input.Auditees.Add(at.tbl_Org_EmployeeID);
                }
            }
            input.Description = entity.tbl_audit_plan.Comments;
            return input;
        }

        public override tbl_audit_schedule MapToEntity(tbl_audit_scheduleInput input, tbl_audit_schedule e)
        {
            var entity = base.MapToEntity(input, e);
            if (input.Auditors != null && input.Auditors.Any())
            {
                if (entity.tbl_audit_participant == null) entity.tbl_audit_participant = new List<tbl_audit_participant>();
                foreach (var pt in input.Auditors)
                {
                    if (!entity.tbl_audit_participant.Where(o => o.tbl_Org_EmployeeID == pt && o.Type == 1).Any())
                    {
                        entity.tbl_audit_participant.Add (new tbl_audit_participant() {
                            Type = 1,
                            tbl_Org_EmployeeID = pt,
                            tbl_Audit_ScheduleID = entity.ID,
                            ClientID = entity.ClientID
                        });
                    }
                }
            }
            if (input.Auditees != null && input.Auditees.Any())
            {
                foreach (var pt in input.Auditees)
                {
                    if (!entity.tbl_audit_participant.Where(o => o.tbl_Org_EmployeeID == pt && o.Type == 2).Any())
                    {
                        entity.tbl_audit_participant.Add(new tbl_audit_participant()
                        {
                            Type = 2,
                            tbl_Org_EmployeeID = pt,
                            tbl_Audit_ScheduleID = entity.ID,
                            ClientID = entity.ClientID
                        });
                    }
                }
            }
            return entity;
        }
    }

    public class audit_recordingMapper : Mapper<tbl_audit_schedule, auditRecordingInput>
    {
        public override tbl_audit_schedule MapToEntity(auditRecordingInput input, tbl_audit_schedule e)
        {
            var entity = base.MapToEntity(input, e);
            if (entity.tbl_audit_observation == null) entity.tbl_audit_observation = new List<tbl_audit_observation>();
            if (input.tbl_audit_observation != null && input.tbl_audit_observation.Any())
            {
                tbl_audit_observation existingRow;
                Mapper<tbl_audit_observation, tbl_audit_observationInput> obsMapper = new Mapper<tbl_audit_observation, tbl_audit_observationInput>();
                foreach (var obs in input.tbl_audit_observation)
                {
                    existingRow = entity.tbl_audit_observation.Where(o => o.ID == obs.ID && o.tbl_Audit_ScheduleID == obs.tbl_Audit_ScheduleID).SingleOrDefault();
                    if (existingRow == null)
                    {
                        entity.tbl_audit_observation.Add(obsMapper.MapToEntity(obs,new tbl_audit_observation()));
                    }
                    else
                    {
                        existingRow = obsMapper.MapToEntity(obs,existingRow);
                    }
                }
            }
            if (input.tbl_audit_checklist != null && input.tbl_audit_checklist.Any())
            {
                tbl_audit_checklist existingRow;
                Mapper<tbl_audit_checklist, tbl_audit_checklistInput> chklstMapper = new Mapper<tbl_audit_checklist, tbl_audit_checklistInput>();
                foreach (var obs in input.tbl_audit_checklist)
                {
                    existingRow = entity.tbl_audit_checklist.Where(o => o.ID == obs.ID && o.tbl_Audit_ScheduleID == obs.tbl_Audit_ScheduleID).SingleOrDefault();
                    if (existingRow == null)
                    {
                        entity.tbl_audit_checklist.Add(chklstMapper.MapToEntity(obs,new tbl_audit_checklist()));
                    }
                    else
                    {
                        existingRow = chklstMapper.MapToEntity(obs, existingRow);
                    }
                }
            }

            return entity;
        }

        public override auditRecordingInput MapToInput(tbl_audit_schedule entity)
        {
            var input = base.MapToInput(entity);
            input.Audit_Reference = entity.tbl_audit_plan.RefID;
            input.ProjectName = entity.tbl_org_project.Name;
            input.tbl_audit_observation = new List<tbl_audit_observationInput>();
            input.tbl_audit_checklist = new List<tbl_audit_checklistInput>();
            if (entity.tbl_audit_observation != null && entity.tbl_audit_observation.Any())
            {
                Mapper<tbl_audit_observation, tbl_audit_observationInput> obsMapper = new Mapper<tbl_audit_observation, tbl_audit_observationInput>();
                foreach (var obs in entity.tbl_audit_observation)
                {
                    input.tbl_audit_observation.Add(obsMapper.MapToInput(obs));
                }
            }
            if (entity.tbl_audit_checklist != null && entity.tbl_audit_checklist.Any())
            {
                Mapper<tbl_audit_checklist, tbl_audit_checklistInput> chkMapper = new Mapper<tbl_audit_checklist, tbl_audit_checklistInput>();
                foreach (var chklst in entity.tbl_audit_checklist)
                {
                    input.tbl_audit_checklist.Add(chkMapper.MapToInput(chklst));
                }
            }
            input.tbl_audit_participant = new List<tbl_audit_participantInput>();
            if (entity.tbl_audit_participant != null && entity.tbl_audit_participant.Any())
            {
                Mapper<tbl_audit_participant, tbl_audit_participantInput> ptMapper = new Mapper<tbl_audit_participant, tbl_audit_participantInput>();
                foreach (var pt in entity.tbl_audit_participant)
                {
                    var ptDetails = ptMapper.MapToInput(pt);
                    ptDetails.Name = pt.tbl_org_employee.GivenName + " " + pt.tbl_org_employee.FamilyName;
                    input.tbl_audit_participant.Add(ptDetails);
                }
            }
            return input;
        }
    }

    public class tbl_org_audit_planMapper : Mapper<tbl_org_audit_plan, tbl_org_audit_planInput>
    {
        public override tbl_org_audit_plan MapToEntity(tbl_org_audit_planInput input, tbl_org_audit_plan e)
        {
            var entity = e;
            entity.ClientID = input.ClientID;
            entity.Comments = input.Comments;
            entity.AuditType = input.AuditType;
            entity.Duration = input.Duration;
            entity.DurationUnit = input.DurationUnit;
            entity.Finish = input.Finish;
            entity.Frequency = input.Frequency;
            entity.ID = input.ID;
            entity.Period = input.Period;
            entity.RefID = input.RefID;
            entity.Start = input.Start;

            if (entity.tbl_org_audit_role == null) entity.tbl_org_audit_role = new List<tbl_org_audit_role>();
            if (input.AuditorRoles != null && input.AuditorRoles.Any())
            {
                foreach (var rl in input.AuditorRoles)
                {
                    if (!entity.tbl_org_audit_role.Where(o => o.tbl_Org_RoleID == rl && o.Type == 1).Any())
                    {
                        entity.tbl_org_audit_role.Add(new tbl_org_audit_role()
                        {
                            tbl_Org_Audit_PlanID = entity.ID,
                            tbl_Org_RoleID = rl,
                            Type = 1
                        });
                    }
                }
            }
            return entity;
        }

        public override tbl_org_audit_planInput MapToInput(tbl_org_audit_plan entity)
        {
            var input = base.MapToInput(entity);
            input.AuditorRoles = new List<int>();
            if (entity.tbl_org_audit_role != null && entity.tbl_org_audit_role.Any())
            {
                foreach (var rl in entity.tbl_org_audit_role)
                {
                    input.AuditorRoles.Add(rl.tbl_Org_RoleID);
                }
            }
            return input;
        }
    }

    public class tbl_org_audit_scheduleMapper : Mapper<tbl_org_audit_schedule, tbl_org_audit_scheduleInput>
    {
        public override tbl_org_audit_scheduleInput MapToInput(tbl_org_audit_schedule entity)
        {
            var input = base.MapToInput(entity);
            input.Audit_Roles = new List<int>();
            input.Auditors = new List<int>();
            input.Auditees = new List<int>();

            if (entity.tbl_org_audit_plan.tbl_org_audit_role != null && entity.tbl_org_audit_plan.tbl_org_audit_role.Any())
            {
                foreach (var rl in entity.tbl_org_audit_plan.tbl_org_audit_role)
                {
                    input.Audit_Roles.Add(rl.tbl_Org_RoleID);
                }
            }
            if (entity.tbl_org_audit_participant != null && entity.tbl_org_audit_participant.Any())
            {
                foreach (var pt in entity.tbl_org_audit_participant.Where(o => o.Type == 1))
                {
                    input.Auditors.Add(pt.tbl_Org_EmployeeID);
                }
                foreach (var at in entity.tbl_org_audit_participant.Where(o => o.Type == 2))
                {
                    input.Auditees.Add(at.tbl_Org_EmployeeID);
                }
            }
            input.Description = entity.tbl_org_audit_plan.Comments;
            return input;
        }

        public override tbl_org_audit_schedule MapToEntity(tbl_org_audit_scheduleInput input, tbl_org_audit_schedule e)
        {
            var entity = base.MapToEntity(input, e);
            if (input.Auditors != null && input.Auditors.Any())
            {
                if (entity.tbl_org_audit_participant == null) entity.tbl_org_audit_participant = new List<tbl_org_audit_participant>();
                foreach (var pt in input.Auditors)
                {
                    if (!entity.tbl_org_audit_participant.Where(o => o.tbl_Org_EmployeeID == pt && o.Type == 1).Any())
                    {
                        entity.tbl_org_audit_participant.Add(new tbl_org_audit_participant()
                        {
                            Type = 1,
                            tbl_Org_EmployeeID = pt,
                            tbl_Org_Audit_ScheduleID = entity.ID,
                            ClientID = entity.ClientID
                        });
                    }
                }
                foreach (var pt in input.Auditees)
                {
                    if (!entity.tbl_org_audit_participant.Where(o => o.tbl_Org_EmployeeID == pt && o.Type == 2).Any())
                    {
                        entity.tbl_org_audit_participant.Add(new tbl_org_audit_participant()
                        {
                            Type = 2,
                            tbl_Org_EmployeeID = pt,
                            tbl_Org_Audit_ScheduleID = entity.ID,
                            ClientID = entity.ClientID
                        });
                    }
                }
            }
            return entity;
        }
    }

    public class org_audit_recordingMapper : Mapper<tbl_org_audit_schedule, orgAuditRecordingInput>
    {
        public override tbl_org_audit_schedule MapToEntity(orgAuditRecordingInput input, tbl_org_audit_schedule e)
        {
            var entity = base.MapToEntity(input, e);
            if (entity.tbl_org_audit_observation == null) entity.tbl_org_audit_observation = new List<tbl_org_audit_observation>();
            if (input.tbl_org_audit_observation != null && input.tbl_org_audit_observation.Any())
            {
                tbl_org_audit_observation existingRow;
                Mapper<tbl_org_audit_observation, tbl_org_audit_observationInput> obsMapper = new Mapper<tbl_org_audit_observation, tbl_org_audit_observationInput>();
                foreach (var obs in input.tbl_org_audit_observation)
                {
                    existingRow = entity.tbl_org_audit_observation.Where(o => o.ID == obs.ID && o.tbl_Org_Audit_ScheduleID == obs.tbl_Org_Audit_ScheduleID).SingleOrDefault();
                    if (existingRow == null)
                    {
                        entity.tbl_org_audit_observation.Add(obsMapper.MapToEntity(obs, new tbl_org_audit_observation()));
                    }
                    else
                    {
                        existingRow = obsMapper.MapToEntity(obs, existingRow);
                    }
                }
            }
            if (input.tbl_org_audit_addln_obs != null && input.tbl_org_audit_addln_obs.Any())
            {
                tbl_org_audit_addln_obs existingRow;
                Mapper<tbl_org_audit_addln_obs, tbl_org_audit_addln_obsInput> chklstMapper = new Mapper<tbl_org_audit_addln_obs, tbl_org_audit_addln_obsInput>();
                var newID = (entity.tbl_org_audit_addln_obs.Any() ? entity.tbl_org_audit_addln_obs.Max(o => o.ID) + 1 : 1);
                foreach (var obs in input.tbl_org_audit_addln_obs)
                {
                    if (obs.ID == null || obs.ID == 0)
                    {
                        obs.ID = newID;
                        newID += 1;
                        entity.tbl_org_audit_addln_obs.Add(chklstMapper.MapToEntity(obs, new tbl_org_audit_addln_obs()));
                    }
                    else
                    {
                        existingRow = entity.tbl_org_audit_addln_obs.Where(o => o.ID == obs.ID && o.tbl_Org_Audit_ScheduleID == obs.tbl_Org_Audit_ScheduleID).SingleOrDefault();
                        if (existingRow == null)
                        {
                            entity.tbl_org_audit_addln_obs.Add(chklstMapper.MapToEntity(obs, new tbl_org_audit_addln_obs()));
                        }
                        else
                        {
                            existingRow = chklstMapper.MapToEntity(obs, existingRow);
                        }
                    }
                }
            }

            return entity;
        }

        public override orgAuditRecordingInput MapToInput(tbl_org_audit_schedule entity)
        {
            var input = base.MapToInput(entity);
            input.Audit_Reference = entity.tbl_org_audit_plan.RefID;
            input.tbl_org_audit_observation = new List<tbl_org_audit_observationInput>();
            input.tbl_org_audit_addln_obs = new List<tbl_org_audit_addln_obsInput>();
            if (entity.tbl_org_audit_observation != null && entity.tbl_org_audit_observation.Any())
            {
                Mapper<tbl_org_audit_observation, tbl_org_audit_observationInput> obsMapper = new Mapper<tbl_org_audit_observation, tbl_org_audit_observationInput>();
                foreach (var obs in entity.tbl_org_audit_observation)
                {
                    input.tbl_org_audit_observation.Add(obsMapper.MapToInput(obs));
                }
            }
            if (entity.tbl_org_audit_addln_obs != null && entity.tbl_org_audit_addln_obs.Any())
            {
                Mapper<tbl_org_audit_addln_obs, tbl_org_audit_addln_obsInput> chkMapper = new Mapper<tbl_org_audit_addln_obs, tbl_org_audit_addln_obsInput>();
                foreach (var chklst in entity.tbl_org_audit_addln_obs)
                {
                    input.tbl_org_audit_addln_obs.Add(chkMapper.MapToInput(chklst));
                }
            }
            input.tbl_org_audit_participant = new List<tbl_org_audit_participantInput>();
            if (entity.tbl_org_audit_participant != null && entity.tbl_org_audit_participant.Any())
            {
                Mapper<tbl_org_audit_participant, tbl_org_audit_participantInput> ptMapper = new Mapper<tbl_org_audit_participant, tbl_org_audit_participantInput>();
                foreach (var pt in entity.tbl_org_audit_participant)
                {
                    var ptDetails = ptMapper.MapToInput(pt);
                    ptDetails.Name = pt.tbl_org_employee.GivenName + " " + pt.tbl_org_employee.FamilyName;
                    input.tbl_org_audit_participant.Add(ptDetails);
                }
            }
            return input;
        }
    }

    public class org_general_tasksMapper : Mapper<mstr_org_general_tasks, mstr_org_general_tasksInput>
    {
        public override mstr_org_general_tasksInput MapToInput(mstr_org_general_tasks entity)
        {
            var input = base.MapToInput(entity);
            input.Roles = new List<string>();
            if (entity.tbl_org_general_task_roles != null && entity.tbl_org_general_task_roles.Any())
            {
                foreach (var rl in entity.tbl_org_general_task_roles)
                {
                    input.Roles.Add(rl.tbl_Org_RoleID.ToString());
                }
            }
            return input;
        }

        public override mstr_org_general_tasks MapToEntity(mstr_org_general_tasksInput input, mstr_org_general_tasks e)
        {
            e = base.MapToEntity(input, e);
            if (e.tbl_org_general_task_roles == null) e.tbl_org_general_task_roles = new List<tbl_org_general_task_roles>();
            if (input.Roles != null && input.Roles.Any())
            {
                // Add new roles
                if (input.Roles != null && input.Roles.Any())
                {
                    foreach (var rl in input.Roles)
                    {
                        if (!e.tbl_org_general_task_roles.Where(o => o.tbl_Org_RoleID == int.Parse(rl)).Any())
                        {
                            e.tbl_org_general_task_roles.Add(new tbl_org_general_task_roles()
                            {
                                ClientID = e.ClientID,
                                tbl_Org_General_TaskID = e.ID,
                                tbl_Org_RoleID = int.Parse(rl)
                            });
                        }
                    }
                }
            }
            return e;
        }
    }

    public class proj_GTTailorMapper : Mapper<tbl_proj_general_tasks, tbl_proj_general_tasksInput>
    {
        public override tbl_proj_general_tasksInput MapToInput(tbl_proj_general_tasks entity)
        {
            var input = base.MapToInput(entity);
            input.Roles = new List<string>();
            if (entity.tbl_proj_general_task_roles != null && entity.tbl_proj_general_task_roles.Any())
            {
                foreach (var rl in entity.tbl_proj_general_task_roles)
                {
                    input.Roles.Add(rl.tbl_Org_RoleID.ToString());
                }
            }
            if (entity.mstr_org_general_tasks != null)
            {
                input.GeneralTask = entity.mstr_org_general_tasks.Task;
            }
            if (entity.tbl_org_project != null)
            {
                input.ProjectName = entity.tbl_org_project.Name;
            }
            return input;
        }

        public override tbl_proj_general_tasks MapToEntity(tbl_proj_general_tasksInput input, tbl_proj_general_tasks e)
        {
            e = base.MapToEntity(input, e);
            if (e.tbl_proj_general_task_roles == null) e.tbl_proj_general_task_roles = new List<tbl_proj_general_task_roles>();
            if (input.Roles != null && input.Roles.Any())
            {
                // Add new roles
                if (input.Roles != null && input.Roles.Any())
                {
                    foreach (var rl in input.Roles)
                    {
                        if (!e.tbl_proj_general_task_roles.Where(o => o.tbl_Org_RoleID == int.Parse(rl)).Any())
                        {
                            e.tbl_proj_general_task_roles.Add(new tbl_proj_general_task_roles ()
                            {
                                ClientID = e.ClientID,
                                tbl_Proj_General_TaskID = e.ID,
                                tbl_Org_RoleID = int.Parse(rl)
                            });
                        }
                    }
                }
            }
            return e;
        }
    }

}

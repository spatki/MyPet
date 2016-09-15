using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class MapRepositoryController : Cruder<tbl_mapping_org_process, tbl_mapping_org_processInput>
    {
        public MapRepositoryController(ICrudService<tbl_mapping_org_process> service, IMapper<tbl_mapping_org_process, tbl_mapping_org_processInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFMAPCFG")
        {
            functionID = "DFMAPCFG";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public ActionResult getMapping(int? functionID, int? subFuncID, int? projType, int? projPhase, int? orgLevelID)
        {
            try
            {
                if (functionID == null && subFuncID == null && projType == null && projPhase == null && orgLevelID == null)
                {
                    // At least one parameter should be provided
                    throw new PAException("Mapping not found");
                }
                //var ctx = (Db)service.getRepo().getDBContext();
                var entity = service.Where(o => ((functionID == null & o.mstr_Org_FunctionID == null) | o.mstr_Org_FunctionID == functionID) & 
                                                ((subFuncID == null & o.mstr_Org_Sub_FunctionID == null) | o.mstr_Org_Sub_FunctionID == subFuncID) &
                                                ((projType == null & o.mstr_Org_Project_TypeID == null) | o.mstr_Org_Project_TypeID == projType) &
                                                ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &
                                                ((orgLevelID == null & o.tbl_Org_Level_OrganisationID == null) | o.tbl_Org_Level_OrganisationID == orgLevelID));
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                var returnList = from node in entity
                                 select new
                                 {
                                     ID = node.ID,
                                     accessID = "R" + node.tbl_Process_RepositoryID + (node.tbl_Process_Rep_TaskID == null ? "" : "A" + node.tbl_Process_Rep_TaskID),
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Add(int repoID, int? activityID, int? functionID, int? subFuncID, int? projType, int? projPhase, int? orgLevelID)
        {
            try
            {
                if (repoID == null && functionID == null && subFuncID == null && projType == null && projPhase == null && orgLevelID == null)
                {
                    throw new PAException("Insufficient information");
                }
                var entity = service.Where(o => ((functionID == null & o.mstr_Org_FunctionID == null) | o.mstr_Org_FunctionID == functionID) &
                                                ((subFuncID == null & o.mstr_Org_Sub_FunctionID == null) | o.mstr_Org_Sub_FunctionID == subFuncID) &
                                                ((projType == null & o.mstr_Org_Project_TypeID == null) | o.mstr_Org_Project_TypeID == projType) &
                                                ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &
                                                ((orgLevelID == null & o.tbl_Org_Level_OrganisationID == null) | o.tbl_Org_Level_OrganisationID == orgLevelID) & 
                                                ((activityID == null & o.tbl_Process_Rep_TaskID == null) | o.tbl_Process_Rep_TaskID == activityID) &
                                                o.tbl_Process_RepositoryID == repoID);
                if (!entity.Any())
                {
                    tbl_mapping_org_process mapInfo = new tbl_mapping_org_process()
                        { ClientID = ((PAIdentity)User.Identity).clientID,
                            mstr_Org_FunctionID = functionID,
                            mstr_Org_Sub_FunctionID = subFuncID,
                            mstr_Org_Project_TypeID = projType,
                            mstr_Org_Proj_PhaseID = projPhase,
                            tbl_Org_Level_OrganisationID = orgLevelID,
                            tbl_Process_RepositoryID = repoID,
                            tbl_Process_Rep_TaskID = activityID
                        };
                    var id = service.Create(mapInfo);
                    Response.StatusCode = 200;
                    return Json(new { Id = id, Type = typeof(tbl_mapping_org_process).Name.ToLower() }, JsonRequestBehavior.AllowGet);
                }
                Response.StatusCode = 200;
                return Json(new { Id = 1, Type = typeof(tbl_mapping_org_process).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 500;
                return Content(ex.Message);
            }
        }

        public ActionResult Remove(int repoID, int? activityID, int? functionID, int? subFuncID, int? projType, int? projPhase, int? orgLevelID)
        {
            try
            {
                if (repoID == null && functionID == null && subFuncID == null && projType == null && projPhase == null && orgLevelID == null)
                {
                    throw new PAException("Insufficient information");
                }
                var entity = service.Where(o => ((functionID == null & o.mstr_Org_FunctionID == null) | o.mstr_Org_FunctionID == functionID) &
                                                ((subFuncID == null & o.mstr_Org_Sub_FunctionID == null) | o.mstr_Org_Sub_FunctionID == subFuncID) &
                                                ((projType == null & o.mstr_Org_Project_TypeID == null) | o.mstr_Org_Project_TypeID == projType) &
                                                ((projPhase == null & o.mstr_Org_Proj_PhaseID == null) | o.mstr_Org_Proj_PhaseID == projPhase) &
                                                ((orgLevelID == null & o.tbl_Org_Level_OrganisationID == null) | o.tbl_Org_Level_OrganisationID == orgLevelID) &
                                                ((activityID == null & o.tbl_Process_Rep_TaskID == null) | o.tbl_Process_Rep_TaskID == activityID) &
                                                o.tbl_Process_RepositoryID == repoID);
                if (entity.Any())
                {
                    service.Delete(entity.First().ID);
                    Response.StatusCode = 200;
                    return Json(new { Id = 1, Type = typeof(tbl_mapping_org_process).Name.ToLower() }, JsonRequestBehavior.AllowGet);
                }
                Response.StatusCode = 200;
                return Json(new { Id = 1, Type = typeof(tbl_mapping_org_process).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException ex)
            {
                Response.StatusCode = 500;
                return Content(ex.Message);
            }
        }

        public virtual ActionResult getProjectTasks(int selectedItem, string controlName, string reload,int projectID, int? phase, int mode)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_project_process_mapping> list = new List<tbl_org_project_process_mapping>();
                exclude = new[] { 0 };
                include = new[] { 0 };
                var ctx = (Db)service.getRepo().getDBContext();

                if (mode == 1)
                {
                    list = ctx.tbl_org_project_process_mapping.Include("tbl_process_repository").Include("tbl_process_rep_task").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID
                                                                                                                                           && rec.tbl_Org_ProjectID == projectID
                                                                                                                                           && rec.Exclude != true
                                                                                                                                           && ((rec.mstr_Org_Proj_PhaseID == null && phase == null)
                                                                                                                                                || rec.mstr_Org_Proj_PhaseID == phase)
                                                                                                                                           && rec.tbl_Process_TaskID != null);
                }
                else
                {
                    list = ctx.tbl_org_project_process_mapping.Include("tbl_process_repository").Include("tbl_process_rep_task").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID
                                                                                                                                           && rec.tbl_Org_ProjectID == projectID
                                                                                                                                           && rec.Exclude != true
                                                                                                                                           && ((rec.mstr_Org_Proj_PhaseID == null && phase == null)
                                                                                                                                                || rec.mstr_Org_Proj_PhaseID == phase)
                                                                                                                                           && (rec.TreatAsTask == true || rec.tbl_Process_TaskID != null));
                }

                var returnList = list.ToList().Select(node => new GroupListItem
                {
                    ID = node.ID,
                    GroupName = (node.tbl_Process_TaskID == null ? "" : node.tbl_process_repository.Name),
                    DisplayText = (node.tbl_Process_TaskID == null ? node.tbl_process_repository.Name : node.tbl_process_rep_task.Name)
                });

                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                ViewBag.reload = reload;
                return PartialView("ListItems/listGroupCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getProjectTasksJSON(int projectID, int? phase, int mode)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_project_process_mapping> list = new List<tbl_org_project_process_mapping>();
                exclude = new[] { 0 };
                include = new[] { 0 };
                var ctx = (Db)service.getRepo().getDBContext();

                list = ctx.tbl_org_project_process_mapping.Include("tbl_process_repository").Include("tbl_process_rep_task").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID
                                                                                                                                        && rec.tbl_Org_ProjectID == projectID
                                                                                                                                        && rec.Exclude != true
                                                                                                                                        && ((rec.mstr_Org_Proj_PhaseID == null && phase == null)
                                                                                                                                            || rec.mstr_Org_Proj_PhaseID == phase)
                                                                                                                                        && ((mode == 1 && rec.tbl_Process_TaskID != null) || (mode != 1 && (rec.TreatAsTask == true || rec.tbl_Process_TaskID != null))));
                if (!list.Any() && phase != null)
                {
                    // No task mappings done for the phase. Then apply mappings for the project
                    list = ctx.tbl_org_project_process_mapping.Include("tbl_process_repository").Include("tbl_process_rep_task").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID
                                                                                                                                            && rec.tbl_Org_ProjectID == projectID
                                                                                                                                            && rec.Exclude != true
                                                                                                                                            && ((mode == 1 && rec.tbl_Process_TaskID != null) || (mode != 1 && (rec.TreatAsTask == true || rec.tbl_Process_TaskID != null))));

                }
                var returnList = list.ToList().Select(node => new GroupListItem
                {
                    ID = node.ID,
                    GroupName = (node.tbl_Process_TaskID == null ? "" : node.tbl_process_repository.Name),
                    DisplayText = (node.tbl_Process_TaskID == null ? node.tbl_process_repository.Name : node.tbl_process_rep_task.Name)
                });

                return Json(returnList.OrderBy(o => o.GroupName), JsonRequestBehavior.AllowGet);
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }
    }
}

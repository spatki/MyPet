using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Data;
using System.Transactions;

namespace ProcessAccelerator.Service
{
    public class Workflow : IWorkflowService
    {
        protected IRepo<tbl_workflow> repo;
        protected IRepo<tbl_workflow_state> repoState;
        protected IRepo<tbl_workflow_fixed_actions> fixedActions;
        protected IRepo<tbl_workflow_state_history> repoHistory;

        public Workflow(IRepo<tbl_workflow> repo, IRepo<tbl_workflow_state> repoSt, IRepo<tbl_workflow_state_history> repoHist, IRepo<tbl_workflow_fixed_actions> repoActions)
        {
            this.repo = repo;
            this.repoState = repoSt;
            this.repoHistory = repoHist;
            this.fixedActions = repoActions;
        }

        public IQueryable<tbl_workflow> getFunctionStatus(string functionID, int? userID, int? roleID, bool? adminAccess, int? Status, int? FunctionKey, int clientID)
        {
            var existingUser = false;
            if (Status != null)
            {
                if (userID == null) return new List<tbl_workflow>().AsQueryable();
                if (FunctionKey != null)
                {
                    var state = repoState.Where(o => o.ClientID == clientID && o.FunctionID == functionID && o.RefID == FunctionKey).SingleOrDefault();
                    if (state == null || (userID != state.UserID && adminAccess == false)) return new List<tbl_workflow>().AsQueryable();
                }
                if (adminAccess != true) existingUser = true;
            }
            // Actions available will be executed in this priority order
            // 1. First check workflow options for general use for creating a new item
            var entity = repo.Where(o => o.ClientID == clientID && (o.FunctionID == functionID && (o.AdminAccess == null || o.AdminAccess == false) && o.UserID == null && o.RoleID == null
                                    && (Status == null & o.PreStatusID == null)));
            // 2. Options available if this user is a workflow user
            if (!entity.Any() && existingUser == true)
            {
                entity = repo.Where(o => o.ClientID == clientID && (o.FunctionID == functionID && (o.AdminAccess == null || o.AdminAccess == false) && o.UserID == null && o.RoleID == null
                                        && o.PreStatusID == Status));   // Assuming PreStatus is not null
            }
            // 3. Actions available for a particular role
            if (!entity.Any() && roleID != null && roleID > 0)
            {
                entity = repo.Where(o => o.ClientID == clientID && (o.FunctionID == functionID && (o.AdminAccess == null || o.AdminAccess == false) && o.UserID == null && o.RoleID == roleID
                                    && ((Status == null & o.PreStatusID == null) | o.PreStatusID == Status)));      // Assuming PreStatus may be null
            }
            // 4. Actions available for a particular user
            if (!entity.Any() && userID != null)
            {
                entity = repo.Where(o => o.ClientID == clientID && (o.FunctionID == functionID && (o.AdminAccess == null || o.AdminAccess == false) && o.UserID == userID && o.RoleID == null
                                    && ((Status == null & o.PreStatusID == null) | o.PreStatusID == Status)));  // Assuming PreStatus may be null
            }
            // 5. Actions available for a Admin User
            if (!entity.Any() && adminAccess == true)
            {
                entity = repo.Where(o => o.ClientID == clientID && (o.FunctionID == functionID && (o.AdminAccess != null && o.AdminAccess == true) 
                                    && ((Status == null & o.PreStatusID == null) | o.PreStatusID == Status)));  // Assuming PreStatus may be null
            }
            return entity;
        }

        public void saveFlow(int refID, int fromUser, int userID, string functionID, int clientID, int status, bool? workflowDirection, string reviewComments)
        {
            var state = repoState.Where(o => o.ClientID == clientID && o.RefID == refID && o.FunctionID == functionID).SingleOrDefault();
            if (state == null)
            {
                if (userID == 0) { userID = fromUser; }     // In case this workflow does not transfer to another user.
                repoState.Insert(new tbl_workflow_state() {
                                    ClientID = clientID,
                                    FunctionID = functionID,
                                    FromUserID = (workflowDirection == true ? fromUser.ToString() : null),
                                    UserID = userID,
                                    RefID = refID,
                                    UpdateDate = System.DateTime.Now,
                                 });
                repoState.Save();
            }
            else
            {
                string previousUser = state.FromUserID;   // default behaviour. Dont add user trail
                if (workflowDirection == true)  // Forward Direction
                {
                    if (userID != 0)
                    {
                        // Add user trail only if the workflow calls for a new workflow user.
                        previousUser = state.FromUserID + (state.FromUserID == null ? "" : ",") + fromUser.ToString();
                    }
                    else { userID = fromUser; } // In case this workflow does not transfer to another user.
                }
                else if (workflowDirection == false)    // Backward direction. Cases of reject
                {
                    if (userID == 0) { userID = fromUser; } // This will almost always be true.
                    var userhistory = state.FromUserID.Split(',');
                    if (userhistory == null || userhistory.Length == 0)
                    {
                        // previous user not found. So maintain the orginal user (current user)
                        userID = fromUser;
                    }
                    else
                    {
                        int lastUser = int.Parse(userhistory[userhistory.Length - 1]);
                        userID = lastUser;
                    }
                    previousUser = null;
                    for (var i = 0; i < userhistory.Length - 1; i++)
                    {
                        previousUser = previousUser + (previousUser == null ? "" : ",") + userhistory[i];
                    }
                }
                state.UpdateDate = System.DateTime.Now;
                state.ClientID = clientID;
                if (userID == 0) state.UserID = fromUser; else state.UserID = userID;
                state.FunctionID = functionID;
                state.RefID = refID;
                state.FromUserID = previousUser;
                repoState.Save();
            }
            // Create history record
            if (workflowDirection != null)
            {
            repoHistory.Insert(new tbl_workflow_state_history()
            {
                ClientID = clientID,
                FunctionID = functionID,
                UserID = userID,
                RefID = refID,
                Status = status,
                StatusDate = System.DateTime.Now.Date,
                ReviewComments = reviewComments
            });
            repoHistory.Save();
                }
        }

        public IQueryable<tbl_workflow_fixed_actions> getActionWorkflow(string functionID, int preStatus, int ClientID)
        {
            return fixedActions.Where(o => o.ClientID == ClientID && o.PreStatusID == preStatus && o.FunctionID == functionID);
        }

        public workflow_edit setWorkflow(workflow_edit info)
        {
            // Get action details for this workflow
            using (TransactionScope scope = new TransactionScope())
            {
                var ctx = (Db)repo.getDBContext();
                var actionDetails = ctx.tbl_workflow_implementation.Where(o => o.ID == info.Action).SingleOrDefault();
                if (actionDetails != null)
                {
                    info.ImplementationName = actionDetails.Name;
                    info.Controller = actionDetails.Controller;
                    info.ActionName = actionDetails.ActionName;
                    info.Dialog = actionDetails.IsReview;
                }
                else
                {
                    // Action details not found
                    throw new Exception("Action details not found");
                }
                if (info.RowIDs == null || (!info.RowIDs.Any()))
                {
                    info.RowIDs = new List<int?>();
                    // This is a new record. Hence add new records here
                    if (info.RoleAccess != null && info.RoleAccess.Any())
                    {
                        foreach (var r in info.RoleAccess)
                        {
                            var newRec = repo.Insert(new tbl_workflow()
                            {
                                PreStatusID = (info.PreStatusID == 0 ? null : info.PreStatusID),
                                PostStatusID = info.PostStatusID,
                                Status = info.PostStatusID.GetValueOrDefault(),
                                Action = info.Action,
                                ActionName = info.ActionName,
                                AdminAccess = (r == 0 ? true : false),
                                ClientID = info.ClientID,
                                ConfirmAction = info.ConfirmAction,
                                Controller = info.Controller,
                                Dialog = info.Dialog,
                                Editable = info.Editable,
                                FunctionID = info.FunctionID,
                                Review = info.Review,
                                RoleAccess = ((r == 0 || r == -1) ? null : r),
                                RoleID = info.RoleID,
                                RoleType = info.RoleType,
                                SendMail = info.SendMail,
                                Sequence = info.Sequence,
                                SuccessMessage = info.SuccessMessage,
                                TimeLimit = info.TimeLimit,
                                UserCaption = info.UserCaption,
                                UserID = info.UserID,
                                WorkFlow = info.Workflow
                            });
                            repo.Save();
                            info.RowIDs.Add(newRec.ID);
                        }
                    }
                    else
                    {
                        var newRec = repo.Insert(new tbl_workflow()
                        {
                            PreStatusID = (info.PreStatusID == 0 ? null : info.PreStatusID),
                            PostStatusID = info.PostStatusID,
                            Status = info.Status.GetValueOrDefault(),
                            Action = info.Action,
                            ActionName = info.ActionName,
                            AdminAccess = false,
                            ClientID = info.ClientID,
                            ConfirmAction = info.ConfirmAction,
                            Controller = info.Controller,
                            Dialog = info.Dialog,
                            Editable = info.Editable,
                            FunctionID = info.FunctionID,
                            Review = info.Review,
                            RoleAccess = null,
                            RoleID = info.RoleID,
                            RoleType = info.RoleType,
                            SendMail = info.SendMail,
                            Sequence = info.Sequence,
                            SuccessMessage = info.SuccessMessage,
                            TimeLimit = info.TimeLimit,
                            UserCaption = info.UserCaption,
                            UserID = info.UserID,
                            WorkFlow = info.Workflow
                        });
                        repo.Save();
                        info.RowIDs.Add(newRec.ID);
                    }
                }
                else
                {
                    // This is an existing record
                    List<int?> newRowIDs = new List<int?>();
                    var existingEntries = repo.Where(o => info.RowIDs.Contains(o.ID));
                    foreach (var rl in info.RoleAccess)
                    {
                        var uptRec = existingEntries.Where(o => o.RoleAccess == rl 
                            || (o.RoleAccess == null && rl == -1 && (o.AdminAccess == null || o.AdminAccess != true)) 
                            || (o.RoleAccess == null && rl == 0 && o.AdminAccess != null && o.AdminAccess == true)).SingleOrDefault();
                        // Update existing
                        if (uptRec != null)
                        {
                            uptRec.PreStatusID = info.PreStatusID;
                            uptRec.PostStatusID = info.PostStatusID;
                            uptRec.Status = info.Status.GetValueOrDefault();
                            uptRec.Action = info.Action;
                            uptRec.ActionName = info.ActionName;
                            uptRec.AdminAccess = (rl == 0 ? true : false);
                            uptRec.ClientID = info.ClientID;
                            uptRec.ConfirmAction = info.ConfirmAction;
                            uptRec.Controller = info.Controller;
                            uptRec.Dialog = info.Dialog;
                            uptRec.Editable = info.Editable;
                            uptRec.FunctionID = info.FunctionID;
                            uptRec.Review = info.Review;
                            uptRec.RoleAccess = ((rl == 0 || rl == -1) ? null : rl);
                            uptRec.RoleID = info.RoleID;
                            uptRec.RoleType = info.RoleType;
                            uptRec.SendMail = info.SendMail;
                            uptRec.Sequence = info.Sequence;
                            uptRec.SuccessMessage = info.SuccessMessage;
                            uptRec.TimeLimit = info.TimeLimit;
                            uptRec.UserCaption = info.UserCaption;
                            uptRec.UserID = info.UserID;
                            uptRec.WorkFlow = info.Workflow;
                            repo.Save();
                            newRowIDs.Add(uptRec.ID);
                        }
                        else
                        {
                            // Role not found hence add new
                            var newRec = repo.Insert(new tbl_workflow()
                            {
                                PreStatusID = info.PreStatusID,
                                PostStatusID = info.PostStatusID,
                                Status = info.Status.GetValueOrDefault(),
                                Action = info.Action,
                                ActionName = info.ActionName,
                                AdminAccess = (rl == 0 ? true : false),
                                ClientID = info.ClientID,
                                ConfirmAction = info.ConfirmAction,
                                Controller = info.Controller,
                                Dialog = info.Dialog,
                                Editable = info.Editable,
                                FunctionID = info.FunctionID,
                                Review = info.Review,
                                RoleAccess = ((rl == 0 || rl == -1) ? null : rl),
                                RoleID = info.RoleID,
                                RoleType = info.RoleType,
                                SendMail = info.SendMail,
                                Sequence = info.Sequence,
                                SuccessMessage = info.SuccessMessage,
                                TimeLimit = info.TimeLimit,
                                UserCaption = info.UserCaption,
                                UserID = info.UserID,
                                WorkFlow = info.Workflow
                            });
                            repo.Save();
                            newRowIDs.Add(newRec.ID);
                        }
                    }
                    // Delete role IDs that are not needed.
                    foreach (var delEntry in existingEntries.Where(o => (!info.RoleAccess.Contains(o.RoleAccess)) || (o.RoleAccess == null && (!info.RoleAccess.Contains(-1)))).ToList())
                    {
                        repo.Delete(delEntry);
                        ctx.Entry(delEntry).State = System.Data.Entity.EntityState.Deleted;
                    }
                    repo.Save();
                    info.RowIDs = newRowIDs;
                }
                scope.Complete();
            }
            return info;
        }

        public bool DeleteWF(List<int?> del)
        {
            if (del != null || del.Any())
            {
                var rec = repo.Where(o => del.Contains(o.ID));
                foreach (var wf in rec.ToList())
                {
                    repo.Delete(wf);
                }
                repo.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        public int StatusUserID(string functionID, int refID)
        {
            var StatusID = repoState.Where(o => o.FunctionID == functionID && o.RefID == refID).ToList();
            if (StatusID == null || !StatusID.Any())
                return 0;
            else
            {
                var userID = StatusID.Last().UserID;
                return userID;
            }
        }

        public bool DeleteRecord(string functionID, int refID)
        {
            try
            {
                repoState.executeStoredCommand("delete from tbl_workflow_state where FunctionID = '" + functionID + "' and RefID = " + refID);
                repoState.executeStoredCommand("delete from tbl_workflow_state_history where FunctionID = '" + functionID + "' and RefID = " + refID);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<tbl_workflow_state_history> GetHistory(string functionID, int refID)
        {
            var ctx = repoHistory.getDBContext();
            var history = repoHistory.Where(o => o.FunctionID == functionID && o.RefID == refID);
            if (history != null && history.Any())
            {
                foreach (var h in history)
                {
                    ctx.Entry(h).Reference(o => o.UserProfile).Load();
                    ctx.Entry(h).Reference(o => o.mstr_process_lc_status).Load();
                }
            }
            return history.ToList();
        }

        public virtual IEnumerable<tbl_workflow_functions> getConfigurableFunctions()
        {
            var ctx = (Db) repo.getDBContext();
            return ctx.tbl_workflow_functions.AsEnumerable();
        }

        public virtual tmp_workflow_data getWorkflowForFunction(int clientID, string functionID)
        {
            tmp_workflow_data returnValue = new tmp_workflow_data() { FunctionID = functionID };
            var ctx = (Db) repo.getDBContext();
            var functionDetails = ctx.tbl_workflow_functions.Where(o => o.FunctionID == functionID).SingleOrDefault();
            // Get all masters
            returnValue.actions = ctx.tbl_workflow_implementation.Where(o => o.FunctionID == functionID);   // Load actions
            returnValue.role = ctx.mstr_org_role.Where(o => o.ClientID == clientID).ToList();    // Load roles
            returnValue.role.Add(new mstr_org_role() { ID = 0, ShortName = "Admin", LongName = "Administrative Access" });
            returnValue.role.Add(new mstr_org_role() { ID = -1, ShortName = "All", LongName = "All Roles" });
            returnValue.status = ctx.mstr_process_lc_status.Where(o => o.ClientID == clientID && o.Type == functionDetails.StatusTypeID).ToList(); // Status statuses for this client
            returnValue.status.Add(new mstr_process_lc_status() { ID = 0, Status = "New" });
            returnValue.emptyWorkflow = new workflow_edit() { FunctionID = functionID };
            // Add workflow user types
            returnValue.workflow_user_type = new List<user_type>()
            {
                new user_type {Text="Organisation Role",Value=1},
                //new user_type {Text="System Role",Value="2"},
                new user_type {Text="Reporting Manager",Value=3},
                new user_type {Text="Reviewer",Value=4},
                new user_type {Text="Project Approver",Value=5},
                new user_type {Text="Project Reviewer",Value=6},
                new user_type {Text="Project Role",Value=7},
                new user_type {Text="Project Member",Value=8},
                new user_type {Text="Individual",Value=9}
            };
            returnValue.workflow_direction = new List<workflow_direction>()
            {
                new workflow_direction { Value=null, Text="Not Applicable"},
                new workflow_direction { Value=true, Text="Proceed Forward"},
                new workflow_direction { Value=false, Text="Roll Back"},
            };
            // Get workflow data
            var wf_data = ctx.vw_workflow.Where(o => o.ClientID == clientID && o.FunctionID == functionID)
                          .OrderBy(o => o.Action).ThenBy(o => o.UserCaption).ThenBy(o => o.Sequence).ThenBy(o => o.Status)
                          .ThenBy(o => o.PreStatusID).ThenBy(o => o.PostStatusID).ThenBy(o => o.SendMail).ThenBy(o => o.RoleID)
                          .ThenBy(o => o.RoleType).ThenBy(o => o.Editable).ThenBy(o => o.Review).ThenBy(o => o.UserID)
                          .ThenBy(o => o.SuccessMessage).ThenBy(o => o.Dialog)
                          .ThenBy(o => o.ConfirmAction).ThenBy(o => o.Workflow);    // Load workflow
            returnValue.workflow = new List<workflow_edit>();
            var prevRecord = new vw_workflow();
            var id = 1;
            foreach (var w in wf_data)
            {
                if (prevRecord.Action != w.Action || prevRecord.UserCaption != w.UserCaption || prevRecord.Sequence != w.Sequence || prevRecord.Status != w.Status || 
                    prevRecord.PreStatusID != w.PreStatusID || prevRecord.PostStatusID != w.PostStatusID ||  
                    prevRecord.SendMail != w.SendMail || prevRecord.RoleID != w.RoleID || prevRecord.RoleType != w.RoleType || 
                    prevRecord.Editable != w.Editable || prevRecord.Review != w.Review || prevRecord.UserID != w.UserID || 
                    prevRecord.SuccessMessage != w.SuccessMessage || prevRecord.Dialog != w.Dialog || prevRecord.ConfirmAction != w.ConfirmAction ||
                    prevRecord.Workflow != w.Workflow)
                {
                    prevRecord = w;
                    returnValue.workflow.Add(new workflow_edit() {
                        ID = id++,
                        FunctionID = prevRecord.FunctionID,
                        Action = prevRecord.Action,
                        ImplementationName = prevRecord.ImplementationName,
                        UserCaption = prevRecord.UserCaption,
                        Sequence = prevRecord.Sequence ,
                        Status = prevRecord.Status ,
                        PreStatusID = (prevRecord.PreStatusID == null ?  0 : prevRecord.PreStatusID),
                        SendMail = prevRecord.SendMail ,
                        PostStatusID = prevRecord.PostStatusID ,
                        RoleAccess = new List<int?>() { prevRecord.RoleAccess == null ? (prevRecord.AdminAccess == true ? 0 : -1) : prevRecord.RoleAccess } ,
                        RoleID = prevRecord.RoleID ,
                        RoleType = prevRecord.RoleType ,
                        Editable = prevRecord.Editable ,
                        Review = prevRecord.Review ,
                        UserID = prevRecord.UserID ,
                        Controller = prevRecord.Controller ,
                        ActionName = prevRecord.ActionName ,
                        SuccessMessage = prevRecord.SuccessMessage ,
                        Dialog = prevRecord.Dialog ,
                        ConfirmAction = prevRecord.ConfirmAction ,
                        Workflow = prevRecord.Workflow ,
                        UserName = prevRecord.UserName ,
                        AccessRoleName = prevRecord.AccessRoleName ,
                        PreStatusName = prevRecord.PreStatusName ,
                        PostStatusName = prevRecord.PostStatusName ,
                        RowIDs = new List<int?>() { prevRecord.ID }
                    });
                }
                else
                {
                    returnValue.workflow.Last().RoleAccess.Add(w.RoleAccess == null ? (w.AdminAccess == true ? 0 : -1) : w.RoleAccess);
                    returnValue.workflow.Last().RowIDs.Add(w.ID);
                }
            }
            returnValue.newID = id;
            return returnValue;
        }

    }
}

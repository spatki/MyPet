using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;


namespace ProcessAccelerator.Core.Service
{
    public interface IWorkflowService
    {
        IQueryable<tbl_workflow> getFunctionStatus(string functionID, int? userID, int? roleID, bool? adminAccess, int? Status, int? FunctionKey, int clientID);
        void saveFlow(int refID, int fromUser, int userID, string functionID, int clientID, int status, bool? workflowDirection, string reviewComments);
        IQueryable<tbl_workflow_fixed_actions> getActionWorkflow(string functionID, int preStatus, int ClientID);
        workflow_edit setWorkflow(workflow_edit info);
        int StatusUserID(string functionID, int refID);
        List<tbl_workflow_state_history> GetHistory(string functionID, int refID);
        bool DeleteRecord(string functionID, int refID);
        IEnumerable<tbl_workflow_functions> getConfigurableFunctions();
        tmp_workflow_data getWorkflowForFunction(int clientID, string functionID);
        bool DeleteWF(List<int?> del);
    }
}

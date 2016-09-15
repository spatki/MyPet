using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Data;
using System.Linq;
using System.Transactions;

namespace ProcessAccelerator.Service
{
    public class AuditPlanService : CrudService<tbl_audit_plan>
    {
        public AuditPlanService(IRepo<tbl_audit_plan> repo)
            :base (repo)
        {
            this.repo = repo;
        }

        public override void LoadDependencies(tbl_audit_plan entity)
        {
            var ctx = (Db)repo.getDBContext();

            ctx.Entry(entity).Collection(o => o.tbl_audit_schedule).Load();
            ctx.Entry(entity).Collection(o => o.tbl_audit_role).Load();
        }

        public override void Delete(int id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                repo.executeStoredCommand("delete from tbl_audit_participant where tbl_Audit_ScheduleID in (select ID from tbl_audit_schedule where tbl_Audit_PlanID = " + id  + ")");
                repo.executeStoredCommand("delete from tbl_audit_schedule where tbl_Audit_PlanID = " + id);
                repo.executeStoredCommand("delete from tbl_audit_role where tbl_Audit_PlanID = " + id);
                repo.Delete(Get(id));
                repo.Save();
                scope.Complete();
            }
        }
    }
}

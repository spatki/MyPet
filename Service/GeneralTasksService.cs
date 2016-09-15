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
    public class GeneralTasksService : CrudService<mstr_org_general_tasks>
    {
        public GeneralTasksService(IRepo<mstr_org_general_tasks> repo)
            :base (repo)
        {
            this.repo = repo;
        }

        public override void LoadDependencies(mstr_org_general_tasks entity)
        {
            var ctx = (Db)repo.getDBContext();

            ctx.Entry(entity).Collection(o => o.tbl_org_general_task_roles).Load();
            foreach (var rl in entity.tbl_org_general_task_roles)
            {
                ctx.Entry(rl).Reference(o => o.mstr_org_role).Load();
            }
            ctx.Entry(entity).Reference(o => o.tbl_org_project).Load();
        }

        public override void Delete(int id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                repo.executeStoredCommand("delete from tbl_org_general_task_roles where tbl_Org_General_TaskID = " + id);
                repo.Delete(Get(id));
                repo.Save();
                scope.Complete();
            }
        }

        public override IEnumerable<mstr_org_general_tasks> GetAll()
        {
            var ctx = (Db) repo.getDBContext();
            return ctx.Set<mstr_org_general_tasks>().Include("tbl_org_general_task_roles").Include("tbl_org_general_task_roles.mstr_org_role").Include("tbl_org_project");
        }

        public override IEnumerable<mstr_org_general_tasks> Where(Expression<Func<mstr_org_general_tasks, bool>> predicate, bool showDeleted = false)
        {
            var ctx = (Db)repo.getDBContext();
            return ctx.mstr_org_general_tasks.Include("tbl_org_general_task_roles").Include("tbl_org_general_task_roles.mstr_org_role").Include("tbl_org_project").Where(predicate);
        }

    }
}

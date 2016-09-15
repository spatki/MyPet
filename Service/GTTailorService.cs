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
    public class GTTailorService : CrudService<tbl_proj_general_tasks>
    {
        public GTTailorService(IRepo<tbl_proj_general_tasks> repo)
            : base(repo)
        {
            this.repo = repo;
        }

        public override void LoadDependencies(tbl_proj_general_tasks entity)
        {
            var ctx = (Db)repo.getDBContext();

            ctx.Entry(entity).Reference(o => o.mstr_org_general_tasks).Load();
            ctx.Entry(entity).Reference(o => o.tbl_org_project).Load();
            ctx.Entry(entity).Collection(o => o.tbl_proj_general_task_roles).Load();
            foreach (var rl in entity.tbl_proj_general_task_roles)
            {
                ctx.Entry(rl).Reference(o => o.mstr_org_role).Load();
            }
        }

        public override void Delete(int id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var entity = Get(id);
                var tailored = entity.Tailored;
                repo.executeStoredCommand("delete from tbl_proj_general_task_roles where tbl_Proj_General_TaskID = " + id);
                repo.Delete(entity);
                repo.Save();
                if (tailored == true)
                {
                    // This task is owned by the project, hence delete it from the main master as well.
                    repo.executeStoredCommand("delete from tbl_org_general_task_roles where tbl_Org_General_TaskID = " + id);
                    repo.executeStoredCommand("delete from mstr_org_general_tasks where ID = " + id);
                }
                scope.Complete();
            }
        }

        public override IEnumerable<tbl_proj_general_tasks> GetAll()
        {
            var ctx = (Db)repo.getDBContext();
            return ctx.Set<tbl_proj_general_tasks>().Include("tbl_proj_general_task_roles")
                                             .Include("tbl_proj_general_task_roles.mstr_org_role")
                                             .Include("mstr_org_general_tasks")
                                             .Include("tbl_org_project");
        }

        public override IEnumerable<tbl_proj_general_tasks> Where(Expression<Func<tbl_proj_general_tasks, bool>> predicate, bool showDeleted = false)
        {
            var ctx = (Db)repo.getDBContext();
            return ctx.tbl_proj_general_tasks.Include("tbl_proj_general_task_roles")
                                             .Include("tbl_proj_general_task_roles.mstr_org_role")
                                             .Include("mstr_org_general_tasks")
                                             .Include("tbl_org_project")
                                             .Where(predicate);
        }

    }
}

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
    public class AuditFindingService : CrudService<tbl_audit_schedule>
    {

        public AuditFindingService(IRepo<tbl_audit_schedule> repo)
            : base(repo)
        {
            this.repo = repo;
        }

        public override void LoadDependencies(tbl_audit_schedule entity)
        {
            var ctx = (Db)repo.getDBContext();
            
            ctx.Entry(entity).Reference(o => o.tbl_audit_plan).Load();
            ctx.Entry(entity).Reference(o => o.tbl_org_project).Load();
            ctx.Entry(entity).Collection(o => o.tbl_audit_observation).Load();
            ctx.Entry(entity).Collection(o => o.tbl_audit_checklist).Load();
            ctx.Entry(entity).Collection(o => o.tbl_audit_participant).Load();
            if (entity.tbl_audit_participant != null && entity.tbl_audit_participant.Any())
            {
                foreach (var prt in entity.tbl_audit_participant)
                {
                    ctx.Entry(prt).Reference(o => o.tbl_org_employee).Load();
                }
            }
        }
    }
}

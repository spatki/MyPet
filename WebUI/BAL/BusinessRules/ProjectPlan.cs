using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Service;
using ProcessAccelerator.Core;
using ProcessAccelerator.Data;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.WebUI.Dto;

namespace ProcessAccelerator.WebUI.BAL.BusinessRules
{
    public class ProjectPlan : CrudService<tbl_org_proj_plan> 
    {
        public ProjectPlan(IRepo<tbl_org_proj_plan> repo)
            : base(repo)
        {
        }

        public override tbl_org_proj_plan Get(int id)
        {
            var entity = repo.Get(id);
            if (entity == null) return entity;

            var ctx = (Db) getRepo().getDBContext();
            ctx.Entry(entity).Collection(o => o.tbl_org_plan_resource).Load();
            ctx.Entry(entity).Collection(o => o.tbl_org_plan_document).Load();
            
            return entity;
        }
    }
}
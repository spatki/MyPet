using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class OrgSubFunctionController : Cruder<mstr_org_sub_function, mstr_org_sub_functionInput>
    {
        public OrgSubFunctionController(ICrudService<mstr_org_sub_function> service, IMapper<mstr_org_sub_function, mstr_org_sub_functionInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGFNSGRP")
        {
            functionID = "DFORGFNSGRP";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_org_sub_function o) { return o.LongName; }

        protected override bool checkForDuplication(mstr_org_sub_functionInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_sub_functionInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

    }
}

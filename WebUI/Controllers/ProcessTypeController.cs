using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.WebUI.BAL.AccessControl;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core;
using System.Transactions;
using WebMatrix.WebData;
using System.Globalization;
using System;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class ProcessTypeController : Cruder<mstr_process_type, mstr_process_typeInput>
    {
        public ProcessTypeController(ICrudService<mstr_process_type> service, IMapper<mstr_process_type, mstr_process_typeInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSMTRTYP")
        {
            functionID = "DFPRSMTRTYP";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName (mstr_process_type o)
        {
            return (o.ShortName == null) ? "" : o.ShortName; 
        }

        protected override bool checkForDuplication(mstr_process_typeInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_process_typeInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }
    }
}

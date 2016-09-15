using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class ProcessDocTypeController : Cruder<mstr_process_doc_type, mstr_process_doc_typeInput>
    {
        public ProcessDocTypeController(ICrudService<mstr_process_doc_type> service, IMapper<mstr_process_doc_type, mstr_process_doc_typeInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSDOCTYP")
        {
            functionID = "DFPRSDOCTYP";
        }
        //
        // GET: /ProcessDocType/

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_process_doc_type o) { return o.Name; }

        public override ActionResult Index()
        {
            return View();
        }

    }
}

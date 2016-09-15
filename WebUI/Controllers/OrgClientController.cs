using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.Data;
using System.Web.UI;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class OrgClientController : Cruder<mstr_org_client, mstr_org_clientInput>
    {
        public OrgClientController(ICrudService<mstr_org_client> service, IMapper<mstr_org_client, mstr_org_clientInput> v, IWorkflowService wf)
            : base(service, v, wf, "PL")
        {
            functionID = "PL";
        }
        //
        // GET: /Client/

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public override ActionResult getListItems(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            ViewBag.defaultItem = "New Client";
            return base.getListItems(selectedItem, controlName, excludeIds, selectIds, reload);
        }

        public virtual ActionResult getClientDetailJSON(int id)
        {
            try
            {
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID == id);
                var returnList = from node in list
                                     select new { ID = node.ID, Name = node.Name,
                                         Description = node.Description,
                                         Address = node.Address,
                                         Type = node.Type,
                                         PrimaryContact = node.PrimaryContact,
                                         PContactMailID = node.PContactMailID
                                    };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        protected override string listDisplayName(mstr_org_client o) { return o.Name; }

        protected override bool checkForDuplication(mstr_org_clientInput input)
        {
            var entity = service.Where(rec => rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_clientInput input)
        {
            var entity = service.Where(rec => rec.ID != input.ID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }
    }
}

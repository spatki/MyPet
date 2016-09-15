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
    public class ProcessLCStatusController : Cruder<mstr_process_lc_status, mstr_process_lc_statusInput>
    {
        public ProcessLCStatusController(ICrudService<mstr_process_lc_status> service, IMapper<mstr_process_lc_status, mstr_process_lc_statusInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSMTRSTS")
        {
            functionID = "DFPRSMTRSTS";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_process_lc_status o)
        {
            return (o.Status == null) ? "" : o.Status;
        }

        public override ActionResult ReloadItems()
        {
            int filter = 0;

            if (Request.QueryString["filter"] != null && Request.QueryString["filter"] != "") filter = int.Parse(Request.QueryString["filter"]);

            var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.Type == filter).OrderBy(o => o.SequenceNo);

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        public override ActionResult Create()
        {
            if (Request.QueryString["value"] == null || Request.QueryString["text"] == null)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = "Pl. select the status type before invoking this action";
                return View("ListItems/showError");
            }

            var input = createMapper.MapToInput(new mstr_process_lc_status());
            input.Type = short.Parse(Request.QueryString["value"]);
            return View(input);
        }

        public virtual ActionResult getStatusFor(int selectedItem, string controlName, string excludeIds, string selectIds, string reload, int Type, bool DefaultSelection)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<mstr_process_lc_status> list = new List<mstr_process_lc_status>();
                exclude = new[] { 0 };
                include = new[] { 0 };


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
                    }
                    else
                    {
                        if (Type > 0)
                        {
                            list = service.Where(rec => rec.Type == Type && rec.ClientID == ((PAIdentity)User.Identity).clientID);
                        }
                        else
                        {
                            list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
                        }
                    }
                }

                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

                if (DefaultSelection)
                {
                    if (selectedItem == 0)
                    {
                        if (list.Where(o => o.IsDefault == true).Any())
                        {
                            selectedItem = list.Where(o => o.IsDefault == true).First().ID;
                        }
                    }
                }
                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                ViewBag.reload = reload;
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        protected override bool checkForDuplication(mstr_process_lc_statusInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.Status.Trim().Equals(input.Status.Trim()) && rec.Type == input.Type);
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_process_lc_statusInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.Status.Trim().Equals(input.Status.Trim()) && rec.Type == input.Type);
            if (entity.Any()) return true;
            else return false;
        }

    }
}

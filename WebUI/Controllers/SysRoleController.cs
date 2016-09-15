using System.Web.Http;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using System.Linq;
using System.Collections.Generic;
using ProcessAccelerator.Core;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class SysRoleController : Cruder<webpages_Roles, webpages_RolesInput>
    {
        public SysRoleController(ICrudService<webpages_Roles> service, IMapper<webpages_Roles, webpages_RolesInput> v, IWorkflowService wf)
            : base(service, v, wf, "SYSUSR")
        {
            functionID = "SYSUSR";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(webpages_Roles o)
        {
            return (o.RoleName == null) ? "" : o.RoleName;
        }

        protected override bool checkForDuplication(webpages_RolesInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.RoleName.Trim().Equals(input.RoleName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(webpages_RolesInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.RoleName.Trim().Equals(input.RoleName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        public virtual ActionResult getListItemsFor(int selectedItem, string controlName, string excludeIds, string selectIds, string reload, int client)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<webpages_Roles> list = new List<webpages_Roles>();
                exclude = new[] { 0 };
                include = new[] { 0 };


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == client);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == client);
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == client);
                    }
                }

                var returnList = orderList(list).ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

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

        public virtual ActionResult getMultiSelectListFor(List<string> selectedItems, string controlName, string excludeIds, string selectIds, string reload, int client)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<webpages_Roles> list = new List<webpages_Roles>();
                exclude = new[] { 0 };
                include = new[] { 0 };


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == client);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == client);
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == client);
                    }
                }

                if (!list.Any())
                {
                    list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
                }

                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

                ViewBag.selectedItem = selectedItems;
                ViewBag.itemName = controlName;
                ViewBag.reload = reload;
                return PartialView("ListItems/multiSelectCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

    }
}

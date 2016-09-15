using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class PRoleController : Controller
    {
        private readonly IRepo<mstr_process_role> repo;

        public PRoleController(IRepo<mstr_process_role> repo)
        {
            this.repo = repo;
        }
        //
        // GET: /Roles/
        public ActionResult getRoles(int selectedItem, string controlName, string excludeIds, string selectIds)
        {
            try
            {
                IEnumerable<int> exclude;

                exclude = new[] { 0 };

                if (excludeIds != "" && excludeIds != null)
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                }

                var list = repo.Where(rec => !exclude.Contains(rec.ID)).OrderBy(o => o.ShortName);
              
                var returnList = list.ToList().Select(node => new SelectListItem
                                 {
                                     Value = node.ID.ToString(),
                                     Text = node.LongName
                                 });

                ViewBag.selectedItem = selectedItem;
                ViewBag.itemName = controlName;
                return PartialView("ListItems/listCombo",returnList.AsEnumerable());
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

    }
}

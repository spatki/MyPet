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
    public class OLevelController : Controller
    {
        private readonly IRepo<mstr_org_level> repo;

        public OLevelController(IRepo<mstr_org_level> repo)
        {
            this.repo = repo;
        }
        //
        // GET: /OLevel/

        public ActionResult getOrgLevels(int selectedItem, string controlName, string excludeIds, string selectIds)
        {
            try
            {
                IEnumerable<int> exclude;

                exclude = new[] { 0 };

                if (excludeIds != "")
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
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }
    }
}

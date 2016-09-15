using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Data;
using ProcessAccelerator.Service;


namespace ProcessAccelerator.WebUI.Controllers
{
    public class AjaxListController : Controller
    {
        public ActionResult getRoleTypes()
        {
            return Json(new List<SelectableItem>
            {
                new SelectableItem(0,"Select"),
                new SelectableItem(1,"Process"),
                new SelectableItem(2,"Job")
            },JsonRequestBehavior.AllowGet);
        }

        public ActionResult getOrgRoles()
        {
 /*           CrudService<mstr_process_role> repo = new CrudService<mstr_process_role>(new Repo<mstr_process_role>(new DbContextFactory()));

            var list = repo.GetAll();

            return Json(list,JsonRequestBehavior.AllowGet); */
            try
            {
                using (var db = new Db())
                {
                    var orgRoles = db.mstr_org_role.Select(l => new SelectListItem {
                        Value = l.ID.ToString(), Text = l.ShortName
                    });

                    return Json(orgRoles.ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ex.Raize();
            } 
            return null;
        }
    }
}

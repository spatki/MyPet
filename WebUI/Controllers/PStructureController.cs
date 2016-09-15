using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class PStructureController : Controller
    {
        private readonly IRepo<tbl_process_structure> repo;

        public PStructureController(IRepo<tbl_process_structure> repo)
        {
            this.repo = repo;
        }
        //
        // GET: /PStructure/

        public ActionResult getStructure()
        {
            try
            {
                var list = repo.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(o => o.Level).ThenBy(o => o.Sequence); 

                var returnList = from node in list
                                 select new
                                 {
                                     ID = node.ID,
                                     nodeID = node.mstr_Process_RoleID,
                                     nodeName = node.mstr_process_role.LongName,
                                     Level = node.Level,
                                     Sequence = node.Sequence,
                                     ParentNodeID = node.ParentRoleID
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    [InitializeSimpleMembership]
    public class OrgLevelController : Cruder<mstr_org_level, mstr_org_levelInput>
    {
        public OrgLevelController(ICrudService<mstr_org_level> service, IMapper<mstr_org_level, mstr_org_levelInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGLVL")
        {
            functionID = "DFORGLVL";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public ActionResult getOrgLevels(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;

                exclude = new[] { 0 };

                if (excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                }

                var list = service.Where(rec => !exclude.Contains(rec.ID)).OrderBy(o => o.ShortName);

                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = node.LongName
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

        public ActionResult getOrgStructure()
        {
            try
            {
                var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
                if (list.Any())
                {
                    var ctx = service.getRepo().getDBContext();
                    foreach (var m in list)
                    {
                        ctx.Entry(m).Collection(o => o.mstr_org_level_master).Load();
                    }
                }
                return View(list);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }


        public virtual ActionResult getListItemsFor(int selectedItem, string controlName, string excludeIds, string selectIds, short levelID, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<mstr_org_level> list = new List<mstr_org_level>();
                exclude = new[] { 0 };
                include = new[] { 0 };


                if (excludeIds != null & excludeIds != "")
                {
                    exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                    list = service.Where(rec => !exclude.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.LevelSequence == levelID);
                }
                else
                {
                    if (selectIds != null & selectIds != "")
                    {
                        include = selectIds.Split(',').Select(str => int.Parse(str));
                        list = service.Where(rec => include.Contains(rec.ID) && rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.LevelSequence == levelID);
                    }
                    else
                    {
                        list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.LevelSequence == levelID);
                    }
                }

                var returnList = list.ToList().Select(node => new SelectListItem
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

        protected override string listDisplayName(mstr_org_level o) { return o.LongName; }

        protected override bool checkForDuplication(mstr_org_levelInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_org_levelInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override void ReSequenceBeforeCreate(mstr_org_levelInput input) 
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.LevelSequence >= input.LevelSequence && o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(p => p.LevelSequence);
                if (restEntries.Any())
                {
                    short sequence = (short)(input.LevelSequence + 1);
                    foreach (var r in restEntries)
                    {
                        if (r.ID != input.ID)
                        {
                            r.LevelSequence = sequence;
                            sequence = (short)(sequence + 1);
                        }
                    }
                }
                service.Save();
            }
        }

        protected override void ReSequenceBeforeEdit(mstr_org_levelInput input) 
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.LevelSequence >= input.LevelSequence && o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(p => p.LevelSequence);
                if (restEntries.Any())
                {
                    short sequence = (short)(input.LevelSequence + 1);
                    foreach (var r in restEntries)
                    {
                        if (r.ID != input.ID)
                        {
                            r.LevelSequence = sequence;
                            sequence = (short)(sequence + 1);
                        }
                    }
                }
                service.Save();
            }
        }

        protected override void InitiazeSequence(mstr_org_levelInput input)
        {
            var seq = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (seq.Any()) { input.LevelSequence = seq.Max(o => o.LevelSequence); input.LevelSequence++; } else { input.LevelSequence = 1; }
        }
    }
}

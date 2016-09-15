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
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Core;
using System.Transactions;
using WebMatrix.WebData;
using System.Globalization;
using System;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class ProjEstmParamsController : Cruder<tbl_org_estm_parameters, tbl_org_estm_parametersInput>
    {
        //
        // GET: /ProjEstmParams/
        public ProjEstmParamsController(ICrudService<tbl_org_estm_parameters> service, IMapper<tbl_org_estm_parameters, tbl_org_estm_parametersInput> v, IWorkflowService wf)
            : base(service, v, wf, "PLEST")
        
        {
            functionID = "PLEST";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_org_estm_parameters o)
        {
            return (o.Name == null) ? "" : o.Name;
        }

        protected override bool checkForDuplication(tbl_org_estm_parametersInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        protected override bool checkForDuplicateEdit(tbl_org_estm_parametersInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.Name.Trim().Equals(input.Name.Trim()));
            if (entity.Any()) return true;
            else return false;
        }

        public ActionResult EditParam(int? id, string callerID)
        {
            var ctx = (Db)service.getRepo().getDBContext();
            tbl_org_estm_parametersInput input;
            ViewBag.callerID = callerID; 
            if (id == null)
            {
                input = new tbl_org_estm_parametersInput()
                {
                    ClientID = ((PAIdentity)User.Identity).clientID,
                    Simple = 1,
                    Medium = 1,
                    Complex = 1
                };
                return View("Create", input);
            }
            else
            {
                var entity = service.Get(id.GetValueOrDefault());
                input = editMapper.MapToInput(entity);
                return View("Edit", input);
            }
        }

        public virtual ActionResult getListItemsCustom(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<tbl_org_estm_parameters> list = new List<tbl_org_estm_parameters>();
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
                        list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
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
                return PartialView("listComboCustom", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }


    }
}

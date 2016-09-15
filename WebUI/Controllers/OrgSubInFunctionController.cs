using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Omu.AwesomeMvc;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class OrgSubInFunctionController : Cruder<mstr_org_sub_in_function, mstr_org_sub_in_functionInput>
    {
        public OrgSubInFunctionController(ICrudService<mstr_org_sub_in_function> service, IMapper<mstr_org_sub_in_function, mstr_org_sub_in_functionInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFORGCFG")
        {
            functionID = "DFORGCFG";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        public ActionResult linkSubFunction(int id)
        {
            try
            {
                var ctx = (Db)service.getRepo().getDBContext();
                var entity = ctx.mstr_org_function.Where(p => p.ID == id);
                if (!entity.Any()) throw new PAException("Function doesn't exist anymore");

                var subFunctions = service.Where(o => o.mstr_Org_FunctionID == id);
                string excludeIDs = "0";

                if (subFunctions.Any())
                {
                    foreach (var p in subFunctions)
                    {
                        excludeIDs = excludeIDs + "," + p.mstr_Org_Sub_FunctionID.ToString();
                    }
                }
                ViewBag.excludeIDs = excludeIDs;

                tbl_org_config_details input = new tbl_org_config_details();
                input.ID = id;
                input.Name = entity.FirstOrDefault().LongName;

                return View("Create", input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult linkSubFunction(tbl_org_config_details input)
        {
            try
            {
                mstr_org_sub_in_function entity;
                int id;

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("Create", input);
                }
                if (input.selectedOptions.Any())
                {
                    foreach (var opt in input.selectedOptions)
                    {
                        entity = new mstr_org_sub_in_function()
                        {
                            mstr_Org_Sub_FunctionID = opt,
                            mstr_Org_FunctionID = input.ID,
                            ClientID = ((PAIdentity)User.Identity).clientID
                        };
                        id = service.Create(entity);
                    }
                }
                var ctx = (Db)service.getRepo().getDBContext();
                var func = ctx.mstr_org_function.Include("mstr_org_sub_in_function").Where(o => o.ID == input.ID).SingleOrDefault();
                if (func.mstr_org_sub_in_function.Any())
                {
                    foreach (var sb in func.mstr_org_sub_in_function)
                    {
                        ctx.Entry(sb).Reference(l => l.mstr_org_sub_function).Load();
                    }
                }
                return View(RowViewName, func);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

    }
}

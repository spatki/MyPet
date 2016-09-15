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
using ProcessAccelerator.WebUI.BAL.AccessControl;
using System.Transactions;
using WebMatrix.WebData;

namespace ProcessAccelerator.WebUI.Controllers
{
    /// <summary>
    /// generic crud controller for entities where there is difference between the edit and create view
    /// </summary>
    /// <typeparam name="TEntity"> the entity</typeparam>
    /// <typeparam name="TCreateInput">create viewmodel</typeparam>
    /// <typeparam name="TEditInput">edit viewmodel</typeparam>

    [Authorize]
    [InitializeSimpleMembership]
    public abstract class Crudere<TEntity, TCreateInput, TEditInput> : BaseController
        where TCreateInput : Input, new()
        where TEditInput : Input, new()
        where TEntity : Entity, new()
    {
        protected readonly ICrudService<TEntity> service;
        protected readonly IMapper<TEntity, TCreateInput> createMapper;
        protected readonly IMapper<TEntity, TEditInput> editMapper;
        protected string functionID;
        protected vw_org_role_access accessInfo;
        protected IWorkflowService wrkflw;

        protected virtual string EditView
        {
            get { return "Edit"; }
        }

        public Crudere(ICrudService<TEntity> service, IMapper<TEntity, TCreateInput> createMapper, IMapper<TEntity, TEditInput> editMapper,IWorkflowService workflow, string FunctionID) : base()
        {
            this.service = service;
            this.createMapper = createMapper;
            this.editMapper = editMapper;
            functionID = FunctionID;
            wrkflw = workflow;
            ViewBag.FunctionID = functionID;
        }

        public virtual ActionResult Index()
        {
            if (!CheckAccess("")) 
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            ViewBag.FunctionID = functionID;
            return View();
        }

        public virtual ActionResult Create()
        {
            try
            {
                ViewBag.FunctionID = functionID;
                if (CheckAccess("Create"))
                {
                    // Check for any workflow 
                    var user = ((PAIdentity)User.Identity);
                    var wf = wrkflw.getFunctionStatus(functionID, null, null, user.IsAdmin(), null, null, user.clientID.GetValueOrDefault());
                    if (wf.Any())
                    {
                        ViewBag.WF = true;
                        ViewBag.workflow = wf;
                    }
                    var input = createMapper.MapToInput(new TEntity());
                    InitiazeSequence(input);
                    return View(input);
                }
                else
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public virtual ActionResult Create(TCreateInput input)
        {
            try
            {
                ViewBag.FunctionID = functionID;
                if (!CheckAccess("Create")) 
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, null, null, user.IsAdmin(), null, null, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View("Create",input);
                }
                if (checkForDuplication(input))
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", "Duplicate Entry Found");
                    return View("Create", input);
                }
                ReSequenceBeforeCreate(input);
                var entity = createMapper.MapToEntity(input, new TEntity());
                entity.ClientID = ((PAIdentity)User.Identity).clientID;
                var id = service.Create(entity);
                var e = service.Get(id);
                LoadDependencies(e);
                if (input.followWF.GetValueOrDefault())
                {
                    wrkflw.saveFlow(e.ID, WebSecurity.CurrentUserId, WebSecurity.CurrentUserId, functionID, ((PAIdentity)User.Identity).clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(), (bool?)true,"");
                }
                // Get changed workflow information
                wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(e), e.ID, user.clientID.GetValueOrDefault());
                ViewBag.WF = true;
                ViewBag.workflow = wf;
                //return Json(new { Content = this.RenderView(RowViewName, new[] { e }) });
                return ProcessView(new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [OutputCache(Location = OutputCacheLocation.None)]//for ie
        public virtual ActionResult Edit(int id)
        {
            try
            {
                ViewBag.FunctionID = functionID;
                //var entity = new TEntity();
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                LoadDependencies(entity);
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID,WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(entity), entity.ID, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }
                ViewBag.Mode = "Edit";
                return View("Edit", editMapper.MapToInput(entity));
            }
            catch (PAException ex)
            {
                Response.StatusCode = 403;
                ViewBag.ErrorMessage = ex.Message;
                return View("ListItems/showError");
            }
        }

        [HttpPost]
        public virtual ActionResult Edit(TEditInput input)
        {
            try
            {
                ViewBag.FunctionID = functionID;
                if (!CheckAccess("Edit")) 
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                var entity = service.Get(input.ID);
                // Check for any workflow 
                var user = ((PAIdentity)User.Identity);
                var wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(entity), entity.ID, user.clientID.GetValueOrDefault());
                if (wf.Any())
                {
                    ViewBag.WF = true;
                    ViewBag.workflow = wf;
                }

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 412;
                    return View(input);
                }
                if (checkForDuplicateEdit(input))
                {
                    Response.StatusCode = 412;
                    ModelState.AddModelError("", "Duplicate Entry Found");
                    return View(input);
                }
                ReSequenceBeforeEdit(input);
                var e = editMapper.MapToEntity(input, entity);
                using (TransactionScope scope = new TransactionScope())
                {
                    e.ClientID = ((PAIdentity)User.Identity).clientID;
                    service.Save();
                    LoadDependencies(e);
                    if (input.followWF.GetValueOrDefault())
                    {
                        wrkflw.saveFlow(e.ID, WebSecurity.CurrentUserId, input.workflowUser, functionID, user.clientID.GetValueOrDefault(), input.statusWF.GetValueOrDefault(), input.workflow, "");
                        wf = wrkflw.getFunctionStatus(functionID, WebSecurity.CurrentUserId, user.role, user.IsAdmin(), status(e), e.ID, user.clientID.GetValueOrDefault());
                        ViewBag.WF = true;
                        ViewBag.workflow = wf;
                    }
                    // Reload workflow based on changes
                    scope.Complete();
                }
                //return Json(new { input.ID, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
                ViewBag.Mode = "Edit";
                return ProcessView(new[] { e });
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public virtual ActionResult ProcessView(IEnumerable<TEntity> entity)
        {
            ViewBag.FunctionID = functionID;
            return View(RowViewName, entity);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            try
            {
                ViewBag.FunctionID = functionID;
                if (!CheckAccess("Delete"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                service.Delete(id);
                return Json(new { Id = id, Type = typeof(TEntity).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Restore(int id)
        {
            service.Restore(id);

            return Json(new { Id = id, Content = this.RenderView(RowViewName, new[] { service.Get(id) }), Type = typeof(TEntity).Name.ToLower() }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetItems()
        {
            ViewBag.FunctionID = functionID;
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        public virtual ActionResult ReloadItems()
        {
            ViewBag.FunctionID = functionID;
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        public virtual ActionResult getListItemsJSON()
        {
            try
            {
                var list = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
                var returnList = from node in list
                                 select new
                                 {
                                     index = node.ID,
                                     name = listDisplayName(node)
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getSelectedListItemsJSON(string IncludeIDs)
        {
            try
            {
                IEnumerable<int> include;
                include = new[] { 0 };

                if (IncludeIDs != null & IncludeIDs != "")
                {
                    include = IncludeIDs.Split(',').Select(str => int.Parse(str));
                }

                var list = service.Where(rec => include.Contains(rec.ID) &&  rec.ClientID == ((PAIdentity)User.Identity).clientID);
                var returnList = from node in list
                                 select new
                                 {
                                     index = node.ID,
                                     name = listDisplayName(node)
                                 };
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getListItems(int selectedItem, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<TEntity> list = new List<TEntity>();
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
                return PartialView("ListItems/listCombo", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public virtual ActionResult getMultiSelectList(List<string> selectedItems, string controlName, string excludeIds, string selectIds, string reload)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<TEntity> list = new List<TEntity>();
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

        public virtual ActionResult getListBoxItems(List<int> selectedItems, string controlName, string excludeIds, string selectIds, string reload, int? source)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                IEnumerable<TEntity> list = new List<TEntity>();
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

                var returnList = list.ToList().Select(node => new SelectListItem
                {
                    Value = node.ID.ToString(),
                    Text = listDisplayName(node)
                });

                ViewBag.selectedItems = selectedItems;
                ViewBag.itemName = controlName;
                ViewBag.reload = reload;
                return PartialView("ListItems/listBox", returnList.AsEnumerable());
            }

            catch (PAException e)
            {
                e.Raize();
            }
            return null;
        }

        public ActionResult DeleteInWF(int id, int workflowUser, int status, string message, bool? workflow, int? statusType)
        {
            var input = service.Get(id);
            ViewBag.Message = message;
            return View(input);
        }

        [HttpPost]
        public virtual ActionResult DeleteInWF(int id)
        {
            try
            {
                if (!CheckAccess("Delete"))
                {
                    Response.StatusCode = 403;
                    return View("Unauthorized");
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    service.Delete(id);
                    wrkflw.DeleteRecord(functionID, id);
                    scope.Complete();
                }
                return View(new TEntity() { ID = id });
            }
            catch (PAException e)
            {
                Response.StatusCode = 412;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        protected abstract string RowViewName { get; }

        protected virtual string listDisplayName(TEntity o) { return o.ID.ToString(); }

        protected virtual int status(TEntity o) { return 0; }

        protected virtual bool checkForDuplication(TCreateInput input) { return false; }

        protected virtual bool checkForDuplicateEdit(TEditInput input) { return false; }

        protected virtual IOrderedEnumerable<TEntity> orderList(IEnumerable<TEntity> list) { return list.OrderBy(o => o.ID); }

        protected virtual void ReSequenceBeforeCreate(TCreateInput input) { }

        protected virtual void ReSequenceBeforeEdit(TEditInput input) { }

        protected virtual bool CheckAccess(string restrictedFunction)
        {
            var user = ((PAIdentity)User.Identity);

            if (functionID != "" & functionID != "Account")
            {
                if (user.IsAdmin())     // Give Administrative Access
                {
                    accessInfo = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Full" && o.FunctionID == functionID).SingleOrDefault();
                    if (accessInfo != null)
                    {
                        if (accessInfo.accessRange == 1)
                        {
                            // Has restricted access
                            switch (restrictedFunction)
                            {
                                case "":
                                    return true;
                                case "Create":
                                    if (accessInfo.addAccess == true) return true;
                                    else return false;
                                case "Edit":
                                    if (accessInfo.updateAccess == true) return true;
                                    else return false;
                                case "Delete":
                                    if (accessInfo.deleteAccess == true) return true;
                                    else return false;
                                default:
                                    return false;
                            }
                        } else return true;
                    }
                    else return false;
                }
                else
                {
                    if (user.IsGuest())     // Give Guest Access
                    {
                        var ctx = (Db) service.getRepo().getDBContext();
                        var access = ctx.webpages_Roles.Where(k => k.RoleName == "Guest");
                        if (access.Any())
                        {
                            accessInfo = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == "Sys" && o.Sys_Role == access.FirstOrDefault().ID && o.FunctionID == functionID).SingleOrDefault();
                            if (accessInfo != null)
                            {
                                // Has restricted access
                                switch (restrictedFunction)
                                {
                                    case "":
                                        return true;
                                    case "Create":
                                        if (accessInfo.addAccess == true) return true;
                                        else return false;
                                    case "Edit":
                                        if (accessInfo.updateAccess == true) return true;
                                        else return false;
                                    case "Delete":
                                        if (accessInfo.deleteAccess == true) return true;
                                        else return false;
                                    default:
                                        return false;
                                }
                            } 
                            else return false;
                        }
                        else
                        {
                            accessInfo = null;
                            return false;
                        }
                    }
                    else
                    {
                        var mode = user.mode();
                        var role = user.role;
                        accessInfo = PACacheManager.MenuOptions.Where(o => o.ClientID == user.clientID && o.AccessType == mode && (o.Sys_Role == null ? o.Org_Role : o.Sys_Role) == role && o.FunctionID == functionID).SingleOrDefault();
                        if (accessInfo != null)
                        {
                            // Has restricted access
                            switch (restrictedFunction)
                            {
                                case "":
                                    return true;
                                case "Create":
                                    if (accessInfo.addAccess == true) return true;
                                    else return false;
                                case "Edit":
                                    if (accessInfo.updateAccess == true) return true;
                                    else return false;
                                case "Delete":
                                    if (accessInfo.deleteAccess == true) return true;
                                    else return false;
                                default:
                                    return false;
                            }
                        } 
                        else return false;
                    }
                }
            }
            return true; // Default to full access
        }

        public string GetName(int id)
        {
            var entity = service.Get(id);
            if (entity != null)
            {
                return listDisplayName(entity);
            }
            else
            {
                return "";
            }
        }

        public string GetNames(List<string> id)
        {
            if (id != null)
            {
                List<int> selectIDs = new List<int>();
                foreach (var selID in id)
                {
                    selectIDs.Add(int.Parse(selID));
                }

                string names = "";

                var entity = service.Where(o => selectIDs.Contains(o.ID));
                if (entity.Any())
                {
                    foreach (var i in entity)
                    {
                        names = names + (names == "" ? "" : ", ") + listDisplayName(i);
                    }
                    return names;
                }
            }
            return "";
        }

        protected virtual void InitiazeSequence(TCreateInput input) { }

        protected virtual void LoadDependencies(TEntity e) { }

        protected virtual void setStatus(TEntity e, int status) { }

        public virtual ActionResult ProceedInWF(int id, int workflowUser, int status, string message, bool? workflow, int? statusType)
        {
            var entity = service.Get(id);
            var ctx = (Db)service.getRepo().getDBContext();
            var LC_StatusName = ctx.mstr_process_lc_status.Where(o => o.ID == status);
            tbl_workflow_state_historyInput input = new tbl_workflow_state_historyInput()
            {
                FunctionID = functionID,
                CreatedBy = StatusUserName(entity),
                StatusName = (LC_StatusName != null && LC_StatusName.Any() ? LC_StatusName.First().Status : ""),
                ReviewItemName = ReviewItemName(entity),
                UserName = ((PAIdentity)User.Identity).friendlyName,
                RefID = id,
                StatusDate = System.DateTime.Now.Date,
                Status = status,
                StatusType = statusType.GetValueOrDefault(),
                UserID = WebSecurity.CurrentUserId,
                ClientID = ((PAIdentity)User.Identity).clientID,
                Message = message,
                workflow = workflow,
                UpdateID = UpdateChangesTo()
            };

            return View("WorkFlowReview",input);
        }

        [HttpPost]
        public virtual ActionResult ProceedInWF(tbl_workflow_state_historyInput input)
        {
            Mapper<tbl_workflow_state_history, tbl_workflow_state_historyInput> histMapper = new Mapper<tbl_workflow_state_history, tbl_workflow_state_historyInput>();
            var ctx = (Db)service.getRepo().getDBContext();

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 412;
                return View("WorkFlowReview", input);
            }

            if (input.RefID == 0)
            {
                input.RefID = input.ID;
            }
            var entity = service.Get(input.RefID);

            using (TransactionScope scope = new TransactionScope())
            {
                // Save Workflow comments
                wrkflw.saveFlow(entity.ID, StatusUserID(entity), input.statusWF.GetValueOrDefault(), functionID, input.ClientID.GetValueOrDefault(), input.Status, input.workflow, input.ReviewComments);
                setStatus(entity, input.Status);
                service.Save();
                scope.Complete();
            }
            LoadDependencies(entity);
            return ProcessView(new[] { entity });
        }

        protected virtual string ReviewItemName(TEntity e) { return ""; }   // Name of the entity being reviewed. This is used by the workflow

        protected virtual string StatusUserName(TEntity e) 
        {
            var ctx = (Db)service.getRepo().getDBContext();
            var userID = StatusUserID(e);
            var userName = ctx.UserProfile.Where(o => o.ID == userID);
            if (userName == null || !userName.Any()) return "";
            else return userName.First().DisplayName;
        }        // Name of the user who has worked last on this entity, this is used as information for the next user in the workflow

        protected virtual int StatusUserID(TEntity e) { return wrkflw.StatusUserID(functionID, e.ID); }    // The userid of the last user who has worked on this entity

        protected virtual string UpdateChangesTo() { return "containerDetails"; }
    
        public virtual ActionResult ShowHistory(int id)
        {
            return PartialView(wrkflw.GetHistory(functionID, id));
        }
    }
}

using System.Web.Mvc;
using System.Linq;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Data;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using System.Collections.Generic;
using System.Transactions;
using WebMatrix.WebData;
using System.IO;
using System.Web;
using System;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class DocMgrController : Cruder<tbl_docmgr_document, tbl_docmgr_documentInput>
    {
        protected string[] allowedExtensions = new[] { ".doc", ".xlsx", ".doc", ".docx", ".txt", ".jpeg", ".gif", ".tiff", ".pdf" };
        protected string fileLocationPath = "~/App_Data/uploads";
        protected string imageLocation = "~/Content/images";
        //
        // GET: /DocMgr/

        public DocMgrController(ICrudService<tbl_docmgr_document> service, IMapper<tbl_docmgr_document, tbl_docmgr_documentInput> v, IWorkflowService wf)
            : base(service, v, wf, "DF")
        {
            functionID = "DF";
        }

        public ActionResult uploadProfilePicture()
        {
            tbl_docmgr_documentInput input = new tbl_docmgr_documentInput();
            return View(input);
        }

        [HttpPost]
        public ActionResult uploadProfilePicture(tbl_docmgr_documentInput input)
        {
            try {
                if (!ModelState.IsValid)    
                {
                    Response.StatusCode = 500;
                    return View("uploadProfilePicture", input);
                }
                if (input.fileName.ContentLength > 0)
                {
                    string[] imageExtensions = new[] { ".jpg", ".jpeg", ".gif", ".tiff" };
                    // Upload and save the file first
                    // var thumbGUID = Guid.NewGuid(); --> use this soon
                    var fileName = Path.GetFileName(input.fileName.FileName);
                    
                    var extension = Path.GetExtension(fileName);

                    if (!imageExtensions.Contains(extension))
                    {
                        Response.StatusCode = 500;
                        ModelState.AddModelError("", "Invalid extension. Valid extensions (.jpg, .jpeg, .gif, .tiff)");
                        return View("uploadProfilePicture", input);
                    }

                    var path = Server.MapPath(imageLocation);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    fileName = User.Identity.Name + "_pic" + extension;

                    input.fileName.SaveAs(path + "/" + fileName);
                    return Content("<img id='usrProfilePic' src='" + Url.Content(imageLocation + "//" + fileName) + "'  title='User Pic' width='50' height='50' alt='User Pic'/>");
                }
                else
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("", "File is invalid or non-existant");
                    return PartialView("Create", input);
                }
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult uploadCompanyLogo()
        {
            tbl_docmgr_documentInput input = new tbl_docmgr_documentInput();
            return View(input);
        }

        [HttpPost]
        public ActionResult uploadCompanyLogo(tbl_docmgr_documentInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("uploadProfilePicture", input);
                }
                if (input.fileName.ContentLength > 0)
                {
                    string[] imageExtensions = new[] { ".jpg", ".jpeg", ".gif", ".tiff", ".png" };
                    // Upload and save the file first
                    // var thumbGUID = Guid.NewGuid(); --> use this soon
                    var fileName = Path.GetFileName(input.fileName.FileName);

                    var extension = Path.GetExtension(fileName);

                    if (!imageExtensions.Contains(extension))
                    {
                        Response.StatusCode = 500;
                        ModelState.AddModelError("", "Invalid extension. Valid extensions (.jpg, .jpeg, .gif, .tiff)");
                        return View("uploadProfilePicture", input);
                    }

                    var path = Server.MapPath(imageLocation);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    fileName = "Client_" + ((PAIdentity)User.Identity).clientID.ToString() + extension;

                    // Save this filename for the current client
                    var ctx = (Db) service.getRepo().getDBContext();
                    var client = ctx.mstr_client.Where(o => o.ID == ((PAIdentity)User.Identity).clientID).SingleOrDefault();
                    client.Logo = fileName;
                    ctx.SaveChanges();

                    input.fileName.SaveAs(path + "/" + fileName);

                    return Content("<img id='companyLogo' src='" + Url.Content(imageLocation + "//" + fileName) + "'  title='Company Logo' alt='Company Logo'/>");
                }
                else
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("", "File is invalid or non-existant");
                    return PartialView("Edit", input);
                }
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Create(tbl_docmgr_documentInput input)
        {
            try
            {
                if (!ModelState.IsValid)    
                {
                    Response.StatusCode = 500;
                    return View("Create", input);
                }
                if (input.fileName.ContentLength > 0)
                {
                    // Upload and save the file first
                    // var thumbGUID = Guid.NewGuid(); --> use this soon
                    var fileName = Path.GetFileName(input.fileName.FileName);
                    
                    var extension = Path.GetExtension(fileName);

                    if (!allowedExtensions.Contains(extension))
                    {
                        Response.StatusCode = 500;
                        ModelState.AddModelError("", "Invalid extension. Valid extensions (.xls, .xlsx, .doc, .docx, .txt, .jpeg, .gif, .pdf)");
                        return View("Create", input);
                    }

                    var path = Server.MapPath(fileLocationPath);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    fileName = Guid.NewGuid().ToString() + extension;

                    input.fileName.SaveAs(path + "\\" + fileName);
                    input.FileLocation = Path.Combine(path,fileName);
                    input.ClientID = ((PAIdentity)User.Identity).clientID;
                    input.UploadedBy = WebSecurity.CurrentUserId;
                    input.UploadDate = System.DateTime.Now.Date;
                    input.Version = 1;

                    var entity = createMapper.MapToEntity(input, new tbl_docmgr_document());
                    var id = service.Create(entity);
                    var e = service.Get(id);

                    // Save this document as a process document by default : Review and implement later
                    var ctx = (Db) service.getRepo().getDBContext();
                    ctx.tbl_process_document.Add(new tbl_process_document() {
                        ClientID = e.ClientID,
                        tbl_DocMgr_DocumentID = e.ID
                    });
                    ctx.SaveChanges();

                    return PartialView(RowViewName, new[] { e });
                }
                else
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("", "File is invalid or non-existant");
                    return PartialView("Create", input);
                }
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }
/*
        public tbl_docmgr_documentInput saveDocument(tbl_docmgr_documentInput input)
        {
            try
            {
                if (input.fileName.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(input.fileName.FileName);
                    
                    var extension = Path.GetExtension(fileName);

                    if (!allowedExtensions.Contains(extension))
                    {
                        Response.StatusCode = 500;
                        ModelState.AddModelError("", "Invalid extension. Valid extensions (.xls, .xlsx, .doc, .docx, .txt, .jpeg, .gif, .pdf)");
                        return input;
                    }

                    var path = Server.MapPath(fileLocationPath);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    fileName = Guid.NewGuid().ToString() + extension;

                    input.fileName.SaveAs(path + "/" + fileName);
                    input.FileLocation = Path.Combine(path,fileName);
                    input.ClientID = ((PAIdentity)User.Identity).clientID;
                    input.UploadedBy = WebSecurity.CurrentUserId;
                    input.UploadDate = System.DateTime.Now.Date;
                    input.Version = 1;

                    var entity = createMapper.MapToEntity(input, new tbl_docmgr_document());
                    var id = service.Create(entity);
                    var e = service.Get(id);
                    return input;
                }
                else
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("", "File is invalid or non-existant");
                    return input;
                }
            }
            catch (PAException ex)
            {
                Response.StatusCode = 500;
                ModelState.AddModelError("", "System Exception");
                return input;
            }

        }
        */
        public virtual ActionResult CreateVersion(int id)
        {
            try
            {
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                service.getRepo().getDBContext().Entry(entity).Collection(o => o.tbl_docmgr_version).Load();
                var input = editMapper.MapToInput(entity);

                input.tbl_docmgr_version = new List<tbl_docmgr_version>();
                if (entity.tbl_docmgr_version.Any())
                {
                    foreach (var v in entity.tbl_docmgr_version)
                    {
                        input.tbl_docmgr_version.Add(v);
                    }
                }
                input.tbl_docmgr_version = new List<tbl_docmgr_version>();
                input.tbl_docmgr_version.Add(new tbl_docmgr_version()
                {
                    ID = (entity.tbl_docmgr_version.LastOrDefault() == null ? 1 : entity.tbl_docmgr_version.LastOrDefault().ID + 1),
                    ClientID = entity.ClientID,
                    version = entity.Version,
                    CreateDate = System.DateTime.Now.Date,
                    CreateUser = WebSecurity.CurrentUserId,
                });
                return View(input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public virtual ActionResult CreateVersion(tbl_docmgr_documentInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("CreateVersion", input);
                }
                if (input.fileName.ContentLength > 0)
                {
                    // Upload and save the file first
                    // var thumbGUID = Guid.NewGuid(); --> use this soon
                    var fileName = Path.GetFileName(input.fileName.FileName);

                    var extension = Path.GetExtension(fileName);

                    if (!allowedExtensions.Contains(extension))
                    {
                        Response.StatusCode = 500;
                        ModelState.AddModelError("", "Invalid extension. Valid extensions (.xls, .xlsx, .doc, .docx, .txt, .jpeg, .gif)");
                        return View("CreateVersion", input);
                    }

                    // Save the current version
                    var entity = service.Get(input.ID);
                    if (entity == null) throw new PAException("this entity doesn't exist anymore");
                    using (TransactionScope scope = new TransactionScope())
                    {
                        entity.tbl_docmgr_version = new List<tbl_docmgr_version>();

                        entity.tbl_docmgr_version.Add(new tbl_docmgr_version()
                        {
                            ID = input.tbl_docmgr_version.LastOrDefault().ID,
                            tbl_DocMgr_DocumentID = entity.ID,
                            ClientID = entity.ClientID,
                            version = input.tbl_docmgr_version.LastOrDefault().version,
                            CreateUser = input.tbl_docmgr_version.LastOrDefault().CreateUser,
                            CreateDate = input.tbl_docmgr_version.LastOrDefault().CreateDate,
                            Comments = input.tbl_docmgr_version.LastOrDefault().Comments,
                            FileLocation = entity.FileLocation,
                            ReadWriteMode = entity.ReadWriteMode,
                            UploadKey = entity.UploadKey,
                            DownloadKey = entity.DownloadKey,
                            ViewKey = entity.ViewKey,
                            StorageMode = entity.StorageMode
                        });

                        // Save the latest version
                        entity.Version = entity.Version + 1;

                        var path = Server.MapPath(fileLocationPath);
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        fileName = Guid.NewGuid().ToString() + extension;

                        input.fileName.SaveAs(path + "/" + fileName);
                        entity.FileLocation = Path.Combine(path, fileName);
                        entity.ClientID = entity.ClientID;
                        entity.UploadedBy = WebSecurity.CurrentUserId;
                        entity.UploadDate = System.DateTime.Now.Date;

                        service.Save();
                        scope.Complete();
                    }
                    return PartialView(RowViewName, new[] { entity });
                }
                else
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("", "File is invalid or non-existant");
                    return PartialView("Create", input);
                }
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }

        public virtual ActionResult ViewVersions(int id)
        {
            try
            {
                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");
                service.getRepo().getDBContext().Entry(entity).Collection(o => o.tbl_docmgr_version).Load();

                return View(entity);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }


        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var entity = service.Get(id);
                    service.getRepo().getDBContext().Entry(entity).Collection(o => o.tbl_docmgr_version).Load();

                    // Files uploaded on the file server
                    if (entity.tbl_docmgr_version.Any())
                    {
                        foreach (var v in entity.tbl_docmgr_version)
                        {
                            // Delete versions
                            if (System.IO.File.Exists(v.FileLocation))
                            {
                                System.IO.File.Delete(v.FileLocation);
                            }        
                        }
                    }
                    // Delete parent
                    if (System.IO.File.Exists(entity.FileLocation))
                    {
                        System.IO.File.Delete(entity.FileLocation);
                    }
                    service.getRepo().executeStoredCommand("delete from tbl_docmgr_version where tbl_DocMgr_DocumentID = " + entity.ID);
                    service.getRepo().executeStoredCommand("delete from tbl_process_document where tbl_DocMgr_DocumentID = " + entity.ID); // Remove from process document by default
                    service.getRepo().executeStoredCommand("delete from tbl_docmgr_document where ID = " + entity.ID);
                    scope.Complete();
                    //
                }
                return Json(new { Id = id, Type = typeof(tbl_docmgr_document).Name.ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (PAException e)
            {
                Response.StatusCode = 500;
                return Json(new { Content = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult ViewDocument(int id, int? version)
        {
            FilePathResult resultFile;
            try
            {
                string fileName = "";
                string fileExtension = "";

                var entity = service.Get(id);
                if (entity == null) throw new PAException("this entity doesn't exist anymore");

                if (version == null || version == 0)
                {
                    fileName = entity.FileLocation;
                }
                else
                {
                    service.getRepo().getDBContext().Entry(entity).Collection(o => o.tbl_docmgr_version).Load();
                    var entityEntry = entity.tbl_docmgr_version.Where(o => o.ID == (int)version).Single();
                    if (entityEntry == null) return new FilePathResult(Server.MapPath(fileLocationPath) + "/DNF.txt", "text/plain");
                    fileName = entityEntry.FileLocation;
                }
                fileExtension = Path.GetExtension(fileName);
                string contentType = "";

                switch (fileExtension)
                {
                    case ".doc":
                        contentType = "application/msword";
                        break;
                    case ".docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case ".xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case ".xls":
                        contentType = "application/vnd.ms-excel";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".tiff":
                        contentType = "image/tiff";
                        break;
                    case ".txt":
                        contentType = "text/plain";
                        break;
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                }
                if (System.IO.File.Exists(fileName))
                    resultFile =  new FilePathResult(fileName, contentType);
                else
                    resultFile = new FilePathResult(Server.MapPath(fileLocationPath) + "/DNF.txt", "text/plain");
            }
            catch (PAException e)
            {
                resultFile = new FilePathResult(Server.MapPath(fileLocationPath) + "/DNF.txt", "text/plain");
            }
            return resultFile;
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_docmgr_document o)
        {
            return (o.Name == null) ? "" : o.Name;
        }

        public override ActionResult getListBoxItems(List<int> selectedItems, string controlName, string excludeIds, string selectIds, string reload, int? source)
        {
            try
            {
                IEnumerable<int> exclude;
                IEnumerable<int> include;
                exclude = new[] { 0 };
                include = new[] { 0 };
                IEnumerable<SelectListItem> returnList;


                if (source != null && source == 1)
                {
                    IEnumerable<tbl_process_document> list = new List<tbl_process_document>();
                    var ctx = (Db)service.getRepo().getDBContext();

                    if (excludeIds != null & excludeIds != "")
                    {
                        exclude = excludeIds.Split(',').Select(str => int.Parse(str));
                        list = ctx.tbl_process_document.Include("tbl_docmgr_document").Where(rec => !exclude.Contains((int)rec.tbl_DocMgr_DocumentID) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
                    }
                    else
                    {
                        if (selectIds != null & selectIds != "")
                        {
                            include = selectIds.Split(',').Select(str => int.Parse(str));
                            list = ctx.tbl_process_document.Include("tbl_docmgr_document").Where(rec => include.Contains((int)rec.tbl_DocMgr_DocumentID) && rec.ClientID == ((PAIdentity)User.Identity).clientID);
                        }
                        else
                        {
                            list = ctx.tbl_process_document.Include("tbl_docmgr_document").Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID);
                        }
                    }
                    returnList = list.ToList().Select(node => new SelectListItem
                    {
                        Value = node.tbl_DocMgr_DocumentID.ToString(),
                        Text = node.tbl_docmgr_document.Name
                    });

                }
                else
                {
                    IEnumerable<tbl_docmgr_document> list = new List<tbl_docmgr_document>();
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
                    returnList = list.ToList().Select(node => new SelectListItem
                    {
                        Value = node.ID.ToString(),
                        Text = node.Name
                    });
                }

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

        public override ActionResult GetItems()
        {
            if (!CheckAccess(""))
            {
                Response.StatusCode = 403;
                return View("Unauthorized");
            }
            var ctx = (Db)service.getRepo().getDBContext();
            var prsDocs = ctx.tbl_process_document.Include("tbl_docmgr_document").Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID).Select(d => d.tbl_docmgr_document);

            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(prsDocs);
        }


    }
}

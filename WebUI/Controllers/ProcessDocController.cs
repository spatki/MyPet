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
    public class ProcessDocController : Cruder<tbl_process_document,tbl_process_documentInput>
    {
        //
        // GET: /ProccessDoc/
        public ProcessDocController(ICrudService<tbl_process_document> service, IMapper<tbl_process_document, tbl_process_documentInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRS")
        {
            functionID = "DFPRS";
        }

        [HttpPost]
        public override ActionResult Create(tbl_process_documentInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    return View("Create", input);
                }
                string errorMess = "";

                using (TransactionScope scope = new TransactionScope())
                {
                    if (input.tbl_docmgr_document != null)
                    {
                        var docMgrInput = new tbl_docmgr_documentInput()
                        {
                            ClientID = ((PAIdentity)User.Identity).clientID,
                            Name = input.tbl_docmgr_document.Name,
                            fileName = input.tbl_docmgr_document.fileName,
                            FileLocation = input.tbl_docmgr_document.FileLocation,
                            Version = input.tbl_docmgr_document.Version,
                            ReadWriteMode = input.tbl_docmgr_document.ReadWriteMode,
                            UploadKey = input.tbl_docmgr_document.UploadKey,
                            DownloadKey = input.tbl_docmgr_document.DownloadKey,
                            ViewKey = input.tbl_docmgr_document.ViewKey,
                            StorageMode = input.tbl_docmgr_document.StorageMode,
                            Classification = input.tbl_docmgr_document.Classification,
                            UploadedBy = WebSecurity.CurrentUserId,
                            UploadDate = System.DateTime.Now.Date
                        };
//                        docMgrInput = (tbl_docmgr_documentInput) RedirectToMethod("saveDocument","DocMgr",new { input = docMgrInput });
                        var entity = createMapper.MapToEntity(input, new tbl_process_document());
                        var ctx = (Db)service.getRepo().getDBContext();

                        var doc_mgr = ctx.tbl_docmgr_document.Create();
                        doc_mgr.ClientID = ((PAIdentity)User.Identity).clientID;
                        doc_mgr.Name = docMgrInput.Name;
                        doc_mgr.FileLocation = docMgrInput.FileLocation;
                        doc_mgr.Version = docMgrInput.Version;
                        doc_mgr.ReadWriteMode = docMgrInput.ReadWriteMode;
                        doc_mgr.UploadKey = docMgrInput.UploadKey;
                        doc_mgr.DownloadKey = docMgrInput.DownloadKey;
                        doc_mgr.ViewKey = docMgrInput.ViewKey;
                        doc_mgr.StorageMode = docMgrInput.StorageMode;
                        doc_mgr.Classification = docMgrInput.Classification;
                        doc_mgr.UploadedBy = WebSecurity.CurrentUserId;
                        doc_mgr.UploadDate = System.DateTime.Now.Date;
                        ctx.SaveChanges();
                        entity.tbl_DocMgr_DocumentID = doc_mgr.ID;

                        var id = service.Create(entity);
                        var e = service.Get(id);
                        return PartialView(RowViewName, new[] { e });
                    }
                    scope.Complete();
                }
                return View(input);
            }
            catch (PAException ex)
            {
                return Content(ex.Message);
            }
        }


        public override ActionResult GetItems()
        {
            var list = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);

            if (list.Any())
            {
                foreach (var d in list)
                {
                    service.getRepo().getDBContext().Entry(d).Reference(o => o.tbl_docmgr_document).Load();
                }
            }
            //by default ordering by id
            //list = list.OrderByDescending(o => o.ID);

            return PartialView(list);
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(tbl_process_document o)
        {
            service.getRepo().getDBContext().Entry(o).Reference(d => d.tbl_docmgr_document).Load();
            return (o.tbl_docmgr_document.Name == null) ? "" : o.tbl_docmgr_document.Name;
        }

    }
}

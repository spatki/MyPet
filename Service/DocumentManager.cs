using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.Entity;
using ProcessAccelerator.Core;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Data;
using System.Transactions;

namespace ProcessAccelerator.Service
{
    public class DocumentManager : IDocumentManager
    {
        protected string[] allowedExtensions = new[] {"xls", ".doc", ".xlsx", ".docx", ".txt", ".jpeg", ".gif", ".tiff", ".pdf" };
        protected string fileLocationPath = "App_Data\\uploads";
        protected IRepo<tbl_docmgr_document> document;

        public DocumentManager(IRepo<tbl_docmgr_document> doc)
        {
            this.document = doc;
        }

        public string getFileLocation(int id)
        {
            var doc = document.Get(id);
            if (doc == null) throw new PAException("Document not found");
            return doc.FileLocation;
        }

        public tbl_docmgr_document getDocument(int id)
        {
            var doc = document.Get(id);
            if (doc == null) throw new PAException("Document not found");
            document.getDBContext().Entry(doc).Collection(o => o.tbl_docmgr_version).Load();
            return doc;
        }

        public int addDocument(Stream doc, string root, string fName, int ClientID, string classification, out string errorMessage)
        {
            if (doc.Length > 0)
            {
                var fileName = Path.GetFileName(fName);
                var extension = Path.GetExtension(fileName);

                if (!allowedExtensions.Contains(extension))
                {
                    errorMessage = "Invalid extension. Valid extensions (.xls, .xlsx, .doc, .docx, .txt, .jpeg, .gif, .pdf)";
                    return 0;
                }

                // File is valid, so proceed wih save
                var path = root + fileLocationPath;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                fileName = Guid.NewGuid().ToString() + extension;

                var fileStream = File.Create(path + "\\" + fileName);
                doc.Seek(0, SeekOrigin.Begin);
                doc.CopyTo(fileStream);
                fileStream.Close();

                fileName = path + "\\" + fileName;
                errorMessage = "";

                var retDoc = document.Insert(new tbl_docmgr_document()
                {
                    ClientID = ClientID,
                    Name = fName,
                    FileLocation = fileName,
                    Version = 1,
                    Classification = classification,
                    UploadDate = System.DateTime.Now
                });
                document.Save();
                return retDoc.ID;
            }
            errorMessage = "File does not have any contents";
            return 0;
        }

        public bool addDocumentVersion(int id, Stream doc, string root, string fName, string comments, int user, out string errorMessage)
        {
            var OrginalDoc = document.Get(id);
            if (OrginalDoc == null)
            {
                errorMessage = "Original Document not found";
                return false;
            }
            var version = OrginalDoc.Version + 1;

            if (doc.Length > 0)
            {
                var fileName = Path.GetFileName(fName);
                var extension = Path.GetExtension(fileName);

                if (!allowedExtensions.Contains(extension))
                {
                    errorMessage = "Invalid extension. Valid extensions (.xls, .xlsx, .doc, .docx, .txt, .jpeg, .gif, .tiff, .pdf)";
                    return false;
                }

                // File is valid, so proceed wih save
                var path = root + fileLocationPath;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                fileName = Guid.NewGuid().ToString() + extension;

                var fileStream = File.Create(path + "\\" + fileName);
                doc.Seek(0, SeekOrigin.Begin);
                doc.CopyTo(fileStream);
                fileStream.Close();

                fileName = path + "\\" + fileName;
                errorMessage = "";

                using (TransactionScope scope = new TransactionScope())
                {
                    OrginalDoc.tbl_docmgr_version = new List<tbl_docmgr_version>();
                    OrginalDoc.tbl_docmgr_version.Add(new tbl_docmgr_version()
                    {
                        ID = (int)OrginalDoc.Version,
                        tbl_DocMgr_DocumentID = id,
                        ClientID = OrginalDoc.ClientID,
                        version = OrginalDoc.Version,
                        CreateDate = OrginalDoc.UploadDate,
                        CreateUser = OrginalDoc.UploadedBy,
                        Comments = comments,
                        FileLocation = OrginalDoc.FileLocation,
                    });
                    OrginalDoc.Version = version;
                    OrginalDoc.FileLocation = fileName;
                    OrginalDoc.UploadDate = System.DateTime.Now;
                    OrginalDoc.UploadedBy = user;
                    document.Save();
                    scope.Complete();
                    return true;
                }
            }
            errorMessage = "File does not have any contents";
            return false;
        }

        public bool deleteDocument(int id)
        {
            var doc = document.Get(id);

            if (doc == null) return false;
            document.getDBContext().Entry(doc).Collection(o => o.tbl_docmgr_version).Load();

            using (TransactionScope scope = new TransactionScope())
            {
                // Files uploaded on the file server
                if (doc.tbl_docmgr_version.Any())
                {
                    foreach (var v in doc.tbl_docmgr_version)
                    {
                        // Delete versions
                        if (System.IO.File.Exists(v.FileLocation))
                        {
                            System.IO.File.Delete(v.FileLocation);
                        }
                    }
                }
                // Delete parent
                if (System.IO.File.Exists(doc.FileLocation))
                {
                    System.IO.File.Delete(doc.FileLocation);
                }
                document.executeStoredCommand("delete from tbl_docmgr_version where tbl_DocMgr_DocumentID = " + doc.ID);
                document.executeStoredCommand("delete from tbl_process_document where tbl_DocMgr_DocumentID = " + doc.ID); // Remove from process document by default
                document.executeStoredCommand("delete from tbl_docmgr_document where ID = " + doc.ID);
                scope.Complete();
            }
            return true;
        }

    }
}

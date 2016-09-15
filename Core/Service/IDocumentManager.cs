using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessAccelerator.Core.Model;

namespace ProcessAccelerator.Core.Service
{
    public interface IDocumentManager
    {
        string getFileLocation(int id);
        tbl_docmgr_document getDocument(int id);
        int addDocument(Stream Doc, string root, string fileName, int ClientID, string classification, out string errorMessage);
        bool addDocumentVersion(int id, Stream Doc, string root, string fName, string comments, int user, out string errorMessage);
        bool deleteDocument(int id);
    }
}

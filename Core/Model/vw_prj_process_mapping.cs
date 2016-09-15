using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class vw_prj_process_mapping : Entity
    {
        public int tbl_Org_ProjectID { get; set; }
        public Nullable<int> mstr_Org_Proj_PhaseID { get; set; }
        public int tbl_Process_RepositoryID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentID { get; set; }
        public int Level { get; set; }
        public int levelID { get; set; }
        public string LevelShortName { get; set; }
        public string LevelLongName { get; set; }
        public bool Exclude { get; set; }
        public Nullable<bool> TreatAsTask { get; set; }
    }
}

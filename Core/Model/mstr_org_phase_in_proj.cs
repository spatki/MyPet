using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_org_phase_in_proj : Entity
    {
        public int mstr_Org_Proj_PhaseID { get; set; }
        public int mstr_Org_Project_TypeID { get; set; }

        [ForeignKey("mstr_Org_Proj_PhaseID")]
        public mstr_org_proj_phase mstr_org_proj_phase { get; set; }
        [ForeignKey("mstr_Org_Project_TypeID")]
        public mstr_org_project_type mstr_org_project_type { get; set; }
    }
}

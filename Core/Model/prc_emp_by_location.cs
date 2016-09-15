using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class prc_emp_by_location
    {
        public int ID { get; set; }
        public string EmpCode { get; set; }
        public int ClientID { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public int CurrentDesignation { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string StatusName { get; set; }
        public string ShortDesignation { get; set; }
        public string LongDesignation { get; set; }
        public Nullable<int> LevelID { get; set; }
        public Nullable<int> LevelMasterID { get; set; }
        public string LevelShortName { get; set; }
        public string LevelLongName { get; set; }
    }
}

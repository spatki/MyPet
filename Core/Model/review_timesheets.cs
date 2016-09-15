using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAccelerator.Core.Model
{
    public class review_timesheets
    {
        public int tbl_Org_EmployeeID { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalHours { get; set; }
    }
}

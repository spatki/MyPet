using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAccelerator.Core.Model
{
    public class tbl_org_calendar : Entity
    {
        public Nullable<DateTime> HolidayDate { get; set; }
        public Nullable<byte> Day { get; set; }
        public Nullable<byte> Month { get; set; }
        public bool IsDateSpecific { get; set; }
        public bool IsProjectSpecific { get; set; }
        public bool IsWeekEnd { get; set; }

        public ICollection<tbl_org_proj_calendar> tbl_org_proj_calendar { get; set; }
    }
}

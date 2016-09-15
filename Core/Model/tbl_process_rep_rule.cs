using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class tbl_process_rep_rule: Entity
    {
        public int tbl_Process_Rep_ActivityID { get; set; }
        public byte Event { get; set; }
        public short Status { get; set; }
        public Nullable<int> NextActivity { get; set; }
        public short SequenceNo { get; set; }
        public bool AssignPickup { get; set; }
        public short AssignTo { get; set; }
        public int AssignDetails { get; set; }
        public bool SendMail { get; set; }
        public short CompletionStatus { get; set; }
        public byte AssignDate { get; set; }
        public byte AdjustStartDate { get; set; }
        public short DurationDays { get; set; }
        public Nullable<byte> Hours { get; set; }
        public Nullable<byte> Mins { get; set; }
        public bool MultipleAssignments { get; set; }
        public Nullable<bool> MailOnlyReviewer { get; set; }
        public Nullable<bool> MailCCRoleID { get; set; }
        public Nullable<bool> MailCCADMIN { get; set; }
        public Nullable<bool> MailIncludeDesc { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    
        public virtual tbl_process_rep_task tbl_process_rep_task { get; set; }
        public virtual tbl_process_rep_task tbl_process_rep_task1 { get; set; }
    }
}

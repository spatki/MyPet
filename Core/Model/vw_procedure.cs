using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class vw_procedure : Entity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CreateUser { get; set; }
        public string ReviewUser { get; set; }
        public string Status { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<short> sectionID { get; set; }
        public Nullable<short> sectionSequence { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public Nullable<int> GroupID { get; set; }
        public string GroupName { get; set; }
        public Nullable<short> groupSequence { get; set; }
        public Nullable<short> ParentGroup { get; set; }
        public Nullable<short> level { get; set; }
    }
}

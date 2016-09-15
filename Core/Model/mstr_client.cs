using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class mstr_client : Entity
    {
        public mstr_client()
        {
            this.UserProfile = new HashSet<UserProfile>();
        }

        public string ClientName { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string ShortName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        public virtual ICollection<UserProfile> UserProfile { get; set; }
    }
}

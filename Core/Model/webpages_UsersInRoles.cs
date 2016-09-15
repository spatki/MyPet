using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAccelerator.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public partial class webpages_UsersInRoles
    {
        [Column(Order=0),Key,ForeignKey("UserProfile")]        
        public int UserId { get; set; }
        [Column(Order = 1), Key, ForeignKey("webpages_Roles")]
        public int RoleId { get; set; }
        public Nullable<bool> IsPrimary { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
        [ForeignKey("RoleId")]
        public virtual webpages_Roles webpages_Roles { get; set; }
    }
}

using System;

namespace ProcessAccelerator.Core.Model
{
    public class Entity
    {
        public virtual new int ID { get; set; }
        public Nullable<int> ClientID { get; set; }
    }
}
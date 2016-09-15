using System;
using System.Collections.Generic;

namespace ProcessAccelerator.Core.Model
{
    public partial class Process: Entity
    {
        public string Title { get; set; }
        public System.DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
    }
}

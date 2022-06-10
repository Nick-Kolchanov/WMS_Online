using System;
using System.Collections.Generic;

namespace WMS_Online.Models
{
    public partial class Delivery
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public DateOnly Date { get; set; }

        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}

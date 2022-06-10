using System;
using System.Collections.Generic;

namespace WMS_Online.Models
{
    public partial class ProductSelling
    {
        public int SellingId { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Selling Selling { get; set; } = null!;
    }
}

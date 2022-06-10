using System;
using System.Collections.Generic;

namespace WMS_Online.Models
{
    public partial class Selling
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateOnly Date { get; set; }
        public int? PersonalDiscount { get; set; }

        public decimal Worth => ProductSellings.Sum(d => d.Product.Nomenclature.ProductWorths.OrderByDescending(pw => pw.Date).First(pw => pw.Date < Date).Worth);

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<ProductSelling> ProductSellings { get; set; } = new HashSet<ProductSelling>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class Nomenclature
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Длина наименования должна быть от 5 до 50 символов")]
        public string Name { get; set; } = "";

        [ScaffoldColumn(false)]
        public int? TypeId { get; set; }

        [ScaffoldColumn(false)]
        public decimal Worth => ProductWorths.Any() ? ProductWorths.OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth : 0;

        public virtual NomenclatureType? Type { get; set; }
        public virtual ICollection<NomenclatureProperty> NomenclatureProperties { get; set; } = new HashSet<NomenclatureProperty>();
        public virtual ICollection<ProductWorth> ProductWorths { get; set; } = new HashSet<ProductWorth>();
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}

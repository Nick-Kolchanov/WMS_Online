using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class WarehouseType
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Длина наименования должна быть от 2 до 20 символов")]
        public string? Name { get; set; }

        public virtual ICollection<Warehouse> Warehouses { get; set; } = new HashSet<Warehouse>();
    }
}

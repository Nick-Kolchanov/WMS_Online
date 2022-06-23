using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class Warehouse
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int? WarehouseType { get; set; }

        [Display(Name = "Адрес")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Длина адреса должна быть от 5 до 50 символов")]
        public string Address { get; set; } = "";

        [Display(Name = "Телефон")]
        [StringLength(15, MinimumLength = 11, ErrorMessage = "Длина телефона должна быть от 11 до 15 символов")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string? Phone { get; set; }

        public virtual WarehouseType? WarehouseTypeNavigation { get; set; }
        public virtual ICollection<Inventarization> Inventarizations { get; set; } = new HashSet<Inventarization>();
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}

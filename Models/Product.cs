using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class Product
    {
        public enum ProductStatus
        { 
            На_приемке, 
            На_складе, 
            Продан, 
            Возвращен, 
            Списан,
        }

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int NomenclatureId { get; set; }

        [ScaffoldColumn(false)]
        public int? DeliveryId { get; set; }

        [ScaffoldColumn(false)]
        public ProductStatus Status { get; set; } = ProductStatus.На_приемке;

        [ScaffoldColumn(false)]
        public int? WarehouseId { get; set; }

        [Display(Name = "Адрес ячейки")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина адреса должна быть от 3 до 50 символов")]
        public string? CellAddress { get; set; }

        public virtual Delivery? Delivery { get; set; }
        public virtual Nomenclature Nomenclature { get; set; } = null!;
        public virtual Warehouse? Warehouse { get; set; }
        public virtual ICollection<ProductSelling> ProductSellings { get; set; } = new HashSet<ProductSelling>();
    }
}

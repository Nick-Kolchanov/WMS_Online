using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static WMS_Online.Models.Product;

namespace WMS_Online.Models
{
    public partial class ProductToScreen
    {
        public string Id { get; set; } = "";

        public ICollection<int> IdList { get; set; } = new List<int>();
        public ProductStatus Status { get; set; } = ProductStatus.На_приемке;

        public int Count { get; set; } = 0;

        public int NomenclatureId { get; set; } 
        public int? WarehouseId { get; set; }

        public virtual Nomenclature Nomenclature { get; set; } = null!;
        public virtual Warehouse? Warehouse { get; set; }

        public ProductToScreen() { }
    }
}

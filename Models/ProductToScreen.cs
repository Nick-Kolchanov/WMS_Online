using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static WMS_Online.Models.Product;

namespace WMS_Online.Models
{
    public partial class ProductToScreen
    {
        [ScaffoldColumn(false)]
        public int SelectedId { get; set; } 

        public IEnumerable<int> IdList { get; set; } = new List<int>();
        public ProductStatus Status { get; set; } = ProductStatus.На_приемке;

        public int Count { get; set; } = 0;

        public int NomenclatureId { get; set; } 
        public int? WarehouseId { get; set; }

        public virtual Nomenclature Nomenclature { get; set; } = null!;
        public virtual Warehouse? Warehouse { get; set; }

        public ProductToScreen() { }
        public ProductToScreen(Product product) 
        {
            SelectedId = product.Id;
            IdList.Append(SelectedId);
            Count = 1;
            Nomenclature = product.Nomenclature;
            NomenclatureId = product.NomenclatureId;
            Warehouse = product.Warehouse;
            WarehouseId = product.WarehouseId;
        }
    }
}

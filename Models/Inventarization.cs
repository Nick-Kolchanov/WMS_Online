using System;
using System.Collections.Generic;

namespace WMS_Online.Models
{
    public partial class Inventarization
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? WarehouseId { get; set; }
        public int? ReasonId { get; set; }

        public virtual InventarizationReason? Reason { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
    }
}

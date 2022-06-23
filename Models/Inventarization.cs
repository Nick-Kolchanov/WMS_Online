using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class Inventarization
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Дата начала")]
        public DateOnly StartDate { get; set; }

        [Display(Name = "Дата окончания")]
        public DateOnly? EndDate { get; set; }

        [ScaffoldColumn(false)]
        public int? WarehouseId { get; set; }

        [ScaffoldColumn(false)]
        public int? ReasonId { get; set; }

        public virtual InventarizationReason? Reason { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
    }
}

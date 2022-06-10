using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class Property
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Длина наименования должна быть от 5 до 50 символов")]
        public string Name { get; set; } = "";
        public int? MeasurementUnitId { get; set; }

        public virtual MeasurementUnit? MeasurementUnit { get; set; }
    }
}

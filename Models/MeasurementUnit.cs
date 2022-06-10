using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class MeasurementUnit
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина наименования должна быть от 2 до 50 символов")]
        public string? Name { get; set; }

        [Display(Name = "Короткое наименование")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Длина наименования должна быть от 1 до 10 символов")]
        public string? ShortName { get; set; }

        public virtual ICollection<Property> Properties { get; set; } = new HashSet<Property>();
    }
}

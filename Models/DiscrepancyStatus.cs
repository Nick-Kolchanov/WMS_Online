using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class DiscrepancyStatus
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина наименования должна быть от 3 до 50 символов")]
        public string Name { get; set; } = "";
    }
}

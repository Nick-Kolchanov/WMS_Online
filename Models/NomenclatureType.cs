using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class NomenclatureType
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Длина наименования должна быть от 5 до 50 символов")]
        public string Name { get; set; } = "";
        public virtual ICollection<Nomenclature>? Nomenclatures { get; set; } = new HashSet<Nomenclature>();
    }
}

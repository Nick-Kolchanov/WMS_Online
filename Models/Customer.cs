using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class Customer
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Имя")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Длина имени должна быть от 2 до 20 символов")]
        public string? Name { get; set; }

        [Display(Name = "Телефон")]
        [StringLength(15, MinimumLength = 11, ErrorMessage = "Длина телефона должна быть от 11 до 15 символов")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Required]
        public string Phone { get; set; } = null!;

        public virtual ICollection<Selling> Sellings { get; set; } = new HashSet<Selling>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class Supplier
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "ИНН")]
        [Range(10000000, 9999999999, ErrorMessage = "ИНН может состоять только из 8 или 10 цифр")]
        public long Tin { get; set; }

        [Display(Name = "Имя")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Длина имени должна быть от 2 до 20 символов")]
        public string? Name { get; set; }

        [Display(Name = "Телефон")]
        [StringLength(15, MinimumLength = 11, ErrorMessage = "Длина телефона должна быть от 11 до 15 символов")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string? Phone { get; set; }

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string? Email { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();
    }
}

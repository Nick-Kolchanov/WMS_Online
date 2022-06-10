using System;
using System.ComponentModel;

namespace WMS_Online.Models
{
    public partial class SellingToScreen
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("Дата")]
        public DateOnly Date { get; set; }
        [DisplayName("Скидка")]
        public int? PersonalDiscount { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WMS_Online.Models
{
    public partial class SupplierToScreen
    {
        [DisplayName("null")]
        public int Id { get; set; }
        [DisplayName("ИНН")]
        public long Tin { get; set; }
        [DisplayName("Имя")]
        public string? Name { get; set; }
        [DisplayName("Телефон")]
        public string? Phone { get; set; }
        [DisplayName("E-mail")]
        public string? Email { get; set; }
        [DisplayName("Количество поставок")]
        public int DeliveriesCount { get; set; }
    }
}

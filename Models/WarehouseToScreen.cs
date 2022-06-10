using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WMS_Online.Models
{
    public partial class WarehouseToScreen
    {
        [DisplayName("null")]
        public int Id { get; set; }
        [DisplayName("Тип")]
        public string? WarehouseType { get; set; }
        [DisplayName("Адрес")]
        public string? Address { get; set; }
        [DisplayName("Телефон уполномоченного")]
        public string? Phone { get; set; }
        [DisplayName("Количество продуктов")]
        public int ProductCount { get; set; }
    }
}

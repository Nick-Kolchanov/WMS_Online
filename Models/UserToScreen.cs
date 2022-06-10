using System.ComponentModel;

namespace WMS_Online.Models
{
    public partial class UserToScreen
    {
        [DisplayName("null")]
        public int Id { get; set; }
        [DisplayName("Имя пользователя")]
        public string Name { get; set; } = null!;
        [DisplayName("Администратор")]
        public bool IsAdmin { get; set; }
    }
}

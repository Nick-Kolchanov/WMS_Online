using WMS_Online.Models;

namespace WMS_Online.Utils
{
    public class ProductSession
    {
        public ICollection<ProductToSession> Products { get; set; } = new List<ProductToSession>();
    }
}

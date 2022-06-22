using WMS_Online.Models;

namespace WMS_Online.Utils
{
    public class CartLine
    {
        public ProductToCart Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}

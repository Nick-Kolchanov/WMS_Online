using WMS_Online.Models;

namespace WMS_Online.Utils
{
    public class ProductToCart
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }

        public ProductToCart() { }
        public ProductToCart(Product product)
        {
            Id = product.Id;
            Name = product.Nomenclature.Name;
            Price = product.Nomenclature.Worth;
        }
    }
}

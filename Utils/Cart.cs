using WMS_Online.Models;

namespace WMS_Online.Utils
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();
        private List<ProductToCart> productCollection = new List<ProductToCart>();

        public void AddItem(ProductToCart product, int quantity)
        {
            if (productCollection.Any(pc => pc.Id == product.Id))
            {
                return;
            }

            productCollection.Add(product);

            CartLine? line = lineCollection
              .Where(p => p.Product.Name == product.Name)
              .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public int GetQuantity(ProductToCart product)
        {
            CartLine? line = lineCollection
              .FirstOrDefault(p => p.Product.Name == product.Name);

            if (line == null)
            {
                return 0;
            }
            else
            {
                return line.Quantity;
            }
        }

        public void RemoveLine(ProductToCart product)
        {
            lineCollection.RemoveAll(l => l.Product.Name == product.Name);
            productCollection.RemoveAll(l => l.Name == product.Name);
        }

        public decimal ComputeTotalValue()
        {
            return lineCollection.Sum(e => e.Product.Price * e.Quantity);
        }

        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
        }

        public IEnumerable<ProductToCart> Products
        {
            get { return productCollection; }
        }
    }
}

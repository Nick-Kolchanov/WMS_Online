namespace WMS_Online.Models
{
    public class AddressViewModel
    {
        public Product Product { get; }
        public IEnumerable<string> Addresses { get; }

        public AddressViewModel(Product product, IEnumerable<string> addresses)
        {
            Product = product;
            Addresses = addresses;
        }
    }
}

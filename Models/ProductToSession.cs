namespace WMS_Online.Models
{
    public class ProductToSession
    {
        public string Id { get; set; } = "";
        public ICollection<int> IdList { get; set; } = new List<int>();
        public string Name { get; set; } = "";
    }
}

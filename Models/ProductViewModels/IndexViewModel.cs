namespace WMS_Online.Models.ProductViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<ProductToScreen> Products { get; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public IndexViewModel(IEnumerable<ProductToScreen> products, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Products = products;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}

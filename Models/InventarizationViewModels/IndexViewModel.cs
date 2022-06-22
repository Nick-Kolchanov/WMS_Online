namespace WMS_Online.Models.InventarizationViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Inventarization> Inventarizations { get; }
        public IEnumerable<Discrepancy>? Discrepancies { get; set; }
        public int? InventarizationId { get; set; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public IndexViewModel(IEnumerable<Inventarization> inventarizations, IEnumerable<Discrepancy>? discrepancies,
            int? inventarizationId, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Inventarizations = inventarizations;
            Discrepancies = discrepancies;
            InventarizationId = inventarizationId;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}

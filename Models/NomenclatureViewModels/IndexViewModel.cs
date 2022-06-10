namespace WMS_Online.Models.NomenclatureViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Nomenclature> Nomenclatures { get; }
        public IEnumerable<NomenclatureProperty>? NomenclatureProperties { get; set; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public IndexViewModel(IEnumerable<Nomenclature> nomenclatures, IEnumerable<NomenclatureProperty>? nomenclatureProperties, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Nomenclatures = nomenclatures;
            NomenclatureProperties = nomenclatureProperties;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}

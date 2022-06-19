namespace WMS_Online.Models.NomenclatureViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Nomenclature> Nomenclatures { get; }
        public IEnumerable<NomenclatureProperty>? NomenclatureProperties { get; set; }
        public int? NomenclatureId { get; set; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public IndexViewModel(IEnumerable<Nomenclature> nomenclatures, IEnumerable<NomenclatureProperty>? nomenclatureProperties,
            int? nomenclatureId, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Nomenclatures = nomenclatures;
            NomenclatureProperties = nomenclatureProperties;
            NomenclatureId = nomenclatureId;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}

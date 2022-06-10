namespace WMS_Online.Models.SupplierViewModels
{
    public enum SortState
    {
        TinAsc,   
        TinDesc,
        NameAsc,
        NameDesc,
    }

    public class SortViewModel
    {
        public SortState TinSort { get; }
        public SortState NameSort { get; } 
        public SortState Current { get; }    

        public SortViewModel(SortState sortOrder)
        {
            TinSort = sortOrder == SortState.TinAsc ? SortState.TinDesc : SortState.TinAsc;
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            Current = sortOrder;
        }
    }
}

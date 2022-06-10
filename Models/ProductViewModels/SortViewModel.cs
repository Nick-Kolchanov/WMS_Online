namespace WMS_Online.Models.ProductViewModels
{
    public enum SortState
    {
        NameAsc,
        NameDesc,
        WorthAsc,
        WorthDesc,
    }

    public class SortViewModel
    {
        public SortState NameSort { get; }
        public SortState WorthSort { get; } 
        public SortState Current { get; }    

        public SortViewModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            WorthSort = sortOrder == SortState.WorthAsc ? SortState.WorthDesc : SortState.WorthAsc;
            Current = sortOrder;
        }
    }
}

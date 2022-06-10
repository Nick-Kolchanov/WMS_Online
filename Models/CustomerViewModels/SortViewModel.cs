namespace WMS_Online.Models.CustomerViewModels
{
    public enum SortState
    {
        PhoneAsc,
        PhoneDesc,
        NameAsc,
        NameDesc,
    }

    public class SortViewModel
    {
        public SortState PhoneSort { get; }
        public SortState NameSort { get; } 
        public SortState Current { get; }    

        public SortViewModel(SortState sortOrder)
        {
            PhoneSort = sortOrder == SortState.PhoneAsc ? SortState.PhoneDesc : SortState.PhoneAsc;
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            Current = sortOrder;
        }
    }
}

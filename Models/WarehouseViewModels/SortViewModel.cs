namespace WMS_Online.Models.WarehouseViewModels
{
    public enum SortState
    {
        AddressAsc,
        AddressDesc,
        PhoneAsc,
        PhoneDesc,
    }

    public class SortViewModel
    {
        public SortState PhoneSort { get; }
        public SortState AddressSort { get; } 
        public SortState Current { get; }    

        public SortViewModel(SortState sortOrder)
        {
            PhoneSort = sortOrder == SortState.PhoneAsc ? SortState.PhoneDesc : SortState.PhoneAsc;
            AddressSort = sortOrder == SortState.AddressAsc ? SortState.AddressDesc : SortState.AddressAsc;
            Current = sortOrder;
        }
    }
}

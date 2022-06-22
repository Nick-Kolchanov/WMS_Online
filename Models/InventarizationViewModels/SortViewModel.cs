namespace WMS_Online.Models.InventarizationViewModels
{
    public enum SortState
    {
        StartDateAsc,
        StartDateDesc,
        EndDateAsc,
        EndDateDesc,
    }

    public class SortViewModel
    {
        public SortState StartDateSort { get; }
        public SortState EndDateSort { get; } 
        public SortState Current { get; }    

        public SortViewModel(SortState sortOrder)
        {
            StartDateSort = sortOrder == SortState.StartDateAsc ? SortState.StartDateDesc : SortState.StartDateAsc;
            EndDateSort = sortOrder == SortState.EndDateAsc ? SortState.EndDateDesc : SortState.EndDateAsc;
            Current = sortOrder;
        }
    }
}

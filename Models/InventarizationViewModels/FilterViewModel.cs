using Microsoft.AspNetCore.Mvc.Rendering;

namespace WMS_Online.Models.InventarizationViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<Warehouse> warehouses, int warehouse, string name)
        {
            warehouses.Insert(0, new Warehouse { Address = "Все", Id = 0 });
            Warehouses = new SelectList(warehouses, "Id", "Name", warehouse);
            SelectedWarehouse = warehouse;
            SelectedName = name;
        }
        public SelectList Warehouses { get; } 
        public int SelectedWarehouse { get; }
        public string SelectedName { get; } 
    }
}

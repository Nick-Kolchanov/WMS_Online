using Microsoft.AspNetCore.Mvc.Rendering;

namespace WMS_Online.Models.WarehouseViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<WarehouseType> types, int type, string name)
        {
            types.Insert(0, new WarehouseType { Name = "Все", Id = 0 });
            Types = new SelectList(types, "Id", "Name", type);
            SelectedType = type;
            SelectedName = name;
        }
        public SelectList Types { get; } 
        public int SelectedType { get; }
        public string SelectedName { get; } 
    }
}

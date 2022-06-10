using Microsoft.AspNetCore.Mvc.Rendering;

namespace WMS_Online.Models.ProductViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<NomenclatureType> types, int type, string name)
        {
            types.Insert(0, new NomenclatureType { Name = "Все", Id = 0 });
            Types = new SelectList(types, "Id", "Name", type);
            SelectedType = type;
            SelectedName = name;
        }
        public SelectList Types { get; } 
        public int SelectedType { get; }
        public string SelectedName { get; } 
    }
}

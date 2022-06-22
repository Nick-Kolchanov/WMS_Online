using Microsoft.AspNetCore.Mvc.Rendering;

namespace WMS_Online.Models.ProductViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<NomenclatureType> types, List<Warehouse> warehouses, int type, int warehouse, int status, string name)
        {
            types.Insert(0, new NomenclatureType { Name = "Все", Id = 0 });
            Types = new SelectList(types, "Id", "Name", type);

            warehouses.Insert(0, new Warehouse { Address = "Все", Id = 0 });
            Warehouses = new SelectList(warehouses, "Id", "Address", warehouse);

            List<string> statuses = Enum.GetNames(typeof(Product.ProductStatus)).ToList();
            List<Utils.Status> selectStatuses = new List<Utils.Status>();
            int cnt = 1;
            for (int i = 0; i < statuses.Count; i++)
            {
                selectStatuses.Add(new Utils.Status { Id = cnt, Name = statuses[i] });
                cnt++;
            }
            selectStatuses.Insert(0, new Utils.Status { Name = "Все", Id = 0});
            Statuses = new SelectList(selectStatuses, "Id", "Name", status);

            SelectedType = type;
            SelectedWarehouse = warehouse;
            SelectedStatus = status;
            SelectedName = name;
        }
        public SelectList Types { get; } 
        public int SelectedType { get; }
        public SelectList Warehouses { get; }
        public int SelectedWarehouse { get; }
        public SelectList Statuses { get; }
        public int SelectedStatus { get; }
        public string SelectedName { get; } 
    }
}

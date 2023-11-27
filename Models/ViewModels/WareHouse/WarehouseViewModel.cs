namespace cerberus.Models.ViewModels
{

    public class WarehouseViewModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public DepartmentViewModel Department { get; set; }

    }
}

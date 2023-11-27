namespace cerberus.Models.ViewModels
{
    public class FactorySiteViewModel
    {

        public int id { get; set; }
        public string name { get; set; }
        public int department_id { get; set; }

        public DepartmentViewModel Department { get; set; }


    }
}

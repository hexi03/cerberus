namespace cerberus.Models.ViewModels
{
    using cerberus.Attributes.Validation;
    using System.ComponentModel.DataAnnotations;

    public class FactorySiteCreateModel
    {

        [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Отдел не выбран")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid department_id")]
        [DepartmentID]
        public int department_id { get; set; }


    }
}

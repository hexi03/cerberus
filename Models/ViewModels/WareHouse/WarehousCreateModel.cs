using System.ComponentModel.DataAnnotations;

namespace cerberus.Models.ViewModels
{
    public class WarehouseCreateModel
    {

        [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
        //[RegularExpression("^[A-Za-z]+$", ErrorMessage = "Имя содержит недопустимые символы")]
        public string name { get; set; }

        [Required(ErrorMessage = "Отдел не выбран")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid department_id")]

        public int department_id { get; set; }

    }
}

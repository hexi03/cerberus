using System.ComponentModel.DataAnnotations;

namespace cerberus.Models.ViewModels
{

    public class DepartmentCreateModel
    {

        [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
        public string name { get; set; }



    }



}
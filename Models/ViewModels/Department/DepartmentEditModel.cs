using cerberus.Attributes.Validation;
using cerberus.Models.edmx;
using System.ComponentModel.DataAnnotations;

namespace cerberus.Models.ViewModels
{
    public class DepartmentEditModel
    {
        [Required]
        [DepartmentID]
        public int id { get; set; }


        [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
        public string name { get; set; }

        public class Mapper
        {
            public static DepartmentEditModel map(Department source)
            {
                var res = new DepartmentEditModel();
                res.id = source.id;
                res.name = source.name;

                return res;
            }
        }

    }



}
namespace cerberus.Models.ViewModels
{
    using cerberus.Attributes.Validation;
    using cerberus.Models.edmx;
    using System.ComponentModel.DataAnnotations;


    public class FactorySiteEditModel
    {
        [Required]
        [FactorySiteID]
        public int id { get; set; }

        [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
        public string name { get; set; }

        public class Mapper
        {
            public static FactorySiteEditModel map(FactorySite source)
            {
                var res = new FactorySiteEditModel();
                res.id = source.id;
                res.name = source.name;


                return res;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cerberus.Models.edmx
{
    [MetadataType(typeof(DepartmentMetadata))]
    public partial class Department
    {
        public partial class DepartmentMetadata
        {

            public int id { get; set; }
            [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
            [Display(Name = "Наименование")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
            //[RegularExpression("^[A-Za-z]+$", ErrorMessage = "Имя содержит недопустимые символы")]
            public string name { get; set; }
        }
    }

    

}
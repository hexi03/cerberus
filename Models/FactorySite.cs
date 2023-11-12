using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace cerberus.Models.edmx
{
    using cerberus.Models.Reports;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Linq;

    [MetadataType(typeof(FactorySiteMetadata))]
    public partial class FactorySite
    {
        

        public partial class FactorySiteMetadata
        {

            public int id { get; set; }

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
}

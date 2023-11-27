﻿using cerberus.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace cerberus.Models.ViewModels
{

    public class ItemEditModel
    {
        [Required]
        [ItemID]
        public int id { get; set; }
        [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
        public string name { get; set; }

        [Display(Name = "Партия")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени партии должна быть в пределах от {2} до {1} символов.")]
        public string batch { get; set; }

        [Display(Name = "Единицы")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна обозначения единиц должна быть в пределах от {2} до {1} символов.")]
        public string units { get; set; }
    }
}
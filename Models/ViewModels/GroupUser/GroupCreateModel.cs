using System.ComponentModel.DataAnnotations;

namespace cerberus.Models.ViewModels
{
    public class GroupCreateModel
    {
        [Required]
        [Display(Name = "Наименование группы")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна наименования группы должна быть в пределах от {2} до {1} символов.")]

        public string Name { get; set; }
    }
}
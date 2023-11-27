using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels
{
    public class ApplicationUserCreateModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени пользователя должна быть в пределах от {2} до {1} символов.")]
        public string UserName { get; set; }

        public string Password { get; set; }

        public Dictionary<string, string> Groups { get; set; }

        public List<string> getGroupIDs()
        {
            return Groups.Values.ToList();
        }

        public ApplicationUserCreateModel() { }

    }
}
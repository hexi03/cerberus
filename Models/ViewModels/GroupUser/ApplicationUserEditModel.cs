using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels
{
    public class ApplicationUserEditModel
    {
        public string Id { get; set; }

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

        public class Mapper : ITypeConverter<(ApplicationUser, IList<IdentityRole>), ApplicationUserEditModel>
        {
            public Mapper() { }
            public ApplicationUserEditModel Convert((ApplicationUser, IList<IdentityRole>) source, ApplicationUserEditModel destination, ResolutionContext context)
            {
                destination.Id = source.Item1.Id;
                destination.UserName = source.Item1.UserName;
                destination.Password = null;
                destination.Groups = source.Item2.ToDictionary(e => Guid.NewGuid().ToString(), e => e.Id);
                return destination;
            }
        }

        public ApplicationUserEditModel() { }

    }
}
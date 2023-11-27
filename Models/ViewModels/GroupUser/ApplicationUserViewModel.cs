using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace cerberus.Models.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        //public string Password { get; set; }

        public List<GroupViewModel> Groups { get; set; }

        public class Mapper : ITypeConverter<(ApplicationUser, IList<IdentityRole>), ApplicationUserViewModel>
        {
            public Mapper() { }
            public ApplicationUserViewModel Convert((ApplicationUser, IList<IdentityRole>) source, ApplicationUserViewModel destination, ResolutionContext context)
            {
                destination.Id = source.Item1.Id;
                destination.UserName = source.Item1.UserName;
                destination.Groups = source.Item2.Select(e => new GroupViewModel { Id = e.Id, Name = e.Name}).ToList();
                return destination;
            }


        }
        public ApplicationUserViewModel() { }

    }
}
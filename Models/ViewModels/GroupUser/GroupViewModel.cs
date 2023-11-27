using AutoMapper.Configuration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace cerberus.Models.ViewModels
{
    public class GroupViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
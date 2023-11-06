using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cerberus.DTO
{
    public class ApplicationUserDTO
    {
        [Required]
        public string Name { get; set; }

        public string Password { get; set; }

        public Dictionary<string, string> Roles { get; set; }

        public ApplicationUserDTO() { }

    }
}
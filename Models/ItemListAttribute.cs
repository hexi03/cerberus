using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cerberus.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ItemListAttribute : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            CerberusDBEntities db = new CerberusDBEntities();
            IDictionary<int,int> dict = ((IDictionary<string, string>)value).ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value));

            foreach (var item in dict) {
                if (!(db.ItemsRegistries.Any(it => it.id == item.Key) && item.Value > 0)) return new ValidationResult(ErrorMessage ?? "Элемент листа недопустим.");
            }

            return ValidationResult.Success;
        }
    }
}
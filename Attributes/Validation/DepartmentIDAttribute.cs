using cerberus.Models.edmx;
using System;
using System.ComponentModel.DataAnnotations;

namespace cerberus.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DepartmentIDAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            using (CerberusDBEntities db = new CerberusDBEntities())
            {
                if (db.Departments.Find((int)value) != null) return ValidationResult.Success;

            }
            return new ValidationResult(ErrorMessage ?? "DepartmentID недопустим.");
        }
    }
}
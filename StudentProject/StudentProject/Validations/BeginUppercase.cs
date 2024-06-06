using System.ComponentModel.DataAnnotations;

namespace StudentProject.Validations
{
    public class BeginUppercase: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var startChar = Convert.ToString(value.ToString()[0]);
            if(startChar != startChar.ToUpper())
            {
                return new ValidationResult("Begin with UpperCase character");

            }
            return ValidationResult.Success;
        }
    }
}

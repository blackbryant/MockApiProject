using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.CustomAnnotations
{
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedValues;

        public AllowedValuesAttribute(string[] allowedValues)
        {
            _allowedValues = allowedValues ?? throw new ArgumentNullException(nameof(allowedValues));
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !_allowedValues.Contains(value.ToString()))
            {
                return new ValidationResult($"The field {validationContext.DisplayName} must be one of the following values: {string.Join(", ", _allowedValues)}.");
            }
            return ValidationResult.Success ?? null ;
        }
        
    }
}
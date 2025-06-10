using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mockAPI.Models
{
    public class EventRegistrationDTO :IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [AllowedValues("C# Programming", "JavaScript Development", "Python for Data Science", "Web Development with ASP.NET", "Machine Learning Basics", "Cloud Computing Essentials", "DevOps Practices")]
        public required string EventName { get; set; } =string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        [Compare("Email", ErrorMessage = "Email addresses do not match.")]
        public string ConfirmEmail { get; set; } = string.Empty;

        [Required]
        [Range(1, 7, ErrorMessage = "Number of days attending must be between 1 and 7.")]
        public int DaysAttending { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, FullName: {FullName}, Email: {Email}, EventName: {EventName}, EventDate: {EventDate.ToShortDateString()}, DaysAttending: {DaysAttending}";
        }

        //符合條件就會返回驗證失敗  
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EventDate < DateTime.Now)
            {
                yield return new ValidationResult(
                    "Event date must be in the future.",
                    new[] { nameof(EventDate) });

            }

            if ((FullName.Contains("Garry") || FullName.Contains("Luke")) && EventName == "C# Conference")
            {
                yield return new ValidationResult(
                    $"{FullName} is banned from {EventName}.",
                    new[] { nameof(FullName), nameof(EventName) });
            }
        }
    }
}
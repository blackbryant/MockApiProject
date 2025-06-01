using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using mockAPI.Models;

namespace mockAPI.Validator
{
    public class EventRegistrationDTOValidator : AbstractValidator<EventRegistrationDTO>
    {
        public EventRegistrationDTOValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.EventName)
                .NotEmpty().WithMessage("Event name is required.")
                .Must(BeAValidEventName).WithMessage("Invalid event name.");

            RuleFor(x => x.EventDate)
                .NotEmpty().WithMessage("Event date is required.")
                .GreaterThan(DateTime.Now).WithMessage("Event date must be in the future.");

            RuleFor(x => x.ConfirmEmail)
                .NotEmpty().WithMessage("Confirm email is required.")
                .Equal(x => x.Email).WithMessage("Email addresses do not match.");

            RuleFor(x => x.DaysAttending)
                .InclusiveBetween(1, 7).WithMessage("Number of days attending must be between 1 and 7.");
        }

        private bool BeAValidEventName(string eventName)
        {
            var validEvents = new[] { "C# Programming", "JavaScript Development", "Python for Data Science", "Web Development with ASP.NET", "Machine Learning Basics", "Cloud Computing Essentials", "DevOps Practices" };
            return validEvents.Contains(eventName);
        }



    }
}
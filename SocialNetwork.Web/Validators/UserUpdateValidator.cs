using FluentValidation;
using SocialNetwork.Web.Models;
using System.Text.RegularExpressions;

namespace SocialNetwork.Web.Validators
{
        public class UserUpdateValidator : AbstractValidator<UserUpdateViewModel>
        {
            public UserUpdateValidator()
            {
                RuleFor(x => x.Password) //value can be null or empty or contains 8-50 characters and contains only latin letters and numbers
                    .Must(x => x == null || x == "" || 
                    (x.Length >= 8 && x.Length <=50 && Regex.IsMatch(x, "^[a-zA-Z0-9]*$")))
                    .WithMessage("Not a valid password format. The password must be at least 8 characters long. Only Latin letters and digits are allowed.");

                RuleFor(x => x.Profile)
                    .NotNull()
                    .SetValidator(new ProfileUpdateValidator());
            }
        }
}

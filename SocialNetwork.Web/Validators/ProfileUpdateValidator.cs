using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Web.Models;
using System.Text.RegularExpressions;

namespace SocialNetwork.Web.Validators
{
    public class ProfileUpdateValidator : AbstractValidator<ProfileUpdateViewModel>
    {
        public ProfileUpdateValidator()
        {
            RuleFor(x => x.Name)//value can be null or "" or have between 2-50 characters and have only letters, `, -
               .Must(x => x.IsNullOrEmpty() || 
               (Regex.IsMatch(x, @"^[\p{L}\p{Mn}\p{Pd}'\s]+$") && x.Length >= 2 && x.Length <= 50))
               .WithMessage("Name must be null, empty, or have between 2 and 50 characters and match the required pattern");

            RuleFor(x => x.Surname)
                .Must(x => x.IsNullOrEmpty() || 
                (Regex.IsMatch(x, @"^[\p{L}\p{Mn}\p{Pd}'\s]+$") && x.Length >= 2 && x.Length <= 50))
                .WithMessage("Surname must be null, empty, or have between 2 and 50 characters and match the required pattern");
            

            RuleFor(x => x.Birthday)//value can be null or "" or value > 2023-100 and value < 2023-3
                .Must(date => date==null || date!=DateTime.MinValue || /// default value 01/01/0001 instead ==0
                (date < DateTime.Now.AddYears(-3) && date > DateTime.Now.AddYears(-100)) );

            RuleFor(x => x.Description)
                .MaximumLength(500);

        }
    }
}

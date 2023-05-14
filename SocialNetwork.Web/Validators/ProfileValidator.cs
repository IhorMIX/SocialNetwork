using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class ProfileValidator : AbstractValidator<ProfileCreateViewModel>
{
    public ProfileValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty()
            .Length(2, 50)
            .Matches(@"^[\p{L}\p{Mn}\p{Pd}'\s]+$");

        RuleFor(x => x.Surname).NotNull().NotEmpty()
            .Length(2, 50)
            .Matches(@"^[\p{L}\p{Mn}\p{Pd}'\s]+$");

        RuleFor(x => x.Email).NotNull()
            .EmailAddress();
        
        RuleFor(x => x.Birthday)
            .Must(date => date < DateTime.Now.AddYears(-3) && date > DateTime.Now.AddYears(-100));
        
        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Sex).NotEmpty();
    }
}
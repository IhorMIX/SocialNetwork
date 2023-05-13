using FluentValidation;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BL.Models;

public class ProfileValidator : AbstractValidator<ProfileModel>
{
    public ProfileValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty()
            .MinimumLength(2).MaximumLength(50)
            .Matches(@"^[\p{L}\p{Mn}\p{Pd}'\s]+$")
            .Must(name => char.IsUpper(name[0]) && name.Skip(1).All(char.IsLower))
            .WithMessage("Name must start from upper letter");
        
        RuleFor(x => x.Surname).NotNull().NotEmpty()
            .MinimumLength(2).MaximumLength(10)
            .Matches(@"^[\p{L}\p{Mn}\p{Pd}'\s]+$")
            .Must(surname => char.IsUpper(surname[0]) && surname.Skip(1).All(char.IsLower))
            .WithMessage("Surname must start from upper letter");
        
        RuleFor(x => x.Email).NotNull().NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Birthday).NotNull()
            .Must(date => date < DateTime.Now.AddYears(-3));
        
        RuleFor(x => x.Description)
            .MaximumLength(200);

        RuleFor(x => x.Sex).NotNull().NotEmpty();
    }
}
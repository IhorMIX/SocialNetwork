using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class AuthorizeValidator: AbstractValidator<UserAuthorizeModel>
{
    public AuthorizeValidator()
    {
        RuleFor(x => x.Login).NotEmpty().NotNull()
            .Length(6,30).WithMessage("Not a valid login format for login")
            .Matches("^[a-zA-Z0-9_-]*$")
            .WithMessage("Not a valid login format for login. Only Latin letters, digits, underscore and hyphen are allowed.");
        
        RuleFor(x => x.Password).NotEmpty().NotNull()
            .Length(8, 50).WithMessage("Not a valid password format for login")
            .Matches("^[a-zA-Z0-9]*$")
            .WithMessage("Not a valid password format for login. Only Latin letters and digits are allowed.");

    }
}
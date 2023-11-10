using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class ResetPasswordEmailValidator: AbstractValidator<ResetPasswordEmail>
{
    public ResetPasswordEmailValidator()
    {
        RuleFor(x => x.Email).NotNull()
            .EmailAddress();
    }
}
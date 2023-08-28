using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class ChatValidator : AbstractValidator<ChatCreateViewModel>
{
    public ChatValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull()
            .Length(1, 50)
            .Matches(@"^[\p{L}\p{Mn}\p{Pd}'\s]+$")
            .WithMessage("Not a valid name format. Only Latin letters, digits, underscore and hyphen are allowed.");
    }
}
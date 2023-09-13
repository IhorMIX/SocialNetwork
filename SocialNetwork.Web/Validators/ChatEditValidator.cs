using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class ChatEditValidator : AbstractValidator<ChatEditModel>
{
    public ChatEditValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull()
            .Length(1, 50)
            .WithMessage("Length has to be from 1 to 50");
        
    }
}
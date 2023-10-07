using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class RoleUpdateValidator : AbstractValidator<RoleUpdateModel>
{
    public RoleUpdateValidator()
    {
        RuleFor(x => x.rolemodel)
            .NotNull()
            .SetValidator(new RoleEditValidator());
    }
}
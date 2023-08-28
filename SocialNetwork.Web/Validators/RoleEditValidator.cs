using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class RoleEditValidator : AbstractValidator<RoleEditModel>
{
    public RoleEditValidator()
    {
        RuleFor(x => x.RoleName).NotNull().NotEmpty()
            .Length(6, 30)
            .Matches(@"^[\p{L}\p{Mn}\p{Pd}'\s]+$")
            .WithMessage("Wrong symbols");
        RuleFor(x => x.Rank).NotNull().NotEmpty()
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(int.MaxValue);
    }
}
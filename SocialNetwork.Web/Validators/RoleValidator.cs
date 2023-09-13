using FluentValidation;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class RoleValidator : AbstractValidator<CreateRoleModel>
{
    public RoleValidator()
    {
        RuleFor(x => x.RoleName).NotNull().NotEmpty()
            .NotNull()
            .NotEmpty()
            .Length(2, 30)
            .WithMessage("Role name should be between 2 and 30 characters");
        RuleFor(x => x.Rank).NotNull().NotEmpty()
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(int.MaxValue);

    }
}
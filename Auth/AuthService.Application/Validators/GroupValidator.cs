using AuthService.Domain.Entities.Groups;
using FluentValidation;

namespace AuthService.Application.Validators
{
    public class GroupValidator : AbstractValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Group name is required")
                .Length(3, 100).WithMessage("Group name must be between 3 and 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Group must have a creator");

            RuleFor(x => x.Visibility)
                .IsInEnum().WithMessage("Invalid group visibility");
        }
    }
}

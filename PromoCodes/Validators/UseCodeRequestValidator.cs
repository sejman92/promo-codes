using FluentValidation;
using PromoCodes.Models;

namespace PromoCodes.Validators;

public class UseCodeRequestValidator : AbstractValidator<UseCodeRequest>
{
    public UseCodeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotNull()
            .WithMessage($"{nameof(UseCodeRequest.Code)} is required.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage($"{nameof(UseCodeRequest.Code)} could not be empty.");

        RuleFor(x => x.Code)
            .Length(ConstValidationValues.MinimalCodeLength, ConstValidationValues.MaximalCodeLength)
            .WithMessage(
                $"{nameof(UseCodeRequest.Code)} length must be between {ConstValidationValues.MinimalCodeLength} and {ConstValidationValues.MaximalCodeLength}");
    }
}
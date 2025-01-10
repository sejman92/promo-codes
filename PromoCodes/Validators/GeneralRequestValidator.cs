using FluentValidation;
using PromoCodes.Models;

namespace PromoCodes.Validators;

public class GeneralRequestValidator : AbstractValidator<GenerateRequest>
{
    public GeneralRequestValidator()
    {
        RuleFor(x => x.Count).InclusiveBetween(ConstValidationValues.MinimalCount, ConstValidationValues.MaximalCount)
            .WithMessage($"Count must be between {ConstValidationValues.MinimalCount} and {ConstValidationValues.MaximalCount}");
        
        RuleFor(x => x.Length).InclusiveBetween(ConstValidationValues.MinimalCodeLength, ConstValidationValues.MaximalCodeLength)
            .WithMessage($"Length must be between {ConstValidationValues.MinimalCodeLength} and {ConstValidationValues.MaximalCodeLength}");
    }
}
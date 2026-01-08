using FluentValidation;
using InterventionsService.DTOs;

namespace InterventionsService.Validators;

public class CreateInterventionRequestValidator : AbstractValidator<CreateInterventionRequest>
{
    public CreateInterventionRequestValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.LaborCost).GreaterThanOrEqualTo(0);
    }
}

public class AddPartRequestValidator : AbstractValidator<AddPartRequest>
{
    public AddPartRequestValidator()
    {
        RuleFor(x => x.PartName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Qty).GreaterThan(0);
    }
}

public class LaborCostRequestValidator : AbstractValidator<LaborCostRequest>
{
    public LaborCostRequestValidator()
    {
        RuleFor(x => x.LaborCost).GreaterThanOrEqualTo(0);
    }
}

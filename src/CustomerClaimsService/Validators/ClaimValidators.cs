using CustomerClaimsService.DTOs;
using FluentValidation;

namespace CustomerClaimsService.Validators;

public class CreateClaimRequestValidator : AbstractValidator<CreateClaimRequest>
{
    public CreateClaimRequestValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty();
        RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}

public class UpdateClaimStatusRequestValidator : AbstractValidator<UpdateClaimStatusRequest>
{
    public UpdateClaimStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

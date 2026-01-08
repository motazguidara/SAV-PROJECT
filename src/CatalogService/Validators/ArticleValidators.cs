using CatalogService.DTOs;
using FluentValidation;

namespace CatalogService.Validators;

public class CreateArticleRequestValidator : AbstractValidator<CreateArticleRequest>
{
    public CreateArticleRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WarrantyMonths).InclusiveBetween(0, 120);
    }
}

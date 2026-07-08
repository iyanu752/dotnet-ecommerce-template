using System;
using FluentValidation;

namespace ecommerce.api;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(x => x.Name)
      .NotEmpty()
      .MaximumLength(100);

        RuleFor(x => x.Description)
        .MaximumLength(500);

        RuleFor(x => x.Price)
        .GreaterThan(0);

        RuleFor(x => x.Stock)
        .GreaterThanOrEqualTo(0);
    }

}

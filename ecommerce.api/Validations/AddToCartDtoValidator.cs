using System;
using FluentValidation;

namespace ecommerce.api;

public class AddToCartDtoValidator: AbstractValidator<AddToCartDto>
{

    public AddToCartDtoValidator()
    {
        RuleFor(x => x.ProductId)
        .GreaterThan(0);

        RuleFor(x => x.Quantity)
        .GreaterThan(0);
    }

}

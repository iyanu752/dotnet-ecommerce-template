using System;
using FluentValidation;

namespace ecommerce.api;

public class UpdateCartItemDtoValidation : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidation()
    {
        RuleFor (X => X.Quantity)
        .GreaterThan(0);
    }

}

using System;
using AutoMapper;

namespace ecommerce.api;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<CartItem, CartItemDto>()
        .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
        .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));

        CreateMap<Cart, CartDto>()
        .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.UserId))
        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<Order, OrderDto>();
    }


}

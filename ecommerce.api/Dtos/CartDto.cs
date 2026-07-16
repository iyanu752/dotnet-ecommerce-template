using System;

namespace ecommerce.api;

public class CartDto
{
    public int CartId {get; set;}
    public List<CartItemDto> Items {get; set;} = new();
    public decimal Total => Items.Sum( i => i.Total);

}

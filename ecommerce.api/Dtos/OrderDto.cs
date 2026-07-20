using System;

namespace ecommerce.api;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt {get; set;}
    public OrderStatus Status {get; set;}
    public decimal TotalAmount {get; set;}
    public List<OrderItem> Items {get; set;} = new();

}

using System;

namespace ecommerce.api;

public class Cart
{
    public int MyProperty { get; set; }

    public int UserId { get; set; }
    public  User User { get; set; } = null!;
    public List<CartItem> Items {get; set;} = new ();

}

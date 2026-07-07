using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.api;

public class Product
{
    public int Id {get; set;}
    public string Name { get; set;} = string.Empty;
    public string Description { get; set;} = string.Empty;
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price {get; set;}
    public int Stock {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

}

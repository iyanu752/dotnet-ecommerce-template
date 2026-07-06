using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProduct()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Pc",
                    Description = "High end spec pc",
                    Price = 12.99m,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "PlayStation 5",
                    Description = "Brand new playstation 5",
                    Price = 900.99m,
                    Stock = 10
                },
                new Product
                {
                    Id = 3,
                    Name = "xbox 360",
                    Description = "Fairly used xbox 360 in mint condition",
                    Price = 500.99m,
                    Stock = 10
                },
            };
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
             var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Pc",
                    Description = "High end spec pc",
                    Price = 12.99m,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "PlayStation 5",
                    Description = "Brand new playstation 5",
                    Price = 900.99m,
                    Stock = 10
                },
                new Product
                {
                    Id = 3,
                    Name = "xbox 360",
                    Description = "Fairly used xbox 360 in mint condition",
                    Price = 500.99m,
                    Stock = 10
                },
            };

            var product = products.FirstOrDefault( p => p.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
            
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if( product is null )
            {
                return BadRequest();
            }
            product.Id = new Random().Next(1000, 9999);
            product.CreatedAt = DateTime.UtcNow;
            return CreatedAtAction(
                nameof(GetProductById),
                new{id = product.Id},
                product
            );

        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id , [FromBody] Product updatedProduct)
        {
            if (updatedProduct is null)
            {
                return BadRequest();
            }
            var product = new List<Product>
            {
              new Product { Id = 1 , Name = "Lenovo leigon", Price = 1500.00m, Stock=30, CreatedAt=DateTime.UtcNow, Description="High performance gaming pc"}  
            };
            var existingProducts = product.FirstOrDefault(p=> p.Id == id);
            if(existingProducts is null)
            {
                return NotFound();
            } 
            existingProducts.Name = updatedProduct.Name;
            existingProducts.Description = updatedProduct.Description;
            existingProducts.Price = updatedProduct.Price;
            existingProducts.Stock = updatedProduct.Stock;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
             var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Pc",
                    Description = "High end spec pc",
                    Price = 12.99m,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "PlayStation 5",
                    Description = "Brand new playstation 5",
                    Price = 900.99m,
                    Stock = 10
                },
                new Product
                {
                    Id = 3,
                    Name = "xbox 360",
                    Description = "Fairly used xbox 360 in mint condition",
                    Price = 500.99m,
                    Stock = 10
                },
            };

            var product = products.FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            products.Remove(product);
            return NoContent();

        }


    }
}

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
            var products = new[]
            {
                new{Id = 1, Name="Pc", Price=1300},
                new{Id = 2, Name="Ps5", Price=1400},
                new{Id = 3, Name="Nitendo Switch", Price=1800},
                new{Id = 1, Name="xbox 360", Price=1000}
            };
            return Ok(products);
        }

    }
}

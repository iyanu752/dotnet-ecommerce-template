using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct([FromQuery] ProductQueryParameters query)
        {
            var (data, totalCount) = await _productService.GetAllProductsAsync(query);
            var response =  new
            {
                data,
                totalCount,
                query.Page,
                query.PageSize
            };

            return Ok(response);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {

            var product = await _productService.GetProductById(id);
            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var product = _productService.CreateProductAsync(createProductDto);


            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product
            );

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updatedProductdto)
        {
     
            var existingProducts = await _productService.UpdateProductsAsync(id, updatedProductdto);

            if (existingProducts is null)
            {
                return NotFound();
            }
            return Ok(existingProducts);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);

            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();

        }

        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception ("This is a test exception from products controller");
        }


    }
}

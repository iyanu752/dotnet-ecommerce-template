using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            var products = await _context.Products.ToListAsync();

            var result = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {

            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ProductDto>(product);

            return Ok(result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            var result = _mapper.Map<ProductDto>(product);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                result
            );

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updatedProductdto)
        {
     
            var existingProducts = await _context.Products.FindAsync(id);

            if (existingProducts is null)
            {
                return NotFound();
            }

            _mapper.Map(updatedProductdto, existingProducts);

            var result = _mapper.Map<ProductDto>(existingProducts);
            await _context.SaveChangesAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();

        }


    }
}

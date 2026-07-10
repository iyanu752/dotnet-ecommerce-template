using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
namespace ecommerce.api;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if(product == null)
        {
            return null;
        }
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync (CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateProductsAsync (int id, UpdateProductDto updateProductDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return null;
        }
        _mapper.Map(updateProductDto, product);
        await _context.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if ( product == null)
        {
            return false;
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

}

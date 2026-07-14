using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

    public async Task<(IEnumerable<ProductDto> Data, int TotalCount)> GetAllProductsAsync(ProductQueryParameters query)
    {
        var productQuery =  _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.search))
        {
            productQuery = productQuery.Where(p => p.Name.Contains(query.search));
        }

        if(query.MinPrice.HasValue)
        {
            productQuery = productQuery.Where(p => p.Price >= query.MinPrice.Value );
        } 

        if(query.MaxPrice.HasValue)
        {
            productQuery = productQuery.Where(p => p.Price <= query.MaxPrice.Value);
        }

        var totalCount = await productQuery.CountAsync();

        productQuery = query.SortBy?.ToLower() switch
        {
            "price" => query.Decending
            ? productQuery.OrderByDescending(p => p.Price)
            : productQuery.OrderBy(p => p.Price),

            "name" => query.Decending
            ? productQuery.OrderByDescending(P => P.Name)
            :productQuery.OrderBy(p => p.Name),

            _ => productQuery.OrderBy(p => p.Id)
        };

        var products = await productQuery
        .Skip((query.Page -1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync();

        var data = _mapper.Map<IEnumerable<ProductDto>>(products);
        return(data, totalCount);

    }

    public async Task<ProductDto?> GetProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return null;
        }
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateProductsAsync(int id, UpdateProductDto updateProductDto)
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
        if (product == null)
        {
            return false;
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

}

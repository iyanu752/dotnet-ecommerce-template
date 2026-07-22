using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
namespace ecommerce.api;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly IMemoryCache _cache;

    public ProductService(AppDbContext context, IMapper mapper, ILogger<ProductService> logger, IMemoryCache memoryCache)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _cache = memoryCache;

    }

    private const string ProductsCacheKey = "products:list";
    private const string ProductByIdCacheKeyPrefix = "products:id";

    public async Task<(IEnumerable<ProductDto> Data, int TotalCount)> GetAllProductsAsync(ProductQueryParameters query)
    {

        var cacheKey = $"{ProductsCacheKey}:page{query.Page}:size={query.PageSize}:search{query.search}:min={query.MinPrice}:max{query.MaxPrice}:sort={query.SortBy}:desc={query.Decending}";

        if (_cache.TryGetValue(cacheKey, out (IEnumerable<ProductDto>Data, int TotalCount) cachedResult))
        {
            _logger.LogInformation("Products list loaded from cache.key : {cacheKey}", cacheKey);
            return cachedResult;
        }
        _logger.LogInformation("Products list not found in database");
     
        var productQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.search))
        {
            productQuery = productQuery.Where(p => p.Name.Contains(query.search));
        }

        if (query.MinPrice.HasValue)
        {
            productQuery = productQuery.Where(p => p.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
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
            : productQuery.OrderBy(p => p.Name),

            _ => productQuery.OrderBy(p => p.Id)
        };

        var products = await productQuery
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync();
        _logger.LogInformation("Retrieving Products with pagination");
        var data = _mapper.Map<IEnumerable<ProductDto>>(products);
        var result = (data, totalCount);
        _cache.Set(
            cacheKey,
            result,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),
                SlidingExpiration = TimeSpan.FromMinutes(1)
            }
        );
        return (data, totalCount);

    }

    public async Task<ProductDto?> GetProductById(int id)
    {
        var cacheKey = $"{ProductByIdCacheKeyPrefix} {id}";
        if (_cache.TryGetValue(cacheKey, out ProductDto? cachedProdut))
        {
            _logger.LogInformation("Product with Id {ProductId} loaded from cache", id);
            return cachedProdut;

        }
        _logger.LogInformation("Product with ID {ProductId} not found in cache.Loadingfrom DB.", id);

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Failed to get product by Id. Product with ID {ProductId} was not found", id);
            return null;
        }
        var result =  _mapper.Map<ProductDto>(product);
        _cache.Set(
            cacheKey,
            result,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            }
        );

        return result;
       
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        _logger.LogInformation("Creating product with name: {ProductName}", createProductDto.Name);
        var product = _mapper.Map<Product>(createProductDto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        RemoveProductCaches();
        _logger.LogInformation("Product created successfully with ID: {ProductID}", product.Id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateProductsAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product update failed. Product with ID {ProductId} was not found", id);
            return null;
        }
        _mapper.Map(updateProductDto, product);
        await _context.SaveChangesAsync();
        RemoveProductCaches();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product delete failed. Product with ID {ProductId} was not found", id);
            return false;
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        RemoveProductCaches();
        return true;
    }

    private void RemoveProductCaches(int? productId = null)
    {
        if (productId.HasValue)
        {
            _cache.Remove($"{ProductByIdCacheKeyPrefix}{productId.Value}");
        }
        _logger.LogInformation($"product cache invalidated : {productId}");
    }

}

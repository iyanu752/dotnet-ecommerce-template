using System;

namespace ecommerce.api;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductById(int id);

    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);

    Task<ProductDto?> UpdateProductsAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);

}

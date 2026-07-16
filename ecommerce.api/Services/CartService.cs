using System;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.api;

public class CartService : ICartService
{
    public readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CartService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == addToCartDto.ProductId);

        if (!productExists)
        {
            throw new KeyNotFoundException("Product not found");
        }

        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == addToCartDto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += addToCartDto.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = addToCartDto.ProductId,
                Quantity = addToCartDto.Quantity
            });
        }
        await _context.SaveChangesAsync();
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto?> UpdateCartItemAsync( int userId, int productId, UpdateCartItemDto updateCartItemDto)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var item = cart.Items.FirstOrDefault( i => i.ProductId == productId);

        if (item == null)
        {
            return null; 
        }

        item.Quantity = updateCartItemDto.Quantity;
        await _context.SaveChangesAsync();
        return _mapper.Map<CartDto>(cart);
    } 

    public async Task <bool> RemoveFromCartAsync(int userId, int productId)
    {
           var cart = await GetOrCreateCartAsync(userId);
        var item = cart.Items.FirstOrDefault( i => i.ProductId == productId);

        if (item == null)
        {
            return false;
        }
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await _context.Carts
        .Include(c => c.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null)
        {
            return cart;
        }

        cart = new Cart
        {
            UserId = userId
        };

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

}

using E_Shopping_API.DTOs;
using E_Shopping_API.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Shopping_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            if (userId == 0)
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found.",
                    Data = null
                });
            }


            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);


            if (cart == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Cart not found.",
                    Data = null
                });
            }


            var cartItems = cart.CartItems.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Price = item.Product.Price,
                Image = item.Product.Image,
                Description = item.Product.Description,
                Quantity = item.Quantity
            }).ToList();


            return Ok(new ApiResponse<List<CartItemDto>>
            {
                Status = true,
                Message = "Cart retrieved successfully.",
                Data = cartItems
            });
        }



        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            var userId = GetUserId();

            if (userId == 0)
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found.",
                    Data = null
                });
            }


            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);


            if (product == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Product not found.",
                    Data = null
                });
            }


            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);


            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }


            var existingItem = cart.CartItems
                .FirstOrDefault(x => x.ProductId == dto.ProductId);


            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                _context.CartItems.Add(cartItem);
            }


            await _context.SaveChangesAsync();


            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Product added to cart successfully.",
                Data = null
            });
        }



        // PUT: api/cart/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuantity(
            int id,
            UpdateQuantityDto dto)
        {
            var userId = GetUserId();


            var cartItem = await _context.CartItems
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Cart.UserId == userId);


            if (cartItem == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Cart item not found.",
                    Data = null
                });
            }


            cartItem.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();


            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Quantity updated successfully.",
                Data = null
            });
        }



        // DELETE: api/cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var userId = GetUserId();


            var cartItem = await _context.CartItems
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Cart.UserId == userId);


            if (cartItem == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Cart item not found.",
                    Data = null
                });
            }


            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();


            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Item removed successfully.",
                Data = null
            });
        }



        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }


            return int.Parse(userId);
        }
    }
}

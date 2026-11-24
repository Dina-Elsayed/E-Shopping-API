using E_Shopping_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Shopping_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }



        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Description = p.Description
                })
                .ToListAsync();


            if (!products.Any())
            {
                return Ok(new ApiResponse<List<ProductDto>>
                {
                    Status = false,
                    Message = "No products found.",
                    Data = null
                });
            }


            return Ok(new ApiResponse<List<ProductDto>>
            {
                Status = true,
                Message = "Products retrieved successfully.",
                Data = products
            });
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementService.Data;
using RestaurantManagementService.Data.Models;
using RestaurantManagementService.Services;

namespace RestaurantManagementService.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace RestaurantManagementService.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [Authorize] 
        public class RestaurantController : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            private readonly RestaurantService _restaurantService;
            public RestaurantController(ApplicationDbContext context, RestaurantService restaurantService)
            {
                _context = context;
                _restaurantService = restaurantService;
            }

            [HttpPost("add-menu-item")]
            [Authorize(Roles = "RestaurantOwner,Admin")] // Only allowed for RestaurantOwner and Admin
            public IActionResult AddMenuItem([FromBody] MenuItem menuItem)
            {
                if (menuItem == null)
                {
                    return BadRequest("Menu item is required.");
                }

                menuItem.CreatedAt = DateTime.UtcNow;
                menuItem.UpdatedAt = DateTime.UtcNow;

                _context.MenuItems.Add(menuItem);
                _context.SaveChanges();

                return Ok("Menu item added successfully.");
            }

            [HttpPut("update-menu-item/{id}")]
            [Authorize(Roles = "RestaurantOwner,Admin")]
            public IActionResult UpdateMenuItem(int id, [FromBody] MenuItem menuItem)
            {
                var existingMenuItem = _context.MenuItems.FirstOrDefault(x => x.MenuItemId == id);

                if (existingMenuItem == null)
                {
                    return NotFound("Menu item not found.");
                }

                existingMenuItem.Name = menuItem.Name;
                existingMenuItem.Description = menuItem.Description;
                existingMenuItem.Price = menuItem.Price;
                existingMenuItem.IsAvailable = menuItem.IsAvailable;
                existingMenuItem.ImageUrl = menuItem.ImageUrl;
                existingMenuItem.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok("Menu item updated successfully.");
            }

            [HttpDelete("delete-menu-item/{id}")]
            [Authorize(Roles = "RestaurantOwner,Admin")]
            public IActionResult DeleteMenuItem(int id)
            {
                var menuItem = _context.MenuItems.FirstOrDefault(x => x.MenuItemId == id);

                if (menuItem == null)
                {
                    return NotFound("Menu item not found.");
                }

                _context.MenuItems.Remove(menuItem);
                _context.SaveChanges();

                return Ok("Menu item deleted successfully.");
            }

            [HttpPut("set-restaurant-availability/{id}")]
            [Authorize(Roles = "RestaurantOwner,Admin")]
            public IActionResult SetRestaurantAvailability(int id, [FromBody] bool isAvailable)
            {
                var restaurant = _context.Restaurants.FirstOrDefault(x => x.RestaurantId == id);

                if (restaurant == null)
                {
                    return NotFound("Restaurant not found.");
                }

                restaurant.IsAvailable = isAvailable;
                restaurant.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok($"Restaurant availability updated to {isAvailable}.");
            }

            [HttpPost("add-restaurant")]
            [Authorize(Roles = "RestaurantOwner,Admin")] // Only allowed for RestaurantOwner and Admin
            public async Task<IActionResult> AddRestaurant([FromBody] RestaurantDto restaurantDto)
            {
                var result = await _restaurantService.ManageRestaurantAsync(
                    "Insert",
                    null,
                    restaurantDto.OwnerId,
                    restaurantDto.CategoryId,
                    restaurantDto.Name,
                    restaurantDto.Description,
                    restaurantDto.Address,
                    restaurantDto.PhoneNumber,
                    restaurantDto.Email,
                    restaurantDto.OpeningTime,
                    restaurantDto.ClosingTime,
                    restaurantDto.IsApproved,
                    restaurantDto.IsAvailable);  // Make sure this is passed

                return Ok(result);
            }

            [HttpPut("update-restaurant/{restaurantId}")]
            [Authorize(Roles = "RestaurantOwner,Admin")]
            public async Task<IActionResult> UpdateRestaurant(int restaurantId, [FromBody] RestaurantDto restaurantDto)
            {
                var result = await _restaurantService.ManageRestaurantAsync(
                    "Update",
                    restaurantId,
                    restaurantDto.OwnerId,
                    restaurantDto.CategoryId,
                    restaurantDto.Name,
                    restaurantDto.Description,
                    restaurantDto.Address,
                    restaurantDto.PhoneNumber,
                    restaurantDto.Email,
                    restaurantDto.OpeningTime,
                    restaurantDto.ClosingTime,
                    restaurantDto.IsApproved,
                    restaurantDto.IsAvailable); 

                return Ok(result);
            }

            [HttpDelete("delete-restaurant/{restaurantId}")]
            [Authorize(Roles = "RestaurantOwner,Admin")]
            public async Task<IActionResult> DeleteRestaurant(int restaurantId)
            {
                var result = await _restaurantService.ManageRestaurantAsync(
                    "Delete",             
                    restaurantId,          
                    0,                   
                    0,                      
                    null, 
                    null,
                    null,       
                    null, 
                    null,
                    TimeSpan.Zero,      // Set default DateTime.MinValue if necessary
                    TimeSpan.Zero,      // Set default DateTime.MinValue if necessary
                    true,                   // Can be set based on logic or validation
                    true                    // Can be set based on logic or validation
                );

                return Ok(result);
            }


        }
    }

}

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
    using System.Security.Claims;

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
            [Authorize(Roles = "RestaurantOwner")]
            public async Task<IActionResult> AddRestaurant([FromBody] RestaurantDto restaurantDto)
            {
                try
                {
                    // Extract userId and email from the token
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Access NameIdentifier claim
                    var userEmail = User.FindFirstValue(ClaimTypes.Name); // Access Name claim

                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
                    {
                        return Unauthorized("User ID or Email is missing in the token.");
                    }

                    // Log the extracted values for debugging
                    Console.WriteLine($"UserId: {userId}, Email: {userEmail}");

                    // Pass the email from the token to the service method
                    var result = await _restaurantService.ManageRestaurantAsync(
                        "Insert",
                        restaurantDto.RestruntId,
                        int.Parse(userId), // Convert userId to int
                        restaurantDto.CategoryId,
                        restaurantDto.Name,
                        restaurantDto.Description,
                        restaurantDto.Address,
                        restaurantDto.PhoneNumber,
                        userEmail, // Use email from token
                        restaurantDto.OpeningTime,
                        restaurantDto.ClosingTime,
                        restaurantDto.IsApproved,
                        restaurantDto.IsAvailable
                    );

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
            [HttpGet("get-restaurant-menu/{restaurantId}")]
            [Authorize]
            public async Task<IActionResult> GetRestaurantMenu(int restaurantId)
            {
               
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                
                return await _restaurantService.GetRestaurantMenusAsync(restaurantId, userId);
            }

            [HttpPut("update-restaurant/{restaurantId}")]
            [Authorize(Roles = "RestaurantOwner,Admin")]
            public async Task<IActionResult> UpdateRestaurant(int restaurantId, [FromBody] RestaurantDto restaurantDto)
            {
                try
                {
                    
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                    var userEmail = User.FindFirstValue(ClaimTypes.Name); 

                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
                    {
                        return Unauthorized("User ID or Email is missing in the token.");
                    }
                   
                    restaurantDto.RestruntId = restaurantId;
                    restaurantDto.Email = userEmail;
                    int.TryParse(userId.ToString(), out int id);
                    
                    var result = await _restaurantService.ManageRestaurantAsync(
                        "Update",
                        restaurantId,
                        id,
                        null,
                        restaurantDto.Name,
                        restaurantDto.Description,
                        restaurantDto.Address,
                        restaurantDto.PhoneNumber,
                        null, 
                        restaurantDto.OpeningTime,
                        restaurantDto.ClosingTime,
                       null,
                        restaurantDto.IsAvailable
                    );

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }


            [HttpDelete("delete-restaurant/{restaurantId}")]
            [Authorize(Roles = "RestaurantOwner,Admin")]
            public async Task<IActionResult> DeleteRestaurant(int restaurantId)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = User.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User ID or Email is missing in the token.");
                }

                int.TryParse(userId.ToString(), out int id);

                var result = await _restaurantService.ManageRestaurantAsync(
                    "Delete",             
                    restaurantId,          
                    id,                   
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
            [HttpGet("get-restaurants")]
            [Authorize(Roles = "RestaurantOwner")]
            public async Task<IActionResult> GetRestaurantsForOwner()
            {
                try
                {
                    // Extract userId and email from the token
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Access NameIdentifier claim
                    var userEmail = User.FindFirstValue(ClaimTypes.Name); // Access Name claim

                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
                    {
                        return Unauthorized("User ID or Email is missing in the token.");
                    }
                    ;
                    int id = int.TryParse(userId.ToString(),out int passid) ? passid : 0 ;
                   
                    var restaurants = await _restaurantService.GetRestaurantsByOwnerEmailAsync(userEmail,id);

                    if (restaurants == null || !restaurants.Any())
                    {
                        return NotFound("No restaurants found for this owner.");
                    }

                    return Ok(restaurants);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }



        }
    }

}

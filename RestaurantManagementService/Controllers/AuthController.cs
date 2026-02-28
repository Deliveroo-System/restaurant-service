using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using RestaurantManagementService.Data.Models;
using RestaurantManagementService.Services;
using BCrypt.Net;

using RestaurantManagementService.Data;

namespace RestaurantManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        public AuthController(JwtService jwtService, ApplicationDbContext context, UserService userService)
        {
            _jwtService = jwtService;
            _context = context;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            // Hash the password
            var hashedPassword = new UserService().HashPassword(registrationDto.Password);

            try
            {
                // Assuming the roleId is passed as part of the registration data
                int roleId = 1; // Example: Assign the "RestaurantOwner" role
                await _userService.RegisterUserAsync(
                    registrationDto.FirstName,
                    registrationDto.LastName,
                    registrationDto.Email,
                    registrationDto.PhoneNumber,
                    hashedPassword,
                    roleId
                );

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error registering user: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }
}

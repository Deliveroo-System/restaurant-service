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
        public AuthController(JwtService jwtService, ApplicationDbContext context, UserService userService, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _context = context;
            _userService = userService;
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _userService = new UserService(connectionString, _context);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            // Hash the password
            var hashedPassword = new UserService().HashPassword(registrationDto.Password);

            try
            {
                
                await _userService.RegisterUserAsync(
                    
                    registrationDto.FirstName,
                    registrationDto.LastName,
                    registrationDto.Email,
                    registrationDto.PhoneNumber,
                    hashedPassword,
                    registrationDto.id
                );

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error registering user: {ex.Message}");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Use UserService to find the user by email using a SQL query
            var logUser = await _userService.GetUserByEmailAsync(loginDto.Email);

            // Check if the user exists and if the password matches
            if (logUser == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, logUser.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }
          
            var token = _jwtService.GenerateToken(logUser.RoleName, logUser.UserId, logUser.Email);
          
            var role = logUser.RoleName;

            // Return the token and role in the response
            return Ok(new { Token = token, Role = role });
        }



    }
}

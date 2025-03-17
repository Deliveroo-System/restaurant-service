using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagementService.Data.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RestaurantManagementService.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(LogUser logUser)
        {
            // Create the roles list from LogUser
            var roles = new List<string> { logUser.il_RoleName };

            // Create claims
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, logUser.lg_email),
        new Claim(ClaimTypes.Role, string.Join(",", roles)) 
    };

            // Key for signing the JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            // Return the generated token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

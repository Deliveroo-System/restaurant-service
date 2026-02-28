using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementService.Data;
using RestaurantManagementService.Data.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RestaurantManagementService.Services
{
    public class RestaurantService
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        public RestaurantService(string connectionString, ApplicationDbContext context)
        {
            _context = context;

            _connectionString = connectionString;
        }
        public async Task<IEnumerable<Restaurant>> GetRestaurantsByOwnerEmailAsync(string email, int ownerId)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }

            string sqlQuery = @"
                                SELECT r.*
                                FROM Restaurants r
                                INNER JOIN Users u ON r.OwnerId = u.UserId
                                WHERE u.Email = @Email AND r.OwnerId = @OwnerId";
            
            var parameters = new[]
            {
        new SqlParameter("@Email", email),
        new SqlParameter("@OwnerId", ownerId)
             };

            var restaurants = await _context.Restaurants
                .FromSqlRaw(sqlQuery, parameters)
                .ToListAsync();

            return restaurants;
        }
        public async Task<IActionResult> GetRestaurantMenusAsync(int restaurantId, int userId)
        {
            // Query the RB_RESTAURANTS_MENUS view
            var menus = await _context.RB_RESTAURANTS_MENUS
                .Where(rm => rm.RestaurantId == restaurantId && rm.OwnerId == userId)
                .ToListAsync();

            if (menus == null || !menus.Any())
            {
                return new NotFoundObjectResult("No menus found for this restaurant.");
            }

            return new OkObjectResult(menus);
        }
        public async Task<string> ManageRestaurantAsync(
            string action,
            int? restaurantId,
            int? ownerId,
            int? categoryId,
            string name,
            string description,
            string address,
            string phoneNumber,
            string? email,
            TimeSpan? openingTime,
            TimeSpan? closingTime,
            bool? isApproved,
            bool? isAvailable)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ManageRestaurant", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Action
                    command.Parameters.AddWithValue("@Action", action ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId.HasValue ? (object)restaurantId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@OwnerId", ownerId.HasValue ? (object)ownerId.Value : DBNull.Value); // Check for null
                    command.Parameters.AddWithValue("@CategoryId", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value); // Check for null

                    // Conditional field updates (Only update if provided)
                    command.Parameters.AddWithValue("@Name", string.IsNullOrWhiteSpace(name) ? (object)DBNull.Value : name.Trim());
                    command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description.Trim());
                    command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address.Trim());
                    command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(phoneNumber) ? (object)DBNull.Value : phoneNumber.Trim());
                    command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email.Trim());

                    // Time fields (Only update if provided)
                    command.Parameters.AddWithValue("@OpeningTime", openingTime.HasValue ? (object)openingTime.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@ClosingTime", closingTime.HasValue ? (object)closingTime.Value : DBNull.Value);

                    // Boolean fields (Only update if provided)
                    command.Parameters.AddWithValue("@IsApproved", isApproved.HasValue ? (object)isApproved.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@IsAvailable", isAvailable.HasValue ? (object)isAvailable.Value : DBNull.Value);

                    try
                    {
                        var result = await command.ExecuteNonQueryAsync();

                        if (result > 0)
                        {
                            return "Operation completed successfully.";
                        }
                        else
                        {
                            return "No rows affected.";
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"An error occurred: {ex.Message}";
                    }
                }
            }
        }


    }
}

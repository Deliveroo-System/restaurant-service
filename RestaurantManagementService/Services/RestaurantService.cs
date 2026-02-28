using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RestaurantManagementService.Services
{
    public class RestaurantService
    {
        private readonly string _connectionString;

        public RestaurantService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> ManageRestaurantAsync(string action, int? restaurantId, int ownerId, int categoryId, string name, string description, string address, string phoneNumber, string email, TimeSpan openingTime, TimeSpan closingTime, bool isApproved, bool isAvailable)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ManageRestaurant", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Action", action);
                    command.Parameters.AddWithValue("@RestaurantId", (object)restaurantId ?? DBNull.Value); 
                    command.Parameters.AddWithValue("@OwnerId", ownerId);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@OpeningTime", openingTime);
                    command.Parameters.AddWithValue("@ClosingTime", closingTime);
                    command.Parameters.AddWithValue("@IsApproved", isApproved);
                    command.Parameters.AddWithValue("@IsAvailable", isAvailable);

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

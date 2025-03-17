using BCrypt.Net;
using Microsoft.Data.SqlClient;
using System.Data;


public class UserService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
    private readonly string _connectionString;

    public UserService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public UserService()
    {
    }

    public async Task RegisterUserAsync(string firstName, string lastName, string email, string phoneNumber, string passwordHash, int roleId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("RegisterUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Adding parameters
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@RoleId", roleId);

                try
                {
                    await command.ExecuteNonQueryAsync(); // Execute the procedure
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    throw new Exception("An error occurred while registering the user", ex);
                }
            }
        }
    }
}

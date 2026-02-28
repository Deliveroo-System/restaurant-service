using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagementService.Data;
using RestaurantManagementService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add UserService to DI container
builder.Services.AddTransient<UserService>(); // Register UserService

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Disable HTTPS metadata requirement only in development
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;

        // Configure token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Validate the issuer
            ValidateAudience = true, // Validate the audience
            ValidateLifetime = true, // Validate the token expiration
            ValidateIssuerSigningKey = true, // Validate the signing key
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Issuer from configuration
            ValidAudience = builder.Configuration["Jwt:Audience"], // Audience from configuration
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Signing key from configuration
        };
    });

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .Options);
// Register RestaurantService (singleton since it's stateless)
builder.Services.AddSingleton(new RestaurantService(builder.Configuration.GetConnectionString("DefaultConnection"), context));

// Register JwtService for JWT handling
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();  // Add authentication middleware
app.UseAuthorization();

app.MapControllers();

app.Run();

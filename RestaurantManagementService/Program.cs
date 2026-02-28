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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) ,// Signing key from configuration
             RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" // IMPORTANT!
        };
    });

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                  .AllowAnyMethod()  
                  .AllowAnyHeader()  
                  .AllowCredentials(); // Allow cookies/authentication
        });
});

var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .Options);
// Register RestaurantService (singleton since it's stateless)
builder.Services.AddSingleton(new RestaurantService(builder.Configuration.GetConnectionString("DefaultConnection"), context));

// Register JwtService for JWT handling
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IMenuService, MenuService>();

var app = builder.Build();

//app.Urls.Add("http://localhost:8080");
//app.Urls.Add("https://localhost:44397");
//app.Urls.Add("https://localhost:8443");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend"); // Enable CORS for frontend

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();

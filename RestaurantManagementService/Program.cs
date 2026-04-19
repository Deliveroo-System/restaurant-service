using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagementService.Data;
using RestaurantManagementService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Run on 8080
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddTransient<UserService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddControllers();

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
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
builder.Services.AddScoped<UserService>();
// REMOVE HTTPS Redirection (since we are not using HTTPS in the container)
builder.Services.AddHttpsRedirection(options =>
{
    // No need to set a port, we will use HTTP only.
    options.HttpsPort = 0;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

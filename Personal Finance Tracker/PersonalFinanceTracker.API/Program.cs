using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PersonalFinanceTracker.Infrastructure;
using PersonalFinanceTracker.Application.Settings;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Bind Jwt options early so both authentication and services use the same values
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration section is missing.");
if (string.IsNullOrEmpty(jwtOptions.Key))
    throw new InvalidOperationException("JWT Key is missing in configuration.");

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Cần thiết cho Swagger
builder.Services.AddSwaggerGen(); // Bạn nên dùng Swagger thay vì OpenApi thuần để dễ test JWT

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Phải đứng TRƯỚC UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
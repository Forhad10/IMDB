using IMDB.Business.Services;
using IMDB.Data;
using IMDB.Data.Entities;
using IMDB.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (use IMDB.Data.Entities namespace's context name)
builder.Services.AddDbContext<IMDBDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register UnitOfWork & Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<TitleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MovieSearchService>();
builder.Services.AddScoped<ActorService>();
builder.Services.AddScoped<AdvanceSearchService>();
builder.Services.AddScoped<DropdownListService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
if (!string.IsNullOrWhiteSpace(jwtKey))
{
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
                ValidIssuer = string.IsNullOrWhiteSpace(jwtIssuer) ? null : jwtIssuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
                ValidAudience = string.IsNullOrWhiteSpace(jwtAudience) ? null : jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
}


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Use CORS before MapControllers
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseHttpsRedirection();
// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

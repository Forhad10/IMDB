using IMDB.Business.Services;
using IMDB.Data;
using IMDB.Data.Entities;
using IMDB.Data.Repositories;
using Microsoft.EntityFrameworkCore;

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
app.UseAuthorization();
app.MapControllers();
app.Run();

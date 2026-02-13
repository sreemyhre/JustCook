using Microsoft.EntityFrameworkCore;
using RecipeVault.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hi Sam!!! Recipe Vault API is running!");

app.Run();

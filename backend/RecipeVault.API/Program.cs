using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeVault.API.Middleware;
using RecipeVault.Application.Interfaces;
using RecipeVault.Application.Mappings;
using RecipeVault.Application.Services;
using RecipeVault.Core.Interfaces;
using RecipeVault.Infrastructure.Data;
using RecipeVault.Infrastructure.Repositories;
using RecipeVault.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── PORT binding for Railway ───────────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://+:{port}");

// ── Firebase Admin SDK ─────────────────────────────────────────────────────
var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT");
var firebaseFilePath = builder.Configuration["Firebase:ServiceAccountPath"];
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];

GoogleCredential firebaseCredential;
if (!string.IsNullOrEmpty(firebaseJson))
    firebaseCredential = GoogleCredential.FromJson(firebaseJson);
else if (!string.IsNullOrEmpty(firebaseFilePath) && File.Exists(firebaseFilePath))
    firebaseCredential = GoogleCredential.FromFile(firebaseFilePath);
else
    firebaseCredential = GoogleCredential.GetApplicationDefault();

FirebaseApp.Create(new AppOptions
{
    Credential = firebaseCredential,
    ProjectId = firebaseProjectId
});

// ── JWT Authentication ─────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ── CORS ───────────────────────────────────────────────────────────────────
var allowedOrigins = new List<string> { "http://localhost:4200" };
var railwayOrigin = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGIN")
    ?? builder.Configuration["Cors:AllowedOrigin"];
if (!string.IsNullOrEmpty(railwayOrigin))
    allowedOrigins.Add(railwayOrigin);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins([.. allowedOrigins])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ── Controllers & Swagger ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Database ───────────────────────────────────────────────────────────────
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// ── HttpClient for reCAPTCHA ───────────────────────────────────────────────
builder.Services.AddHttpClient("recaptcha");

// ── AutoMapper ─────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// ── Application Services ───────────────────────────────────────────────────
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddScoped<IMealPlanRepository, MealPlanRepository>();
builder.Services.AddScoped<IMealPlanService, MealPlanService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// ── Build ──────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Auto-migrate on startup ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowAngular");
app.UseMiddleware<CookieJwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();

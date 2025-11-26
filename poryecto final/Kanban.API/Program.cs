using Kanban.Application.Services;
using Kanban.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
//  DATABASE (SIEMPRE USAR POSTGRES EN DOCKER)
// ==========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");

    Console.WriteLine("🔵 Using PostgreSQL: " + conn);

    options.UseNpgsql(conn);
});

// ==========================================
//  SERVICES
// ==========================================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaskService>();

// ==========================================
//  JWT CONFIG
// ==========================================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// ==========================================
//  CORS PARA DOCKER + ANGULAR
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularCorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==========================================
//  Controllers
// ==========================================
builder.Services.AddControllers();

var app = builder.Build();

// ==========================================
//  Middleware
// ==========================================

app.UseCors("AngularCorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================================
//  APPLY MIGRATIONS (POSTGRES REAL)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Console.WriteLine("⚙ Applying migrations...");
    db.Database.Migrate();
}

app.Run();

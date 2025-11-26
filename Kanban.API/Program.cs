using Kanban.Application.Services;
using Kanban.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================
//  DATABASE
// ============================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseInMemoryDatabase("KanbanDb");
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// ============================
//  SERVICES
// ============================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaskService>();

// ============================
//  JWT CONFIG
// ============================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "defaultkey";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "defaultissuer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "defaultaudience";

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

// -------------------------------------------
// CORS: crear UNA SOLA política limpia
// -------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ============================
//  CONTROLLERS
// ============================
builder.Services.AddControllers();

var app = builder.Build();

// -------------------------------------------
// APLICAR CORS EN EL ORDEN CORRECTO
// -------------------------------------------

// 1) CORS primero
app.UseCors("AngularCorsPolicy");

// 2) HTTPS (si existe)
app.UseHttpsRedirection();

// 3) Authentication antes de Authorization
app.UseAuthentication();

// 4) Authorization
app.UseAuthorization();

// 5) Controllers
app.MapControllers();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

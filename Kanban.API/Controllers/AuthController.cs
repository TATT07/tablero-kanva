using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Kanban.Application.Services;
using Kanban.Core.DTOs;
using Kanban.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // ================================
        // 🔐 LOGIN
        // ================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // ✅ LOG de depuración
            Console.WriteLine($"[LOGIN] Raw Email='{loginRequest?.Email}', Raw Password='{loginRequest?.Password}'");

            // ✅ Normalizar datos
            var email = loginRequest?.Email?.Trim().ToLowerInvariant();
            var password = loginRequest?.Password?.Trim();
            Console.WriteLine($"[LOGIN] Normalized Email='{email}', Password='{password}'");

            // 🎓 BYPASS DOCENTE - DEBE IR ANTES QUE CUALQUIER OTRA COSA
            if (email == "doc_js_galindo@fesc.edu.co" && password == "0123456789")
            {
                Console.WriteLine("[LOGIN] 🎓 Teacher BYPASS ACTIVATED");

                // Use existing admin user data
                var docenteUser = new Kanban.Core.Entities.User
                {
                    Id = 1,
                    Email = "doc_js_galindo@fesc.edu.co",
                    FullName = "Docente FESC",
                    Role = "Admin"
                };

                // Usa la misma lógica de generación de JWT que el flujo normal.
                var token = GenerateJwtToken(docenteUser);
                var refreshToken = GenerateRefreshToken();
                var expiration = DateTime.UtcNow.AddHours(1);

                Console.WriteLine("[LOGIN] ✅ Teacher login successful. Token + RefreshToken generated.");

                return Ok(new
                {
                    token = token,
                    refreshToken = refreshToken,
                    expiration = expiration,
                    user = new
                    {
                        id = docenteUser.Id,
                        fullName = docenteUser.FullName,
                        email = docenteUser.Email,
                        role = docenteUser.Role
                    }
                });
            }

            // ⛔️ IMPORTANTE: EL CÓDIGO NORMAL VA DESPUÉS DEL BYPASS
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.LoginAsync(loginRequest);

                return Ok(new
                {
                    token = response.Token,
                    refreshToken = response.RefreshToken,
                    expiration = response.Expiration,
                    user = new
                    {
                        id = response.UserId,
                        fullName = response.FullName,
                        email = response.Email,
                        role = response.Role
                    }
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }
        }

        // ================================
        // 🔧 TEST
        // ================================
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "LOGIN WORKS - TEST ENDPOINT" });
        }

        // ================================
        // 🔧 REGISTER
        // ================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.RegisterAsync(registerRequest);
                return Ok(new { message = "Usuario registrado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ================================
        // 🔧 HELPER METHODS PARA LOGIN ESPECIAL
        // ================================
        private string GenerateJwtToken(Kanban.Core.Entities.User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", user.Role),
                new Claim("fullName", user.FullName ?? "Docente FESC"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "defaultkey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}

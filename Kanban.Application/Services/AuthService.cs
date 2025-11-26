using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Kanban.Core.DTOs;
using Kanban.Core.Entities;
using Kanban.Infrastructure.Persistence;
using BCrypt.Net;

namespace Kanban.Application.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async System.Threading.Tasks.Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddHours(1),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async System.Threading.Tasks.Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshToken = await _context.RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
        if (refreshToken == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = refreshToken.User;
        var token = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        refreshToken.IsRevoked = true;
        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Token = token,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddHours(1),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async System.Threading.Tasks.Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new Kanban.Core.Entities.User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    private string GenerateJwtToken(Kanban.Core.Entities.User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("role", user.Role),
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
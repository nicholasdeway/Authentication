using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Autenticação.Models;
using Autenticação.Models.DTOs;

public class AuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IConfiguration config, IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("users");
        _users = database.GetCollection<User>("users");

        _jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (existingUser != null) return null;

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        await _users.InsertOneAsync(user);
        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponse> RefreshAccessTokenAsync(string email, string refreshToken)
    {
        var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            return null;

        return await GenerateTokenAsync(user);
    }

    private async Task<AuthResponse> GenerateTokenAsync(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("firstName", user.FirstName ?? ""),
            new Claim("lastName", user.LastName ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        // Geração do Refresh Token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays);

        // Atualiza no banco
        var update = Builders<User>.Update
            .Set(u => u.RefreshToken, refreshToken)
            .Set(u => u.RefreshTokenExpiry, refreshExpiry);

        await _users.UpdateOneAsync(u => u.Id == user.Id, update);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken
        };
    }
}
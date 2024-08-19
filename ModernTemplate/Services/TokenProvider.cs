using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ModernTemplate.Services;

public class TokenProvider
{
    private readonly IConfiguration _configuration;

    public TokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Create()
    {
        var secret = _configuration["Jwt:Secret"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "John Doe"),
                new Claim(JwtRegisteredClaimNames.Email, "Email")
            ]),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return token;
    }
}

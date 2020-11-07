using CryptoHelper;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace YOP.Services
{
    public class AuthService
    {
        public string GenerateToken(User user, IConfiguration configuration) 
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection("TokenOptions:JwtSecretKey").Value)
            );
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: configuration.GetSection("TokenOptions:Issuer").Value,
                audience: configuration.GetSection("TokenOptions:Audience").Value,
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                },
                expires: DateTime.Now.AddDays(Convert.ToDouble(configuration.GetSection("TokenOptions:LifeTime").Value)),
                signingCredentials: signinCredentials
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
        public string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }
        public bool VerifyPassword(string hash, string password)
        {
            return Crypto.VerifyHashedPassword(hash, password);
        }
    }
}

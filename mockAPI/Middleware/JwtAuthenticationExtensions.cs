using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using mockAPI.Middleware.Setting;

namespace mockAPI.Middleware;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings
        {
            Key = configuration["JWTKey"] ?? throw new ArgumentNullException("JwtKey is not set in configuration"),
            Issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is not set in configuration"),
            Audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience is not set in configuration"),
            ExpirationInMinutes = int.Parse(configuration["Jwt:ExpirationInMinutes"] ?? "60")
        };
         
 


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });


            // 你也要自訂 JwtSettings 的 Key 來源
        services.Configure<JwtSettings>(opts =>
        {
            opts.Key = jwtSettings.Key;
            opts.Issuer = jwtSettings.Issuer;
            opts.Audience = jwtSettings.Audience;
            opts.ExpirationInMinutes = jwtSettings?.ExpirationInMinutes ?? 60;
        });
 


        return services;
    }
}

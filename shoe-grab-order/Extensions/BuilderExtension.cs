﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoeGrabCommonModels;
using ShoeGrabOrderManagement.Clients;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ShoeGrabOrderManagement.Extensions;

public static class BuilderExtension
{
    public static void AddGrpcAndClients(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcSection = configuration.GetSection("GrpcServices");
        var clientCertificate = new X509Certificate2("Resources\\client.pfx", "test123");

        services.AddGrpcClient<UserManagement.UserManagementClient>(options =>
        {
            options.Address = new Uri(grpcSection["UserManagementAddress"]);
        })
        .ConfigurePrimaryHttpMessageHandler(() => ConfigureHandlerUseCertificate(clientCertificate));

        services.AddGrpcClient<ProductManagement.ProductManagementClient>(options =>
        {
            options.Address = new Uri(grpcSection["ProductManagementAddress"]);
        })
        .ConfigurePrimaryHttpMessageHandler(() => ConfigureHandlerUseCertificate(clientCertificate));

        services.AddGrpcClient<CrmService.CrmServiceClient>(options =>
        {
            options.Address = new Uri(grpcSection["CrmServiceAddress"]);
        })
        .ConfigurePrimaryHttpMessageHandler(() => ConfigureHandlerUseCertificate(clientCertificate));
    }
    public static void AddJWTAuthenticationAndAuthorization(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(UserRole.Admin));

            options.AddPolicy("UserOnly", policy =>
                policy.RequireRole(UserRole.User));
        });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
                RoleClaimType = ClaimTypes.Role
            };
        });
        builder.Services.AddAuthorization();
    }

    public static void SetupSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }

    private static HttpClientHandler ConfigureHandlerUseCertificate(X509Certificate2 clientCertificate)
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(clientCertificate);
        return handler;
    }
}

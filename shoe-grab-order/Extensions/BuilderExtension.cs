using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoeGrabCommonModels;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ShoeGrabOrderManagement.Extensions;

public static class BuilderExtension
{
    public static void AddGrpcAndClients(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcSection = configuration.GetSection("GrpcServices");

        var userService = services.AddGrpcClient<UserManagement.UserManagementClient>(options =>
        {
            options.Address = new Uri(grpcSection["UserManagementAddress"]);
        });

        var productService = services.AddGrpcClient<ProductManagement.ProductManagementClient>(options =>
        {
            options.Address = new Uri(grpcSection["ProductManagementAddress"]);
        });

        var crmService = services.AddGrpcClient<CrmService.CrmServiceClient>(options =>
        {
            options.Address = new Uri(grpcSection["CrmServiceAddress"]);
        });

        services.AddGrpc();

        var certificatePath = grpcSection["Certificate:Path"];
        var certificatePassword = grpcSection["Certificate:Password"];

        if (certificatePath != null && certificatePassword != null)
        {
            var clientCertificate = new X509Certificate2("Resources\\client.pfx", "test123");

            crmService.ConfigurePrimaryHttpMessageHandler(() => ConfigureHandlerUseCertificate(clientCertificate));
            productService.ConfigurePrimaryHttpMessageHandler(() => ConfigureHandlerUseCertificate(clientCertificate));
            userService.ConfigurePrimaryHttpMessageHandler(() => ConfigureHandlerUseCertificate(clientCertificate));
        }
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

    public static void SetupKestrel(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            var kestrelSection = context.Configuration.GetSection("Kestrel:Endpoints");

            var grpcEndpoint = kestrelSection.GetSection("Grpc");
            if (grpcEndpoint.Exists())
            {
                var grpcUrl = new Uri(grpcEndpoint["Url"]);
                options.Listen(IPAddress.Any, grpcUrl.Port, listenOptions =>
                {
                    listenOptions.Protocols = Enum.Parse<HttpProtocols>(grpcEndpoint["Protocols"]);
                    listenOptions.UseHttps(httpsOptions =>
                    {
                        var certificatePath = grpcEndpoint["Certificate:Path"];
                        var certificatePassword = grpcEndpoint["Certificate:Password"];
                        var parseSuccess = Enum.TryParse(grpcEndpoint["Certificate:ClientCertificateMode"], out ClientCertificateMode clientCertificateMode);

                        if (certificatePath != null && certificatePassword != null && parseSuccess)
                        {
                            var certificate = new X509Certificate2(certificatePath, certificatePassword);
                            httpsOptions.ServerCertificate = certificate;
                            httpsOptions.ClientCertificateMode = clientCertificateMode;
                        }
                    });
                });
            }

            var restApiEndpoint = kestrelSection.GetSection("RestApi");
            if (restApiEndpoint.Exists())
            {
                var restApiUrl = new Uri(restApiEndpoint["Url"]);
                options.Listen(IPAddress.Any, restApiUrl.Port, listenOptions =>
                {
                    listenOptions.Protocols = Enum.Parse<HttpProtocols>(restApiEndpoint["Protocols"]);
                });
            }
        });
    }

    private static HttpClientHandler ConfigureHandlerUseCertificate(X509Certificate2 clientCertificate)
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(clientCertificate);
        return handler;
    }
}

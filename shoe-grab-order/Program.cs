using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabOrderManagement.Clients;
using ShoeGrabOrderManagement.Database.Mappers;
using ShoeGrabOrderManagement.Extensions;
using ShoeGrabOrderManagement.GrpcServices;
using ShoeGrabOrderManagement.Services;


var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

builder.SetupKestrel();
//Grpc
builder.Services.AddGrpcAndClients(builder.Configuration);
builder.Services.AddScoped<IGrpcClient, GrpcClient>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

//Contexts
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContextPool<OrderContext>(opt =>
        opt.UseNpgsql(
            builder.Configuration.GetConnectionString("DB_CONNECTION_STRING"),
            o => o
                .SetPostgresVersion(17, 0)));
}
else
{
    builder.Services.AddDbContextPool<OrderContext>(opt =>
        opt.UseNpgsql(
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"),
            o => o
                .SetPostgresVersion(17, 0)));
}

//Security
builder.AddJWTAuthenticationAndAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add AutoMapper with all profiles in the assembly
builder.Services.AddAutoMapper(typeof(OrderMappingProfile).Assembly);
////APP PART////
var app = builder.Build();

//Migrations
app.ApplyMigrations();


app.MapGrpcService<OrderManagementService>();
//Security
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAllOrigins");
}

app.MapControllers();

app.Run();
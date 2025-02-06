using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabOrderManagement.Clients;
using ShoeGrabOrderManagement.Controllers;
using ShoeGrabOrderManagement.Database.Mappers;
using ShoeGrabOrderManagement.Extensions;
using ShoeGrabOrderManagement.Services;


var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

//Grpc
builder.Services.AddGrpcAndClients(builder.Configuration);
builder.Services.AddScoped<IGrpcClient, GrpcClient>();
builder.Services.AddScoped<IOrderService, OrderService>();
//Swagger
builder.Services.SetupSwagger();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

//Contexts
builder.Services.AddDbContextPool<OrderContext>(opt =>
  opt.UseNpgsql(
    builder.Configuration.GetConnectionString("PostgreSQL"),
    o => o
      .SetPostgresVersion(17, 0)));

//Security
builder.AddJWTAuthenticationAndAuthorization();

// Add AutoMapper with all profiles in the assembly
builder.Services.AddAutoMapper(typeof(OrderMappingProfile).Assembly);
////APP PART////
var app = builder.Build();

//Migrations
app.ApplyMigrations();

//Security
app.UseAuthentication();
app.UseAuthorization();

//Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
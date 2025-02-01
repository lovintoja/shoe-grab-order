using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoeGrabCommonModels;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabOrderManagement.Clients;
using ShoeGrabOrderManagement.Controllers;
using ShoeGrabOrderManagement.Dto;
using System.Security.Claims;

namespace ShoeGrabTests;

public class OrderManagementControllerTests : IDisposable
{
    private readonly Mock<OrderContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderManagementController _controller;
    private readonly UserContextMockHelper _mockHelper;
    private readonly Mock<IGrpcClient> _mockGrpcClient;

    public OrderManagementControllerTests()
    {
        _mockHelper = new UserContextMockHelper();
        _mockContext = _mockHelper.CreateMockContext();
        _mockMapper = new Mock<IMapper>();
        _mockGrpcClient = new Mock<IGrpcClient>();

        _controller = new OrderManagementController(
            _mockContext.Object,
            _mockMapper.Object,
            _mockGrpcClient.Object
        );

        SetupDefaultMappings();
    }

    private void SetupDefaultMappings()
    {
        _mockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
            .Returns((Order src) => new OrderDto());
    }

    private void SetAuthenticatedUser(int userId, string role = UserRole.User)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Authentication, userId.ToString()),
            new(ClaimTypes.Role, role)
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };
    }

    public void Dispose() => _mockContext.Object.Dispose();

    [Fact]
    public async Task CreateOrder_ValidRequest_ReturnsOrderDto()
    {
        // Arrange
        SetAuthenticatedUser(1);
        var product = new Product { Id = 1, Price = 100 };
        _mockHelper.Products.Add(product);

        var request = new CreateOrderDto
        {
            Items = new List<OrderItemCreateDto> { new() { ProductId = 1, Quantity = 2 } }
        };
        var user = new User
        {
            Id = 1,
            Email = "test",
            Username = "test"
        };
        _mockMapper.Setup(x => x.Map<Order>(It.IsAny<CreateOrderDto>())).Returns(new Order());
        _mockMapper.Setup(x => x.Map<OrderDto>(It.IsAny<Order>())).Returns(new OrderDto { TotalAmount = 200 });
        _mockHelper.Users.Add(user);
        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<OrderDto>(okResult.Value);
        Assert.Equal(200, dto.TotalAmount);
    }

    [Fact]
    public async Task GetOrder_ExistingOrder_ReturnsOrderDetails()
    {
        // Arrange
        var order = new Order { Id = 1, UserId = 1 };
        _mockHelper.Orders.Add(order);
        _mockMapper.Setup(x => x.Map<OrderDetailsDto>(It.IsAny<Order>())).Returns(new OrderDetailsDto());

        // Act
        var result = await _controller.GetOrder(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<OrderDetailsDto>(okResult.Value);
    }

    [Fact]
    public async Task UpdateOrder_UnauthorizedUser_ReturnsForbid()
    {
        // Arrange
        SetAuthenticatedUser(2);
        var order = new Order { Id = 1, UserId = 1 };
        _mockContext.Setup(x => x.Orders.FindAsync(order.Id)).Returns(new ValueTask<Order>(Task.FromResult(order)));

        // Act
        var result = await _controller.UpdateOrder(1, new UpdateOrderDto());

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task CancelOrder_AdminUser_AllowsCancellation()
    {
        // Arrange
        SetAuthenticatedUser(999, UserRole.Admin);
        var order = new Order { Id = 1, UserId = 1, Status = OrderStatus.New };
        _mockContext.Setup(x => x.Orders.FindAsync(order.Id)).Returns(new ValueTask<Order>(Task.FromResult(order)));

        // Act
        var result = await _controller.CancelOrder(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }
}
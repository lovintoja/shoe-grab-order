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
}
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;
using ShoeGrabOrderManagement.Dto;
using ShoeGrabOrderManagement.Services;
using System.Security.Claims;

namespace ShoeGrabOrderManagement.Controllers;

[Route("api/order")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;
    private readonly IMapper _mapper;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger, IMapper mapper)
    {
        _orderService = orderService;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto request)
    {
        var userId = GetUserId();

        if (!userId.HasValue)
            return Unauthorized();

        var order = _mapper.Map<Order>(request);

        var addedOrder = await _orderService.CreateOrder(userId.Value, order);
        
        var response = _mapper.Map<OrderDto>(addedOrder);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var userId = GetUserId();

        if (!userId.HasValue)
            return Unauthorized();

        var orders = await _orderService.GetOrders(userId.Value);
        var mappedOrders = orders.Select(o => _mapper.Map<OrderDto>(o)).ToList();

        return Ok(mappedOrders);
    }

    private int? GetUserId()
    {
        if (User.Identity?.IsAuthenticated is not null && User.Identity.IsAuthenticated)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Authentication);
            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }
        }
        return null;
    }
}

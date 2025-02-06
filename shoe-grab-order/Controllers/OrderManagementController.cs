using ShoeGrabCommonModels.Contexts;
using ShoeGrabOrderManagement.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;
using System.Security.Claims;
using AutoMapper;
using ShoeGrabOrderManagement.Clients;

namespace ShoeGrabOrderManagement.Controllers;

[Route("api/admin/order")]
[Authorize(Roles = UserRole.Admin)]
public class OrderManagementController : ControllerBase
{
    private readonly OrderContext _context;
    private readonly IMapper _mapper;
    private readonly IGrpcClient _grpcClient;

    public OrderManagementController(OrderContext context, IMapper mapper, IGrpcClient grpcClient)
    {
        _context = context;
        _mapper = mapper;
        _grpcClient = grpcClient;
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return NotFound();

        try
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong during order delete operation.");
        }
        return Ok();
    }

    [HttpPut]
    [Route("status")]
    public async Task<IActionResult> ChangeOrderStatus(ChangeOrderStatusDto request)
    {
        var order = await _context.Orders.FindAsync(request.OrderId);
        if (order == null)
            return NotFound();

        if (!Enum.TryParse(typeof(OrderStatus), request.OrderStatus, out var status))
            return BadRequest("Wrong status format.");
        var mappedStatus = (OrderStatus)status;
        order.Status = mappedStatus;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong when updating order status.");
        }

        return Ok();
    }

    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<AdminOrderDto>>> SearchOrders(
        [FromQuery] AdminOrderSearchQuery query)
    {
        var dbQuery = _context.Orders
            .AsQueryable();

        if (query.UserId.HasValue)
            dbQuery = dbQuery.Where(o => o.UserId == query.UserId.Value);

        if (query.StartDate.HasValue)
            dbQuery = dbQuery.Where(o => o.OrderDate >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            dbQuery = dbQuery.Where(o => o.OrderDate <= query.EndDate.Value);

        if (!string.IsNullOrEmpty(query.Status) && Enum.TryParse(typeof(OrderStatus), query.Status, out var status))
        {
            var mappedStatus = (OrderStatus)status;
            dbQuery = dbQuery.Where(o => o.Status == mappedStatus);
        }

        var orders = await dbQuery
            .OrderByDescending(o => o.OrderDate)
            .Include(o => o.Items)
            .ToListAsync();

        var ordersDto = new List<AdminOrderDto>();
        foreach (var order in orders)
        {
            var orderDto = new AdminOrderDto { Order = _mapper.Map<OrderDto>(order) };
            var user = await _grpcClient.GetUser(order.UserId);
            orderDto.UserId = user.Id;
            orderDto.Username = user.Username;
            ordersDto.Add(orderDto);
        }

        return Ok(ordersDto);
    }
}
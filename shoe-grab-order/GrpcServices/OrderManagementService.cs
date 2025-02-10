using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabOrderManagement.Dto;

namespace ShoeGrabOrderManagement.GrpcServices;

public class OrderManagementService : OrderManagement.OrderManagementBase
{
    private readonly IMapper _mapper;
    private readonly OrderContext _context;

    public OrderManagementService(IMapper mapper, OrderContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public override async Task<DeleteOrderResponse> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
    {
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
            return new DeleteOrderResponse { Success = false };

        try
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return new DeleteOrderResponse { Success = true };
        }
        catch (Exception)
        {
            return new DeleteOrderResponse { Success = false };
        }
    }

    public override async Task<ChangeOrderStatusResponse> ChangeOrderStatus(ChangeOrderStatusRequest request, ServerCallContext context)
    {
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
            return new ChangeOrderStatusResponse { Success = false };

        if (!Enum.TryParse(typeof(OrderStatus), request.Status, out var status))
            return new ChangeOrderStatusResponse { Success = false };

        try
        {
            var mappedStatus = (OrderStatus)status;
            order.Status = mappedStatus;
            await _context.SaveChangesAsync();
            return new ChangeOrderStatusResponse { Success = true };
        }
        catch (Exception)
        {
            return new ChangeOrderStatusResponse { Success = false };
        }
    }

    public override async Task<SearchOrdersResponse> SearchOrders(SearchOrdersRequest request, ServerCallContext context)
    {
        var dbQuery = _context.Orders
            .AsQueryable();

        if (request.UserId != -1)
            dbQuery = dbQuery.Where(o => o.UserId == request.UserId);

        dbQuery = dbQuery.Where(o => o.OrderDate >= request.StartDate.ToDateTime());
        dbQuery = dbQuery.Where(o => o.OrderDate <= request.EndDate.ToDateTime());

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse(typeof(OrderStatus), request.Status, out var status))
        {
            var mappedStatus = (OrderStatus)status;
            dbQuery = dbQuery.Where(o => o.Status == mappedStatus);
        }

        var orders = await dbQuery
            .OrderByDescending(o => o.OrderDate)
            .Include(o => o.Items)
            .ToListAsync();
        var response = new SearchOrdersResponse();
        response.Orders.AddRange(orders.Select(o => _mapper.Map<OrderProto>(o)).ToList());

        return response;
    }
}

namespace ShoeGrabOrderManagement.Dto;

public class AdminOrderDto
{
    public OrderDto Order { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}

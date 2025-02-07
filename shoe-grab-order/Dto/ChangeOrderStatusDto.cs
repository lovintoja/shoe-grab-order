namespace ShoeGrabOrderManagement.Dto;

public class ChangeOrderStatusDto
{
    public int OrderId { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
}

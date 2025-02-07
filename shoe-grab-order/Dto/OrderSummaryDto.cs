namespace ShoeGrabOrderManagement.Dto;
public class OrderSummaryDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}

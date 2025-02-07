namespace ShoeGrabOrderManagement.Dto;
public class AdminOrderSearchQuery
{
    public int? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal? MinTotalPrice { get; set; }
    public decimal? MaxTotalPrice { get; set; }
}

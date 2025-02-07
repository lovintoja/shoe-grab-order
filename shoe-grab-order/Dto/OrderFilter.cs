namespace ShoeGrabOrderManagement.Dto;
public class OrderFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public decimal? MinTotalPrice { get; set; }
    public decimal? MaxTotalPrice { get; set; }
}

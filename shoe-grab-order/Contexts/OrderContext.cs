using Microsoft.EntityFrameworkCore;

namespace ShoeGrabCommonModels.Contexts;

public class OrderContext : DbContext
{
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(order =>
        {
            order.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            order.OwnsOne(o => o.ShippingAddress);
            order.OwnsOne(o => o.PaymentInfo);
        });

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);
    }
}

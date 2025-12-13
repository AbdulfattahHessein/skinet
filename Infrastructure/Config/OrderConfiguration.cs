using System;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.ShippingAddress, o => o.WithOwner());
        builder.OwnsOne(x => x.PaymentSummary, o => o.WithOwner());

        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        AddCheckConstraint(builder);

        builder.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
        builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder
            .Property(x => x.OrderDate)
            .HasConversion(
                d => d.ToUniversalTime(),
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
            );
    }

    private static void AddCheckConstraint(EntityTypeBuilder<Order> builder)
    {
        // Get all valid string names from the enum
        var validStatusNames = Enum.GetNames<OrderStatus>().Select(s => $"'{s}'"); // Quote each name for the SQL IN clause

        var sqlConstraint = $"[{nameof(Order.Status)}] IN ({string.Join(", ", validStatusNames)})";
        // This is the key line to restore database-level safety
        builder.ToTable(t => t.HasCheckConstraint("CK_Order_Status_Valid", sqlConstraint));
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(x => x.ItemOrdered, o => o.WithOwner());

        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    }
}

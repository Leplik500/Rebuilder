using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PublicWorkout.Domain.Aggregate;

namespace PublicWorkout.Application.Database.EntityConfiguration;

public class OrderDbConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId") // Using a shadow property for communication
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(o => o.Taxes)
            .WithOne()
            .HasForeignKey("OrderId") // Using a shadow property for communication
            .OnDelete(DeleteBehavior.Cascade);
    }
}

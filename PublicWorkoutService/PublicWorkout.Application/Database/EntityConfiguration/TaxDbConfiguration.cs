using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PublicWorkout.Domain.Entity;

namespace PublicWorkout.Application.Database.EntityConfiguration;

public class TaxDbConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.Property<Guid>("OrderId"); // Defining a shadow property
    }
}

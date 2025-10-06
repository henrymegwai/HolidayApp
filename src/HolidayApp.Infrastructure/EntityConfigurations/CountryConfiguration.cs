using HolidayApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HolidayApp.Infrastructure.EntityConfigurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CountryCode)
            .IsRequired()
            .HasMaxLength(2);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(e => e.CountryCode)
            .IsUnique();
    }
}
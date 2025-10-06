using HolidayApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HolidayApp.Infrastructure.EntityConfigurations;

public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.LocalName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.GlobalName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.Date)
            .IsRequired();
        builder.Property(e => e.Year)
            .IsRequired();
        builder.Property(e => e.IsWeekend)
            .IsRequired();
        
        builder.HasIndex(e => new { e.CountryId, e.Date });
        builder.HasIndex(e => new { e.Year, e.IsWeekend });
        builder.HasIndex(e => e.Date);
        
        builder.HasOne(e => e.Country)
            .WithMany(c => c.Holidays)
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
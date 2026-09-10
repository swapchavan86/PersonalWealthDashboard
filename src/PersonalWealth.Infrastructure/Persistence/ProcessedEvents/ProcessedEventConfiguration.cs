using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PersonalWealth.Infrastructure.Persistence.ProcessedEvents;

public sealed class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.HasKey(x => x.EventId);
        builder.Property(x => x.EventId).ValueGeneratedNever();
        builder.Property(x => x.EventType).HasMaxLength(512).IsRequired();
        builder.Property(x => x.ProcessedAtUtc).IsRequired();
    }
}

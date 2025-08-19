using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;

public class ActivityTypeEntityConfiguration
    : IEntityTypeConfiguration<Domain.AggregatesModel.ActivityTypeAggregate.ActivityType>
{
    public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ActivityTypeAggregate.ActivityType> e)
    {
        e.ToTable("activity_type");
        e.Ignore(b => b.DomainEvents);

        e.HasKey(o => o.Id);
        
        e.Property(o => o.Id)
            .HasColumnName("activity_type_id")
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        e.Property(r => r.Name)
            .HasColumnName("name")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();
        
        e.Property(x => x.GC)
            .HasColumnName("gc")
            .IsRequired(false)
            .HasDefaultValue(null);
    }
}
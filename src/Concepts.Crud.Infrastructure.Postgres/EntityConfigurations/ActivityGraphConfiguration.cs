#if false
using Concepts.Crud.Domain.AggregatesModel.ActivityGraphAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;

public class ActivityGraphConfiguration
    : IEntityTypeConfiguration<ActivityGraph>
{
    public void Configure(EntityTypeBuilder<ActivityGraph> e)
    {
        e.ToTable("activity_graph");
        e.Ignore(b => b.DomainEvents);

        e.HasKey(o => o.Id);

        e.Property(o => o.Id)
            .HasColumnName("activity_graph_id")
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        e.Property(r => r.Code)
            .HasColumnName("code")
            .IsUnicode()
            .HasMaxLength(128);

        e.Property(r => r.Type)
            .HasColumnName("type")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();

        e.Property(r => r.Name)
            .HasColumnName("name")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();

        e.Property(r => r.IsGroup)
            .HasColumnName("is_group");

        e.Property(x => x.GC)
            .HasColumnName("gc")
            .IsRequired(false)
            .HasDefaultValue(null);
    }
}
#endif
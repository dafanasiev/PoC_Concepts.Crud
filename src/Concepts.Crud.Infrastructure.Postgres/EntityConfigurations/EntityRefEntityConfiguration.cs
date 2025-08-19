using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;

public class EntityRefEntityConfiguration
    : IEntityTypeConfiguration<EntityRef>
{
    public void Configure(EntityTypeBuilder<EntityRef> e)
    {
        e.ToTable("entity_ref");
        e.Ignore(b => b.DomainEvents);

        e.HasKey(o => o.Id);

        e.Property(o => o.Id)
            .HasColumnName("entity_ref_id")
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        e.Property(r => r.Name)
            .HasColumnName("name")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();

        e.Property(r => r.ReferredClassName)
            .HasColumnName("referred_class_name")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();
        
        e.Property(x => x.GC)
            .HasColumnName("gc")
            .IsRequired(false)
            .HasDefaultValue(null);
    }
}
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;

public class RelationshipTypeEntityConfiguration
    : IEntityTypeConfiguration<RelationshipType>
{
    public void Configure(EntityTypeBuilder<RelationshipType> e)
    {
        e.ToTable("relationship_type");
        e.Ignore(b => b.DomainEvents);

        e.HasKey(o=>o.Id);

        e.Property(o => o.Id)
            .HasColumnName("relationship_type_id")
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        e.Property(r => r.Name)
            .HasColumnName("name")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();

        e.Property(r => r.Description)
            .HasColumnName("description")
            .IsUnicode()
            .HasColumnType("text");
        
        e.Property(x => x.GC)
            .HasColumnName("gc")
            .IsRequired(false)
            .HasDefaultValue(null);
    }
}
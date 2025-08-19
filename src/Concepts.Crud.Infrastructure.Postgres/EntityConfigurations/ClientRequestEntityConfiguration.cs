using Concepts.Crud.Domain.AggregatesModel.ClientRequestAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;

public class ClientRequestEntityConfiguration
    : IEntityTypeConfiguration<ClientRequest>
{
    public void Configure(EntityTypeBuilder<ClientRequest> e)
    {
        e.ToTable("client_request");
        e.Ignore(b => b.DomainEvents);

        e.HasKey(o => o.Id);

        e.Property(o => o.Id)
            .HasColumnName("client_request_id")
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        e.Property(r => r.Time)
            .HasColumnName("time")
            .HasColumnType("timestamptz")
            .IsRequired();

        e.Property(r => r.Name)
            .HasColumnName("name")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();

        e.Property(r => r.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb")
            .HasDefaultValue(null)
            .IsRequired(false);
    }
}
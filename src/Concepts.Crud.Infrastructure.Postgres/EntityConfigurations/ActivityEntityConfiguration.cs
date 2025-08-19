using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;

public class ActivityEntityConfiguration
    : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> e)
    {
        e.ToTable("activity");
        e.Ignore(b => b.DomainEvents);

        e.HasKey(o => o.Id);

        e.Property(o => o.Id)
            .HasColumnName("activity_id")
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        e.Property(r => r.Code)
            .HasColumnName("code")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();

        e.Property(r => r.Name)
            .HasColumnName("name")
            .IsUnicode()
            .HasMaxLength(128)
            .IsRequired();

        e.Property(r => r.Description)
            .HasColumnName("description")
            .IsUnicode()
            .HasColumnType("text");

        e.Property(r => r.IsGroup)
            .HasColumnName("is_group");

        e.Property(x => x.TypeId)
            .HasColumnName("activity_type_id");

        e.Property(x => x.GC)
            .HasColumnName("gc")
            .IsRequired(false)
            .HasDefaultValue(null);

        e.HasOne(r => r.Type)
            .WithMany(r => r.ActivityList)
            .HasPrincipalKey(x => x.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull);

        e.HasOne(x => x.Type)
            .WithMany(x => x.ActivityList)
            .HasForeignKey(x => x.TypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
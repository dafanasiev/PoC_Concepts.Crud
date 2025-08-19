using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;

public class ActivityRelationshipEntityConfiguration
    : IEntityTypeConfiguration<ActivityRelationship>
{
    public void Configure(EntityTypeBuilder<ActivityRelationship> e)
    {
        e.ToTable("activity_relationship");
        e.Ignore(b => b.DomainEvents);

        e.HasKey(o => o.Id);

        e.Property(o => o.Id)
            .HasColumnName("activity_relationship_id")
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        e.HasOne(x => x.Activity)
            .WithMany(x => x.RelationshipList)
            .HasPrincipalKey(x => x.Id);
        e.HasOne(x => x.Activity)
            .WithMany(x => x.RelationshipList)
            .HasForeignKey(x => x.ActivityId);
        e.Property(x => x.ActivityId)
            .HasColumnName("activity_id");


        e.HasOne(x => x.RelationshipType)
            .WithMany(x => x.ActivityRelationshipList)
            .HasPrincipalKey(x => x.Id);
        e.HasOne(x => x.RelationshipType)
            .WithMany(x => x.ActivityRelationshipList)
            .HasForeignKey(x => x.RelationshipTypeId);
        e.Property(x => x.RelationshipTypeId)
            .HasColumnName("relationship_type_id");

        e.HasOne(x => x.EntityRef)
            .WithMany(x => x.ActivityRelationshipList)
            .HasPrincipalKey(x => x.Id);
        e.HasOne(x => x.EntityRef)
            .WithMany(x => x.ActivityRelationshipList)
            .HasForeignKey(x => x.EntityRefId);
        e.Property(x => x.EntityRefId)
            .HasColumnName("entity_ref_id");

        e.Property(x => x.GC)
            .HasColumnName("gc")
            .IsRequired(false)
            .HasDefaultValue(null);
#if false
        e.HasOne(x=>x.ActivityGraph)
            .WithMany(x=>x.ActivityRelationshipList)
            .HasPrincipalKey(x=>x.Id);
        e.HasOne(x => x.ActivityGraph)
            .WithMany(x => x.ActivityRelationshipList)
            .HasForeignKey(x => x.ActivityGraphId);
        e.Property(x=>x.ActivityGraphId)
            .HasColumnName("activity_graph_id");
#endif
    }
}
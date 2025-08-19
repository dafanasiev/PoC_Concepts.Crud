using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;

public class RelationshipType
    : Entity<Guid>, IAggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public virtual ICollection<ActivityRelationship> ActivityRelationshipList { get; private set; }

    protected RelationshipType()
    {
        ActivityRelationshipList = new List<ActivityRelationship>();
    }

    public RelationshipType(string name, string? description)
        :this()
    {
        Name = name;
        Description = description;
    }
}
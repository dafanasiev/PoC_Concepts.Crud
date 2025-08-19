using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;

public class EntityRef
    : Entity<Guid>, IAggregateRoot
{
    public string ReferredClassName { get; private set; }
    public string Name { get; private set; }


    protected EntityRef()
    {
        ActivityRelationshipList = new List<ActivityRelationship>();
    }

    public EntityRef(string name, string referredClassName)
        : this()
    {
        Name = name;
        ReferredClassName = referredClassName;
    }

    public virtual ICollection<ActivityRelationship> ActivityRelationshipList { get; init; }
}
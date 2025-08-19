#if false
using System;
using System.Collections.Generic;
using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Domain.AggregatesModel.ActivityGraphAggregate;

public class ActivityGraph
    : Entity<Guid>, IAggregateRoot
{
    public string Code { get; private set; }
    public string Type { get; private set; }
    public string Name { get; private set; }
    public bool IsGroup { get; private set; }
    public virtual ICollection<ActivityRelationship> ActivityRelationshipList { get; init; }

    protected ActivityGraph()
    {
        ActivityRelationshipList = new List<ActivityRelationship>();
    }

    public ActivityGraph(string name, string type, string code, bool isGroup)
        : this()
    {
        Name = name;
        Type = type;
        Code = code;
        IsGroup = isGroup;
    }
}
#endif
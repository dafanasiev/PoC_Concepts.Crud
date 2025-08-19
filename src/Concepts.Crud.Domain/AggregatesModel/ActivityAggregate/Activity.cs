using System.ComponentModel;
using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.Events;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;

public class Activity
    : Entity<Guid>, IAggregateRoot, IEditableObject
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsGroup { get; private set; }

    public virtual Guid TypeId { get; private set; }
    public virtual ActivityType Type { get; private set; }

    public virtual ICollection<ActivityRelationship> RelationshipList { get; init; }

    protected Activity()
    {
        RelationshipList = new List<ActivityRelationship>();
    }

    public Activity(string name, ActivityType type, string code, string? description, bool isGroup)
        : this()
    {
        Name = name;

        Type = type;
        TypeId = type.Id;

        Code = code;
        Description = description;
        IsGroup = isGroup;
    }

    public override void Delete()
    {
        base.Delete();
        AddDomainEvent(new ActivityDeletedDomainEvent(this));
    }


    public void BeginEdit()
    {
        //TODO: save current_state
    }

    public void CancelEdit()
    {
        //TODO: set current_state=null
    }

    public void EndEdit()
    {
        //TODO: check current_state and addDomainEvent ONLY when changes detected
        AddDomainEvent(new ActivityEditedDomainEvent(this));
    }

    public void SetCode(string code)
    {
        Code = code;
    }

    public void SetType(ActivityType type)
    {
        Type = type;
        TypeId = type.Id;
    }

    public void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        Name = name;
    }

    public void SetDescription(string? aDescription)
    {
        Description = aDescription;
    }

    public void SetIsGroup(bool aIsGroup)
    {
        IsGroup = aIsGroup;
    }

    public void SetRelationshipList(IEnumerable<SetRelationshipListArgItem> list)
    {
        //TODO: bad perfomance
        var existsList = new List<ActivityRelationship>(RelationshipList.Count);
        foreach (var i in list)
        {
            foreach (var ei in RelationshipList)
            {
                if (ei.RelationshipTypeId == i.RelationshipType.Id && ei.EntityRefId == i.TargetSpecification.Id)
                {
                    existsList.Add(ei);
                    goto exists;
                }
            }

            // not exists
            var newItem = new ActivityRelationship(this, i.RelationshipType, i.TargetSpecification);
            RelationshipList.Add(newItem);
            existsList.Add(newItem);

            exists: ;
        }

        // cleanup no more exists
        foreach (var i in RelationshipList.ToList())
        {
            foreach (var ei in existsList)
            {
                if (i == ei) goto exists;
            }

            RelationshipList.Remove(i); //TODO: fixme (GCProblem)
            exists: ;
        }
    }

    public class SetRelationshipListArgItem
    {
        public RelationshipType RelationshipType { get; set; }
        public EntityRef TargetSpecification { get; set; } //TOOD: bad spec, targetSpecification.referredClassName?
    }
}
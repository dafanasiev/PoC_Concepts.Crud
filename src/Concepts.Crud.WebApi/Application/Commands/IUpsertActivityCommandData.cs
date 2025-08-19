using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;

namespace Concepts.Crud.WebApi.Application.Commands;

public interface IUpsertActivityCommandData
{
        public string Code { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsGroup { get; }
        public IType Type { get; }
        public IEnumerable<IRelationship> Relationship { get; }

        public interface IType
        {
            public Guid Id { get; }
        }

        public interface IRelationship
        {
            public IRelationshipType RelationshipType { get; }
            public ITargetSpecification TargetSpecification { get; }

            public interface ITargetSpecification
            {
                public Guid Id { get; }
                public string ReferredClassName { get; }
            }

            public interface IRelationshipType
            {
                public Guid Id { get; }
            }
        }
    

    // internal async Task<ICollection<Activity>>
    //     ToActivityList(
    //         IActivityTypeRepository activityTypeRepository,
    //         IRelationshipTypeRepository relationshipTypeRepository,
    //         IEntityRefRepository entityRefRepository,
    //         CancellationToken ct
    //     )
    // {
    //     ICollection<Activity> activitiesAggList =
    //         new List<Activity>();
    //     foreach (var activity in this)
    //     {
    //         var a = await ToActivity(activityTypeRepository, relationshipTypeRepository, entityRefRepository, ct, activity);
    //         activitiesAggList.Add(a);
    //     }
    //
    //     return activitiesAggList;
    // }

    internal async Task<Activity> ToActivity(
        IActivityTypeRepository activityTypeRepository,
        IRelationshipTypeRepository relationshipTypeRepository,
        IEntityRefRepository entityRefRepository,
        CancellationToken ct
        )
    {
        var at = await activityTypeRepository.FindById(Type.Id, ct);
        if (at == null)
        {
            //TODO: better exceptions
            throw new Exception($"Activity type with id {Type.Id} not found");
        }

        var a = new Activity(
            name: Name,
            code: Code,
            description: Description,
            isGroup: IsGroup,
            type: at
        );
        foreach (var relationShip in Relationship)
        {
            var rst = await relationshipTypeRepository.FindById(relationShip.RelationshipType.Id, ct);
            if (rst == null)
            {
                //TODO: better exceptions?
                throw new Exception($"Relationship type with id {relationShip.RelationshipType.Id} not found");
            }

            var er = await entityRefRepository.FindById(relationShip.TargetSpecification.Id, ct);
            if (er == null)
            {
                throw new Exception($"EntityRef with id {relationShip.TargetSpecification.Id} not found");
            }

            var rs = new ActivityRelationship(
                activity: a,
                relationshipType: rst,
                entityRef: er
            );
            a.RelationshipList.Add(rs);
        }

        return a;
    }
}
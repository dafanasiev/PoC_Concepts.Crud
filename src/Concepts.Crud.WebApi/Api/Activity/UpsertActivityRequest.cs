using System.Runtime.Serialization;
using Concepts.Crud.WebApi.Application.Commands;

namespace Concepts.Crud.WebApi.Api.Activity;

public /*abstract*/ class UpsertActivityRequest : IUpsertActivityCommandData
{
    [DataMember(Name = "code")] [Required] public required string Code { get; init; }

    [DataMember(Name = "type")] [Required] public TType Type { get; init; }
    IUpsertActivityCommandData.IType IUpsertActivityCommandData.Type => Type;

    [DataMember(Name = "name")] [Required] public required string Name { get; init; }

    [DataMember(Name = "description")]
    [Required]
    public required string Description { get; init; }

    [DataMember(Name = "isGroup")]
    [Required]
    public required bool IsGroup { get; init; }

    [DataMember(Name = "relationship")]
    [Required]
    public required List<TRelationship> Relationship { get; init; }

    IEnumerable<IUpsertActivityCommandData.IRelationship> IUpsertActivityCommandData.Relationship => Relationship;

    public class TRelationship : IUpsertActivityCommandData.IRelationship
    {
        [DataMember(Name = "relationshipType")]
        [Required]
        public required TRelationshipType RelationshipType { get; init; }

        IUpsertActivityCommandData.IRelationship.IRelationshipType IUpsertActivityCommandData.IRelationship.RelationshipType => RelationshipType;

        [DataMember(Name = "targetSpecification")]
        [Required]
        public required TTargetSpecification TargetSpecification { get; init; }

        IUpsertActivityCommandData.IRelationship.ITargetSpecification IUpsertActivityCommandData.IRelationship.TargetSpecification => TargetSpecification;

        public class TTargetSpecification : IUpsertActivityCommandData.IRelationship.ITargetSpecification
        {
            [DataMember(Name = "id")] [Required] public required Guid Id { get; init; }

            [DataMember(Name = "referredClassName")]
            [Required]
            public required string ReferredClassName { get; init; }
        }

        public class TRelationshipType : IUpsertActivityCommandData.IRelationship.IRelationshipType
        {
            [DataMember(Name = "id")] [Required] public required Guid Id { get; init; }
        }
    }

    public class TType : IUpsertActivityCommandData.IType
    {
        [DataMember(Name = "id")] [Required] public required Guid Id { get; init; }
    }
}
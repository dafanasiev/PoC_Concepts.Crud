using Concepts.Crud.WebApi.Application.Queries;

namespace Concepts.Crud.WebApi.Api.Relationship;

public class RelationshipServices(
    IRelationshipQueries queries,
    ILogger<RelationshipServices> logger
)
{
    public ILogger<RelationshipServices> Logger { get; } = logger;

    public IRelationshipQueries Queries { get; } = queries;
}
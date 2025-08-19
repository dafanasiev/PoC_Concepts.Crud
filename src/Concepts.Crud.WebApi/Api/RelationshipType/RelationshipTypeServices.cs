using Concepts.Crud.WebApi.Application.Queries;

namespace Concepts.Crud.WebApi.Api.RelationshipType;

public class RelationshipTypeServices(
    IRelationshipTypeQueries queries,
    ILogger<RelationshipTypeServices> logger
)
{
    public ILogger<RelationshipTypeServices> Logger { get; } = logger;
    
    public IRelationshipTypeQueries Queries { get; } = queries;
}
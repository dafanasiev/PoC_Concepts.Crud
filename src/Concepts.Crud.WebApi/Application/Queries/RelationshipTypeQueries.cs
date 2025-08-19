using Concepts.Crud.Infrastructure;
using Concepts.Crud.WebApi.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Concepts.Crud.WebApi.Application.Queries;

public class RelationshipTypeQueries(ICrudContext context) : IRelationshipTypeQueries
{
    public async Task<ICollection<RelationshipType>> GetActivityRelationshipType(CancellationToken ct)
    {
        return await context.RelationshipTypeSet
            .Select(c => new RelationshipType
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description ?? string.Empty
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
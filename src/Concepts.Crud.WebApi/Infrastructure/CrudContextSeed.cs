#if DEBUG
using Concepts.Crud.Infrastructure.Postgres;
using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Activity = Concepts.Crud.Domain.AggregatesModel.ActivityAggregate.Activity;
using ActivityRelationship = Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate.ActivityRelationship;
using ActivityType = Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate.ActivityType;
using EntityRef = Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate.EntityRef;
using RelationshipType = Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate.RelationshipType;
#if false
using ActivityGraph = Concepts.Crud.Domain.AggregatesModel.ActivityGraphAggregate.ActivityGraph;
#endif

namespace Concepts.Crud.WebApi.Infrastructure;

// ReSharper disable AccessToDisposedClosure
// ReSharper disable once ClassNeverInstantiated.Global
public class CrudContextSeed : IDbSeeder<CrudContext>
{
    const int EntityRefCount = 10;
    const int RelationshipTypeCount = 10;
    const int ActivityTypeCount = 10;
    const int ActivityRelationshipCount = 5;
    const int ActivityCount = 10;
#if false
    const int ActivityGraphCount = 10;
#endif

    public async Task SeedAsync(CrudContext context)
    {
        var activityTypeSet = await SeedActivityType(context);
        var relationshipTypeSet = await SeedRelationshipType(context);
        var entityRefSet = await SeedEntityRef(context);
        var activitySet = await SeedActivity(context, activityTypeSet);
#if false
        var activityGraphSet = await SeedActivityGraph(context);
#endif
        _ = await SeedActivityRelationship(context, relationshipTypeSet, entityRefSet, activitySet
#if false
            , activityGraphSet
#endif
            );
    }


    private static async Task<IQueryable<ActivityRelationship>> SeedActivityRelationship(CrudContext context,
        IQueryable<RelationshipType> relationshipTypeSet,
        IQueryable<EntityRef> entityRefSet,
        IQueryable<Activity> activityTypeSet
#if false
        ,IQueryable<ActivityGraph> activityGraphSet
#endif
        )
    {
        var initialized = await context.ActivityRelationshipSet.AnyAsync();
        if (initialized)
            return context.ActivityRelationshipSet;

        var randomActivityRelationshipGenerator = new RandomActivityRelationshipGenerator(
            relationshipTypeSet,
            entityRefSet,
            activityTypeSet
        );
        using var fakeRelationshipType = randomActivityRelationshipGenerator.GetRandomRelationshipType().GetEnumerator();
        using var fakeEntityRef = randomActivityRelationshipGenerator.GetRandomEntityRef().GetEnumerator();
        using var fakeActivity = randomActivityRelationshipGenerator.GetRandomActivity().GetEnumerator();
        
        var data = new Faker<ActivityRelationship>()
            .CustomInstantiator(_ =>
            {
#if false
                var activityGraph = activityGraphSet.Skip(Random.Shared.Next(0, ActivityGraphCount - 1)).First();
#endif
                return new ActivityRelationship(
                    fakeActivity.MoveNext() ? fakeActivity.Current : throw new Exception("newer"),
                    fakeRelationshipType.MoveNext() ? fakeRelationshipType.Current : throw new Exception("newer"),
                    fakeEntityRef.MoveNext() ? fakeEntityRef.Current : throw new Exception("newer")
                );
            })
            .Generate(ActivityRelationshipCount) ?? throw new Exception("ActivityRelationship generation failed");

        await context
            .ActivityRelationshipSet
            .AddRangeAsync(data, CancellationToken.None);

        await context.SaveChangesAsync();

        return context.ActivityRelationshipSet;
    }

    #if false
    private static async Task<IQueryable<ActivityGraph>> SeedActivityGraph(CrudContext context
    )
    {
        var initialized = await context.ActivityGraphSet.AnyAsync();
        if (initialized)
            return context.ActivityGraphSet;
        
        using var fakeWords = SerialWordGenerator.Generate("activity_graph-").GetEnumerator();
        using var fakeWords2 = SerialWordGenerator.Generate("type-").GetEnumerator();
        using var fakeWords3 = SerialWordGenerator.Generate("code-").GetEnumerator();

        var data = new Faker<ActivityGraph>()
            .CustomInstantiator(_ => new ActivityGraph(
                fakeWords.MoveNext() ? fakeWords.Current : throw new Exception("newer"),
                fakeWords2.MoveNext() ? fakeWords2.Current : throw new Exception("newer"),
                fakeWords3.MoveNext() ? fakeWords3.Current : throw new Exception("newer"),
                Random.Shared.Next(0, 2) == 0
            ))
            .Generate(ActivityGraphCount) ?? throw new Exception("ActivityGraph generation failed");

        context
            .ActivityGraphSet
            .AddRange(data);

        await context.SaveChangesAsync();
        return context.ActivityGraphSet;
    }
#endif

    private static async Task<IQueryable<Activity>> SeedActivity(CrudContext context,
        IQueryable<ActivityType> activityTypeSet)
    {
        var initialized = await context.ActivitySet.AnyAsync();
        if (initialized)
            return context.ActivitySet;

        using var fakeActivityCode = SerialWordGenerator.Generate("code-").GetEnumerator();
        using var fakeActivityName = SerialWordGenerator.Generate("activity-").GetEnumerator();

        var data = new Faker<Activity>()
            .CustomInstantiator(f =>
            {
                var activityType = activityTypeSet.Skip(Random.Shared.Next(0, ActivityTypeCount - 1)).First();
                return new Activity(
                    fakeActivityName.MoveNext() ? fakeActivityName.Current : throw new Exception("never"),
                    activityType,
                    fakeActivityCode.MoveNext() ? fakeActivityCode.Current : throw new Exception("never"),
                    f.Lorem.Paragraph(),
                    Random.Shared.Next(0, 2) == 0
                );
            })
            .Generate(ActivityCount) ?? throw new Exception("Activity generation failed");

        context
            .ActivitySet
            .AddRange(data);

        await context.SaveChangesAsync();
        return context.ActivitySet;
    }


    private static async Task<IQueryable<EntityRef>> SeedEntityRef(CrudContext context)
    {
        var initialized = await context.EntityRefSet.AnyAsync();
        if (initialized)
            return context.EntityRefSet;

        using var fakeEntityRefName = SerialWordGenerator.Generate("entity_ref-").GetEnumerator();
        using var fakeEntityRefReferredClassName = SerialWordGenerator.Generate("referred_class_name-").GetEnumerator();

        var data = new Faker<EntityRef>()
            .CustomInstantiator(_ => new EntityRef(
                    fakeEntityRefName.MoveNext() ? fakeEntityRefName.Current : throw new Exception("never"),
                    fakeEntityRefReferredClassName.MoveNext() ? fakeEntityRefReferredClassName.Current : throw new Exception("never")
                )
            )
            .Generate(EntityRefCount) ?? throw new Exception("EntityRef generation failed");

        await context
            .EntityRefSet
            .AddRangeAsync(data, CancellationToken.None);

        await context.SaveChangesAsync();

        return context.EntityRefSet;
    }

    private static async Task<IQueryable<RelationshipType>> SeedRelationshipType(CrudContext context)
    {
        var initialized = await context.RelationshipTypeSet.AnyAsync();
        if (initialized)
            return context.RelationshipTypeSet;

        using var fakeRelationTypeName = SerialWordGenerator.Generate("relationship_type-").GetEnumerator();

        var data = new Faker<RelationshipType>()
            .CustomInstantiator(f => new RelationshipType(
                    fakeRelationTypeName.MoveNext() ? fakeRelationTypeName.Current : throw new Exception("never"),
                    f.Lorem.Paragraph()
                )
            )
            .Generate(RelationshipTypeCount) ?? throw new Exception("RelationshipType generation failed");

        await context
            .RelationshipTypeSet
            .AddRangeAsync(data, CancellationToken.None);

        await context.SaveChangesAsync();

        return context.RelationshipTypeSet;
    }

    private static async Task<IQueryable<ActivityType>> SeedActivityType(CrudContext context)
    {
        var initialized = await context.ActivityTypeSet.AnyAsync();
        if (initialized)
            return context.ActivityTypeSet;

        using var fakeActivityTypeName = SerialWordGenerator.Generate("activity_type-").GetEnumerator();

        var data = new Faker<ActivityType>()
            .CustomInstantiator(_ => new ActivityType(
                    fakeActivityTypeName.MoveNext() ? fakeActivityTypeName.Current : throw new Exception("never")
                )
            )
            .Generate(ActivityTypeCount) ?? throw new InvalidOperationException();

        await context
            .ActivityTypeSet
            .AddRangeAsync(data, CancellationToken.None);

        await context.SaveChangesAsync();

        return context.ActivityTypeSet;
    }

    static class SerialWordGenerator
    {
        public static IEnumerable<string> Generate(string prefix, int length = int.MaxValue)
        {
            for (int i = 1; i < length; i++)
            {
                yield return $"{prefix}{i}";
            }
        }
    }

    class RandomActivityRelationshipGenerator(
        IQueryable<RelationshipType> relationshipTypeSet,
        IQueryable<EntityRef> entityRefSet,
        IQueryable<Activity> activitySet
    )
    {
        public IEnumerable<RelationshipType> GetRandomRelationshipType()
        {
            HashSet<Guid> alreadyGenerated = new();
            var n = relationshipTypeSet.Count();
            while (true)
            {
                RelationshipType rv;
                do
                {
                    rv = relationshipTypeSet
                        .Where(x => !alreadyGenerated.Contains(x.Id))
                        .Skip(Random.Shared.Next(0, n - 1 - alreadyGenerated.Count))
                        .First();
                } while (!alreadyGenerated.Add(rv.Id));

                yield return rv;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public IEnumerable<EntityRef> GetRandomEntityRef()
        {
            HashSet<Guid> alreadyGenerated = new();
            var n = entityRefSet.Count();
            while (true)
            {
                EntityRef rv;
                do
                {
                    rv = entityRefSet
                        .Where(x => !alreadyGenerated.Contains(x.Id))
                        .Skip(Random.Shared.Next(0, n - 1 - alreadyGenerated.Count))
                        .First();
                } while (!alreadyGenerated.Add(rv.Id));

                yield return rv;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public IEnumerable<Activity> GetRandomActivity()
        {
            HashSet<Guid> alreadyGenerated = new();
            var n = activitySet.Count();
            while (true)
            {
                Activity rv;
                do
                {
                    rv = activitySet
                        .Where(x => !alreadyGenerated.Contains(x.Id))
                        .Skip(Random.Shared.Next(0, n - 1 - alreadyGenerated.Count))
                        .First();
                } while (!alreadyGenerated.Add(rv.Id));

                yield return rv;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
#else
// do not seed any data in non debug builds (aka "production")
public class CrudContextSeed : IDbSeeder<CrudContext>
{
    public async Task SeedAsync(CrudContext context){}
}
#endif
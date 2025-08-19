using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Infrastructure;
using Concepts.Crud.Infrastructure.Idempotency;
using Concepts.Crud.Infrastructure.Postgres;
using Concepts.Crud.Infrastructure.Postgres.Idempotency;
using Concepts.Crud.Infrastructure.Postgres.Repositories;
using Concepts.Crud.WebApi.Api;
using Concepts.Crud.WebApi.Api.Activity;
using Concepts.Crud.WebApi.Api.ActivityType;
using Concepts.Crud.WebApi.Api.Relationship;
using Concepts.Crud.WebApi.Api.RelationshipType;
using Concepts.Crud.WebApi.Application.Behaviors;
using Concepts.Crud.WebApi.Application.Commands;
using Concepts.Crud.WebApi.Application.Queries;
using Concepts.Crud.WebApi.Infrastructure;
using Concepts.Crud.WebApi.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Concepts.Crud.WebApi.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // web services
        services.AddHttpContextAccessor();

        // infrastructure services and its options
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddTransient<IEntityRefHrefProvider, EntityRefHrefProvider>();
        services.AddOptions<EntityRefHrefProvider.TOptions>()
            .BindConfiguration("EntityRefHrefProvider")
            .ValidateDataAnnotations();
        
        services.AddTransient<IEntityRefHrefProvider, EntityRefHrefProvider>();
        services.AddOptions<EntityRefHrefProvider.TOptions>()
            .BindConfiguration("EntityRefHrefProvider")
            .ValidateDataAnnotations();        
        
        services.AddTransient<IProblemHrefProvider, ProblemHrefProvider>();
        services.AddOptions<ProblemHrefProvider.TOptions>()
            .BindConfiguration("ProblemHrefProvider")
            .ValidateDataAnnotations();
        
        services.AddDbContext<ICrudContext, CrudContext>(o =>
        {
            o.UseNpgsql(builder.Configuration.GetSection("Database")["crud"])
                ; //.UseLazyLoadingProxies();

            o.EnableSensitiveDataLogging();
        });

        if (builder.Environment.IsDevelopment())
        {
            builder
                .Services
                .AddMigration<CrudContext, CrudContextSeed>();
        }

        // Configure mediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // queries
        services.AddScoped<IActivityTypeQueries, ActivityTypeQueries>();
        services.AddScoped<IActivityQueries, ActivityQueries>();
        services.AddScoped<IRelationshipQueries, RelationshipQueries>();
        services.AddScoped<IRelationshipTypeQueries, RelationshipTypeQueries>();

        // api services
        services.AddScoped<ActivityTypeServices>();
        services.AddScoped<ActivityServices>();
        services.AddScoped<RelationshipServices>();
        services.AddScoped<RelationshipTypeServices>();
        
        // commands
        services.AddSingleton<IValidator<CreateActivityCommand>, CreateActivityCommandValidator>();
        
        // repositories
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IActivityTypeRepository, ActivityTypeRepository>();
        services.AddScoped<IEntityRefRepository, EntityRefRepository>();
        services.AddScoped<IRelationshipTypeRepository, RelationshipTypeRepository>();
        services.AddScoped<IRequestManager, RequestManager>();
    }
}
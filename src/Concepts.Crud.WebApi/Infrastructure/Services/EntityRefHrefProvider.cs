using Concepts.Crud.WebApi.Application.Models;
using Concepts.Crud.WebApi.Application.Queries;
using Microsoft.Extensions.Options;

namespace Concepts.Crud.WebApi.Infrastructure.Services;

public class EntityRefHrefProvider(
    IOptions<EntityRefHrefProvider.TOptions> options
)
    : IEntityRefHrefProvider
{
    private readonly TOptions _options = options.Value;

    public class TOptions
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Required] public required Uri BaseUrl { get; init; }
    }

    public async Task<Uri> Obtain(EntityRef eRef, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(eRef, nameof(eRef));
        ArgumentNullException.ThrowIfNull(eRef.Id, $"{nameof(eRef)}.{nameof(eRef.Id)}");
        ArgumentException.ThrowIfNullOrEmpty(eRef.ReferredClassName, $"{nameof(eRef)}.{nameof(eRef.ReferredClassName)}");

        // example: http://blabla/entity/some_class_name/FABF2B9A-9730-4966-AB9A-D5192C1D209F
        var rv = new Uri(_options.BaseUrl, $"entity/{Uri.EscapeDataString(eRef.ReferredClassName)}/{eRef.Id.Value:D}");
        return rv;
    }
}
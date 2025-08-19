using Microsoft.AspNetCore.Http;

namespace Concepts.Crud.WebApi.Infrastructure.Services;

public class IdentityService(IHttpContextAccessor context)  : IIdentityService
{
    public string GetUserIdentity()
        => context.HttpContext?.User.FindFirst("sub")?.Value;
}
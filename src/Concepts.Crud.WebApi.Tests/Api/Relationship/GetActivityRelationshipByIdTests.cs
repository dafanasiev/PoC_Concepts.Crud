using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Tests.Api.Relationship;

public class GetActivityRelationshipByIdTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly ICrudContext _crudContext;
    private readonly HttpClient _httpClient;

    public GetActivityRelationshipByIdTests(WebApiFixture webapi)
    {
        _webapi = webapi;
        _crudContext = webapi.Services.GetRequiredService<ICrudContext>();
        _httpClient = _webapi.CreateDefaultClient();
    }
    
    [Fact]
    public async Task ApiOk()
    {
        var activity = _crudContext.ActivitySet
            .AsNoTracking()
            .Include(x => x.RelationshipList)
            .First(x => x.RelationshipList.Count > 0);


        var response = await _httpClient.GetAsync($"/activity/{activity.Id:D}/relationship/{activity.RelationshipList.First().Id:D}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ActivityRelationship>();
        Assert.NotNull(body);

        Assert.NotNull(body.Id);
        //TODO: more assertions
    }
}
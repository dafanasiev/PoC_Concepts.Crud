using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Tests.Api.Relationship;

public class GetActivityRelationshipTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly ICrudContext _crudContext;
    private readonly HttpClient _httpClient;

    public GetActivityRelationshipTests(WebApiFixture webapi)
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
            .First(x=>x.RelationshipList.Count>0);

        var response = await _httpClient.GetAsync($"/activity/{activity.Id:D}/relationship");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<ActivityRelationship>>();
        Assert.NotNull(body);
        Assert.NotEmpty(body);

        Assert.DoesNotContain(body, x => x.Id == null);
        //TODO: more assertions
    }
}
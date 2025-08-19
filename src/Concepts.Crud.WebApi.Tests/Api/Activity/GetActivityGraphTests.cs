#if false
using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Tests.Api.Activity;

public class GetActivityGraphTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly HttpClient _httpClient;

    public GetActivityGraphTests(WebApiFixture webapi)
    {
        _webapi = webapi;
        _httpClient = _webapi.CreateDefaultClient();
    }

    [Fact]
    public async Task ApiOk()
    {
        var response = await _httpClient.GetAsync("/activity/graph");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<ActivityGraph>>();
        Assert.NotNull(body);
        Assert.NotEmpty(body);
        
        Assert.DoesNotContain(body, x=>x.Id == null);
        Assert.DoesNotContain(body, x=>x.Type == null);
        Assert.DoesNotContain(body, x=>x.Name == null);
        //TODO: more assertions
    }
}
#endif
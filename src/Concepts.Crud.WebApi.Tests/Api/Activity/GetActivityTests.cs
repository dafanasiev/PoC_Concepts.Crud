namespace Concepts.Crud.WebApi.Tests.Api.Activity;

public class GetActivityTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly HttpClient _httpClient;

    public GetActivityTests(WebApiFixture webapi)
    {
        _webapi = webapi;
        _httpClient = _webapi.CreateDefaultClient();
    }

    [Fact]
    public async Task ApiOk()
    {
        var response = await _httpClient.GetAsync("/activity");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<Application.Models.Activity>>();
        Assert.NotNull(body);
        Assert.NotEmpty(body);
        
        Assert.DoesNotContain(body, x=>x.Id == null);
        Assert.DoesNotContain(body, x=>x.Name == null);
        //TODO: more assertions
    }
}
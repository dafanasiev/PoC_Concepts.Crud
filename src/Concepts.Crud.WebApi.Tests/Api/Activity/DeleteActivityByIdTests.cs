namespace Concepts.Crud.WebApi.Tests.Api.Activity;

public class DeleteActivityByIdTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly ICrudContext _crudContext;
    private readonly HttpClient _httpClient;

    public DeleteActivityByIdTests(WebApiFixture webapi)
    {
        _webapi = webapi;
        _crudContext = webapi.Services.GetRequiredService<ICrudContext>();
        ;
        _httpClient = _webapi.CreateDefaultClient();
    }

    [Fact]
    public async Task ApiOk()
    {
        var activity = _crudContext.ActivitySet
            .AsNoTracking()
            .First(x => x.GC == null);

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/activity/{activity.Id:D}");
        request.Headers.Add("X-RequestId", Guid.NewGuid().ToString("D"));
        var response = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Empty(body);

        //TODO: more assertions
    }
}
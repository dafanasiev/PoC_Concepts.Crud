namespace Concepts.Crud.WebApi.Tests.Api.Activity;

public class GetActivityByIdTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly ICrudContext _crudContext;
    private readonly HttpClient _httpClient;

    public GetActivityByIdTests(WebApiFixture webapi)
    {
        _webapi = webapi;
        _crudContext = webapi.Services.GetRequiredService<ICrudContext>();;
        _httpClient = _webapi.CreateDefaultClient();
    }
    
    [Fact]
    public async Task ApiOk()
    {
        var activity = _crudContext.ActivitySet
            .AsNoTracking()
            .First(x => x.GC == null);
        
        var response = await _httpClient.GetAsync($"/activity/{activity.Id:D}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Application.Models.Activity>();
        Assert.NotNull(body);
        
        Assert.NotNull(body.Id);
        Assert.NotNull(body.Name);
        //TODO: more assertions
    }
}
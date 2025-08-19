namespace Concepts.Crud.WebApi.Tests.Api.RelationshipType;

public class GetRelationshipTypeTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _httpClient;

    public GetRelationshipTypeTests(WebApiFixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }
    
    [Fact]
    public async Task ApiOk()
    {
        var response = await _httpClient.GetAsync("/activity/relationship/type");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<Application.Models.RelationshipType>>();
        Assert.NotNull(body);
        Assert.NotEmpty(body);
        
        Assert.DoesNotContain(body, x=>x.Id == null);
        Assert.DoesNotContain(body, x=>x.Name == null);
        Assert.DoesNotContain(body, x=>x.Description == null);
    }
}
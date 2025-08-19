namespace Concepts.Crud.WebApi.Tests.Api.Activity;

public class CreateActivityTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly ICrudContext _crudContext;
    private readonly HttpClient _httpClient;

    public CreateActivityTests(WebApiFixture webapi)
    {
        _webapi = webapi;
        _crudContext = webapi.Services.GetRequiredService<ICrudContext>();
        _httpClient = _webapi.CreateDefaultClient();
    }

    [Fact]
    public async Task ApiOk()
    {
        var at = await _crudContext.ActivityTypeSet
            .AsNoTracking()
            .FirstAsync(x => x.GC == null);
        var rt = await _crudContext.RelationshipTypeSet
            .AsNoTracking()
            .FirstAsync(x => x.GC == null);
        var tspec = await _crudContext.EntityRefSet
            .AsNoTracking()
            .FirstAsync(x => x.GC == null);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/activity");
        request.Headers.Add("X-RequestId", Guid.NewGuid().ToString("D"));
        request.Content = JsonContent.Create(
            (List<Application.Models.Activity>)
            [
                new()
                {
                    Code = "code-999",
                    Type = new()
                    {
                        Id = at.Id,
                    },
                    Name = "name-999",
                    Description = "description-999",
                    IsGroup = true,
                    Relationship =
                    [
                        new()
                        {
                            RelationshipType = new()
                            {
                                Id = rt.Id,
                            },
                            TargetSpecification = new()
                            {
                                Id = tspec.Id,
                                ReferredClassName = "referred_class_name-999",
                            }
                        }
                    ]
                }
            ]
        );
        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<Application.Models.Activity>>();
        Assert.NotNull(body);
        Assert.Single(body);
        Assert.DoesNotContain(body, x => x.Id == null);
        Assert.Contains(body, x => x.Name == "name-999");
        Assert.Contains(body, x => x.Code == "code-999");
        Assert.Contains(body, x => x.ClassName == "name-999__class_name");
        Assert.Contains(body, x => x.IsGroup == true);
        Assert.Contains(body, x => x.Type.Id == at.Id);
        Assert.Single(body[0].Relationship);
        Assert.Equal(rt.Id, body[0].Relationship[0].RelationshipType.Id);
        Assert.Equal(tspec.Id, body[0].Relationship[0].TargetSpecification.Id);
        //TODO: more assertions

        //TODO: move next from webapi tests to own unit tests
        var saved = await _crudContext.ActivityTypeSet
            .AsNoTracking()
            .FirstAsync(x => x.Id == at.Id && x.GC == null);
        Assert.NotNull(saved);
    }
}
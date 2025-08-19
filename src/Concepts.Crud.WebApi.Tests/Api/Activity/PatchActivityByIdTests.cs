using Concepts.Crud.WebApi.Api.Activity;

namespace Concepts.Crud.WebApi.Tests.Api.Activity;

public class PatchActivityByIdTests : IClassFixture<WebApiFixture>
{
    private readonly WebApplicationFactory<Program> _webapi;
    private readonly ICrudContext _crudContext;
    private readonly HttpClient _httpClient;

    public PatchActivityByIdTests(WebApiFixture webapi)
    {
        _webapi = webapi;
        _crudContext = webapi.Services.GetRequiredService<ICrudContext>();
        _httpClient = _webapi.CreateDefaultClient();
    }

    [Fact]
    public async Task ApiOk()
    {
        var a = await _crudContext.ActivitySet
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.RelationshipType)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.EntityRef)
            .AsNoTracking()
            .FirstAsync(x =>
                x.GC == null
                && x.RelationshipList.Count > 0
            );


        var at = await _crudContext.ActivityTypeSet.FirstAsync(x => x.GC == null && x.Id != a.TypeId);

        var rt = await _crudContext.RelationshipTypeSet.FirstAsync(x => x.GC == null);
        var tspec = await _crudContext.EntityRefSet.FirstAsync(x => x.GC == null);

        var expectedRelationshipTypes = new HashSet<Guid>([rt.Id]);
        var expectedTargetSpecifiations = new HashSet<Guid>([tspec.Id]);

        using var request = new HttpRequestMessage(HttpMethod.Patch, $"/activity/{a.Id:D}");
        request.Headers.Add("X-RequestId", Guid.NewGuid().ToString("D"));
        request.Content = JsonContent.Create(
            new UpdateActivityRequest()
            {
                Code = "code-1111",
                Type = new()
                {
                    Id = at.Id,
                },
                Name = "name-1111",
                Description = "description-1111",
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
                            ReferredClassName = "referred_class_name-1111", //TODO: spec bug, not used?
                        }
                    }
                ]
            },
            options: new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        );

        //var s = await request.Content.ReadAsStringAsync();

        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Application.Models.Activity>();
        Assert.NotNull(body.Id);
        Assert.Equal("name-1111", body.Name);
        Assert.Equal("code-1111", body.Code);
        Assert.Equal("name-1111__class_name", body.ClassName);
        Assert.Equal(true, body.IsGroup);
        Assert.Equal(at.Id, body.Type.Id);
        Assert.Equal(rt.Id, body.Relationship[0].RelationshipType.Id);
        Assert.Equal(tspec.Id, body.Relationship[0].TargetSpecification.Id);
        //TODO: more assertions

        //TODO: move next from webapi tests to own unit tests
        var saved = await _crudContext.ActivitySet
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.RelationshipType)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.EntityRef)
            .AsNoTracking()
            .FirstAsync(x => x.Id == a.Id && x.GC == null);
        Assert.NotNull(saved);

        var newRelationshipTypes = new HashSet<Guid>(saved.RelationshipList.Select(x => x.RelationshipType.Id));
        var newTargetSpecifiations = new HashSet<Guid>(saved.RelationshipList.Select(x => x.EntityRef.Id));

        Assert.Equal(expectedTargetSpecifiations, newTargetSpecifiations);
        Assert.Equal(expectedRelationshipTypes, newRelationshipTypes);
    }
}
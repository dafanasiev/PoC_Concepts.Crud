namespace Concepts.Crud.WebApi.Tests.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class WebApiFixture
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private IHost? _app;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // builder.ConfigureHostConfiguration(config =>
        // {
        //     config.AddInMemoryCollection(new Dictionary<string, string>
        //     {
        //         //? { "Identity:Url", Concepts.Crud.WebApi.Api.ActivityType.ActivityTypeController.GetEndpoint("http").Url }
        //     });
        // });
        // builder.ConfigureServices(services =>
        // {
        //     //services.AddSingleton<IStartupFilter>(new AutoAuthorizeStartupFilter());
        // });
        _app = base.CreateHost(builder);
        return _app;
    }

    public Task InitializeAsync()
    {
        //await _app.StartAsync();
        return Task.CompletedTask;
    }

    public override async ValueTask DisposeAsync()
    {
        await ((IAsyncLifetime) this).DisposeAsync();
        GC.SuppressFinalize(this);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            switch (_app)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                default:
                    _app.Dispose();
                    break;
            }

            _app = null;
        }

        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
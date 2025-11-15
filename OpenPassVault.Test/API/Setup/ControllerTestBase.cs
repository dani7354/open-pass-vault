using OpenPassVault.API;

namespace OpenPassVault.Test.API.Setup;

public abstract class ControllerTestBase : IDisposable, IAsyncDisposable
{
    protected readonly CustomWebApplicationFactory<Startup> Factory = new();
    
    public void Dispose()
    {
        Factory.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
    }
}
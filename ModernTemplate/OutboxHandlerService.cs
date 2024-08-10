
using Microsoft.VisualBasic;

namespace ModernTemplate;

public sealed class OutboxHandlerService : IHostedService
{
    //Se till så att den får göra klart sin metod innan den stängs av om man recycla App Service

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

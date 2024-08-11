
using Microsoft.VisualBasic;

namespace ModernTemplate;

public sealed class OutboxHandlerService : BackgroundService
{
    //Se till så att den får göra klart sin metod innan den stängs av om man recycla App Service
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}

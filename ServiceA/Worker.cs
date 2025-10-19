using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceA;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBitcoinPriceTracker _bitcoinPriceTracker; 

    public Worker(ILogger<Worker> logger, IBitcoinPriceTracker bitcoinPriceTracker) 
    {
        _logger = logger;
        _bitcoinPriceTracker = bitcoinPriceTracker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker service started.");

        await _bitcoinPriceTracker.TrackBitcoinPrices(stoppingToken);

        _logger.LogInformation("Worker service stopped.");
    }
}

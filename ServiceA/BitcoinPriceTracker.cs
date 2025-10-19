using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceA
{
    public interface IBitcoinPriceTracker
    {
        Task TrackBitcoinPrices(CancellationToken stoppingToken);

    }

    public class BitcoinPriceTracker : IBitcoinPriceTracker
    {
        private readonly ILogger<BitcoinPriceTracker> _logger;
        private readonly HttpClient _httpClient = new();
        private readonly List<decimal> _lastTenPrices = new();
        private int _minuteCounter = 0;

        public BitcoinPriceTracker(ILogger<BitcoinPriceTracker> logger)
        {
            _logger = logger;
        }

        public async Task TrackBitcoinPrices(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bitcoin tracker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var price = await GetBitcoinPrice();
                    _logger.LogInformation("BTC Price: {Price} USD", price);

                    if (_lastTenPrices.Count == 10)
                        _lastTenPrices.RemoveAt(0);

                    _lastTenPrices.Add(price);

                    _minuteCounter++;
                    if (_minuteCounter % 10 == 0)
                    {
                        var averagePrice = _lastTenPrices.Average();
                        _logger.LogInformation("10-minute average: {AveragePrice}", averagePrice);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching BTC price.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task<decimal> GetBitcoinPrice()
        {
            var response = await _httpClient.GetStringAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd");
            var data = JObject.Parse(response);
            return data["bitcoin"]?["usd"]?.Value<decimal>() ?? 0;
        }
    }
}

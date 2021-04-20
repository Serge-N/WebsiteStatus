using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteStatus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient Client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Client.Dispose();
            _logger.LogInformation("Stopping up the service");
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await Client.GetAsync("http://google.com/", stoppingToken);

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"The website is up. Status Code  : {result.StatusCode}");
                }
                else
                {
                    _logger.LogError($"The website is down. Status code {result.StatusCode}");
                }
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5*1000, stoppingToken);
            }
        }
    }
}

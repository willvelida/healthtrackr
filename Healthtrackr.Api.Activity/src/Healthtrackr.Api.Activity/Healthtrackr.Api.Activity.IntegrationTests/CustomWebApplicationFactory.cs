using Azure.Identity;
using Healthtrackr.Api.Activity.Common;
using Healthtrackr.Api.Activity.Repository;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Services;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Healthtrackr.Api.Activity.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddEnvironmentVariables();
                config.AddAzureAppConfiguration(app =>
                {
                    app.Connect(new Uri(Environment.GetEnvironmentVariable("AzureAppConfigEndpoint")), new DefaultAzureCredential());
                });
            });
            builder.ConfigureTestServices(services =>
            {
                var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();
                services.Configure<Settings>(config.GetSection("Healthtrackr"));
                var cosmosClient = new CosmosClient(config.GetValue<string>("CosmosDbEndpoint"), new DefaultAzureCredential());
                services.AddSingleton(cosmosClient);
                services.AddDbContext<ActivityContext>(opt => opt.UseSqlServer(config.GetValue<string>("SqlConnectionString")));
                services.AddSingleton<IActivityRepository, ActivityRepository>();
                services.AddSingleton<ICosmosDbRepository, CosmosDbRepository>();
                services.AddSingleton<IActivityService, ActivityService>();
            });
            builder.UseEnvironment("Development");
        }
    }
}

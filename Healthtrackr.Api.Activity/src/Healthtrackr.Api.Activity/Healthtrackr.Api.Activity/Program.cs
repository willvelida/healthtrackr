using Azure.Identity;
using Healthtrackr.Api.Activity.Common;
using Healthtrackr.Api.Activity.Repository;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Services;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddAzureAppConfiguration(sp =>
{
    sp.Connect(new Uri(builder.Configuration.GetValue<string>("AzureAppConfigEndpoint")), new DefaultAzureCredential());
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Healthtrackr"));
var cosmosClientOptions = new CosmosClientOptions
{
    MaxRetryAttemptsOnRateLimitedRequests = 5,
    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(10),
};
var cosmosClient = new CosmosClient(builder.Configuration.GetValue<string>("CosmosDbEndpoint"), new DefaultAzureCredential(), cosmosClientOptions);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton(cosmosClient);
builder.Services.AddTransient<ICosmosDbRepository, CosmosDbRepository>();
builder.Services.AddTransient<IActivityService, ActivityService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/healthz/liveness", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
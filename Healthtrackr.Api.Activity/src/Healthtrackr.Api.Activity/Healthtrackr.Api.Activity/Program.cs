using Azure.Identity;
using Healthtrackr.Api.Activity.Common;
using Healthtrackr.Api.Activity.Repository;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Services;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddAzureAppConfiguration(sp =>
{
    sp.Connect(new Uri(builder.Configuration.GetValue<string>("AzureAppConfigEndpoint")), new DefaultAzureCredential());
});
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Healthtrackr"));
var cosmosClientOptions = new CosmosClientOptions
{
    MaxRetryAttemptsOnRateLimitedRequests = 5,
    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(10),
};
var cosmosClient = new CosmosClient(builder.Configuration.GetValue<string>("CosmosDbEndpoint"), new DefaultAzureCredential(), cosmosClientOptions);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton(cosmosClient);
builder.Services.AddDbContext<ActivityContext>(opt => opt.UseSqlServer(builder.Configuration.GetValue<string>("SqlConnectionString")));
builder.Services.AddTransient<IActivityRepository, ActivityRepository>();
builder.Services.AddTransient<ICosmosDbRepository, CosmosDbRepository>();
builder.Services.AddTransient<IActivityService, ActivityService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUriService>(u =>
{
    var accessor = u.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext.Request;
    var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
    return new UriService(uri);
});
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

public partial class Program { }
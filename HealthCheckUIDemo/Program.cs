using HealthChecks.UI.Client;
using HealthCheckUIDemo.Helpers;
using HealthCheckUIDemo.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);
ConfigurationHelpers.SetConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDiscoveryClient();

builder.Services.AddHttpClient("HealthCheckUIDemoTestAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5068");
})
.AddServiceDiscovery()
.AddRoundRobinLoadBalancer();

builder.Services.AddHttpClient("HealthCheckUIDemoTestMVC", client =>
{
    client.BaseAddress = new Uri("http://localhost:5256");
})
.AddServiceDiscovery()
.AddRoundRobinLoadBalancer();

var clientFactory = builder.Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();

builder.Services.AddHealthChecks()
    .AddRedis(
        redisConnectionString: ConfigurationHelpers.GetSectionValueStr("RedisConnectionString"),
        name: "Redis",
        tags: ["Docker-Compose", "Redis"])
    .AddRabbitMQ(
        rabbitConnectionString: ConfigurationHelpers.GetSectionValueStr("RabbitAmqpConnectionString"),
        name: "RabbitMQ",
        tags: ["Docker-Compose", "RabbitMQ"])
    .AddNpgSql(
        connectionString: ConfigurationHelpers.GetSectionValueStr("PostgresDbConnectionString"),
        name: "Target Postgres Database",
        tags: ["Docker-Compose", "PostgreSQL", "Database"])
    .AddSqlServer(
        connectionString: ConfigurationHelpers.GetSectionValueStr("SqlServerDbConnectionString"),
        name: "Target Sql Server Database",
        tags: ["Docker-Compose", "Sql Server", "Database"])
    .AddConsul(setup =>
    {
        setup.HostName = "localhost";
        setup.Port = 8500;
        setup.RequireHttps = false;
    },
    name: "Consul",
    tags: ["Docker-Compose", "Consul", "Service-Discovery", "hashicorp"])
    .AddElasticsearch(opt =>
    {
        opt.UseServer(ConfigurationHelpers.GetSectionValueStr("ElasticsearchAddress"));
        opt.UseBasicAuthentication(ConfigurationHelpers.GetSectionValueStr("ElasticsearchUsername"), ConfigurationHelpers.GetSectionValueStr("ElasticsearchPassword"));
    },
    name: "Elasticsearch",
    tags: ["Docker-Compose", "Elasticsearch", "Logging", "Analytics"])
    .AddCheck(
        name: "HealthCheckUIDemo TestAPI",
        instance: new HealthChecker(clientFactory, "HealthCheckUIDemoTestAPI"),
        tags: ["Service", "REST"]
    )
    .AddCheck(
name: "HealthCheckUIDemo TestMVC",
        instance: new HealthChecker(clientFactory, "HealthCheckUIDemoTestMVC"),
        tags: ["Service", "MVC"]
    );


builder.Services.AddHealthChecksUI(setupSettings =>
{
    setupSettings.SetHeaderText("Inventory Health Check Gate");
    setupSettings.AddHealthCheckEndpoint("Basic Health Check", "/health");
    setupSettings.SetEvaluationTimeInSeconds(10);
    setupSettings.SetApiMaxActiveRequests(2);
}).AddSqlServerStorage(ConfigurationHelpers.GetSectionValueStr("MonitoringDbConnectionString"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(config =>
{
    config.UIPath = "/health-ui";
    config.PageTitle = "Health Checks";
});

app.Run();

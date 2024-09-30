using Leon.Components;
using Leon.Models;
using Leon.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Dialog;
using MudBlazor.Services;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph.ExternalConnectors;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Load the app's configuration settings
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');

// Add services to the container.
//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
//    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
//    .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
//    //.AddInMemoryTokenCaches()
//    .AddDownstreamApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
//    .AddInMemoryTokenCaches();
// Used for controlling IAM Access With AzureAD and Graph API
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddDbContext<Leon_Context>(options => options.UseSqlServer(configuration.GetConnectionString("Leon_Dev")).EnableDetailedErrors());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddMudServices();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<GraphApiService>();
builder.Services.AddScoped<AssignedTaskService>();
builder.Services.AddScoped<OperationService>();
builder.Services.AddScoped<LeonWorkerService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<AppMetricsService>();

//builder.Services.AddHostedService<LeonBackgroundService>();


builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

using CSE325_Team_2.Components;
using Npgsql;
using CSE325_Team_2.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddNpgsqlDataSource(ConfigurationHelper.GetConnectionString("Default"));
builder.Services.AddScoped<UsersController>();
builder.Services.AddScoped<PlanService>();
builder.Services.AddScoped<EventTaskService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient());


string connectionString = ConfigurationHelper.GetConnectionString("Default");
await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();

Console.WriteLine($"The PostgreSQL version: {conn.PostgreSqlVersion}");


builder.Services.AddScoped<CSE325_Team_2.Data.CSE325_Team_2Context>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Must be first: trust Render's reverse proxy so the app sees the correct
// HTTPS scheme. Without this, antiforgery tokens fail on form submissions
// because ASP.NET Core thinks the request is plain HTTP.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// Clear the default networks and proxies (which only trust localhost)
// so that it accepts the headers from Render's load balancers.
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");
// Render terminates TLS at its edge proxy; the container only receives plain HTTP.
// Enabling HTTPS redirection here causes redirect loops in production.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

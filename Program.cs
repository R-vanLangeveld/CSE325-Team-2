using CSE325_Team_2.Components;
using Npgsql;
using HolidayPlanner.Services;
using Microsoft.AspNetCore.Components.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddNpgsqlDataSource(ConfigurationHelper.GetConnectionString("Default"));
builder.Services.AddScoped<UsersController>();
builder.Services.AddScoped<PlansController>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5070/") });


string connectionString = ConfigurationHelper.GetConnectionString("Default");
await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();

Console.WriteLine($"The PostgreSQL version: {conn.PostgreSqlVersion}");


builder.Services.AddScoped<HolidayPlanner.Data.HolidayPlannerContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

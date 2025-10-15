using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KlondikeSolitaire.Blazor;
using KlondikeSolitaire.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register game services
builder.Services.AddSingleton<GameService>();
builder.Services.AddScoped<StatisticsService>();

await builder.Build().RunAsync();

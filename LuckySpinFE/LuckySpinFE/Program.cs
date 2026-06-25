using LuckySpinFE.Components;
using LuckySpinFE.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("LuckySpin", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiHosts:LuckySpin"]!);
});

//builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("LuckySpin"));

builder.Services.AddScoped<LuckySpinApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var http = factory.CreateClient("LuckySpin");
    return new LuckySpinApiClient(http);
});

var app = builder.Build(); 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
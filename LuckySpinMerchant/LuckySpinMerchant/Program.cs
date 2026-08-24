
using LuckySpinMerchant.Components;
using LuckySpinMerchant.Services;

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

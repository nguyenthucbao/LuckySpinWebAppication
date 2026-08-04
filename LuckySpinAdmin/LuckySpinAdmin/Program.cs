using LuckySpinAdmin.Components;
using LuckySpinAdmin.Service;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("LuckySpin", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiHosts:LuckySpin"]!);
});


builder.Services.AddScoped<LuckySpinApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var http = factory.CreateClient("LuckySpin");
    return new LuckySpinApiClient(http);
});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie();
//builder.Services.AddAuthorization();


var app = builder.Build();

//app.MapGet("admin/export-excel", async (LuckySpinApiClient apiClient) =>
//{
//    var bytes = await apiClient.ExportStoresExcelAsync();
//    if (bytes == null)
//    {
//        return Results.Problem("Xuất file thất bại.");
//    }

//    var fileName = $"Danh_Sach_Cua_Hang.xlsx";
//    return Results.File(
//        bytes,
//        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//        fileName);
//});//.RequireAuthorization();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseAuthentication();
//app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

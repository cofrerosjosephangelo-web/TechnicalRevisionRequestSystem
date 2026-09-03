using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using TechnicalRevisionRequestSystem.Models;
using TechnicalRevisionRequestSystem.Services;
using TechnicalRevisionRequestSystem.Services.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<LoginService>();

builder.Services.AddScoped<TRRSRepositoryInsert, ISQLDB>();
builder.Services.AddScoped<TRRSRepositorySelect, SSQLDB>();
builder.Services.AddScoped<TRRSRepositoryUpdate, USQLDB>();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddHttpClient();

builder.Services.AddHttpClient<KnowledgeAIService>(client =>
{
	client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddScoped<IAIService, KnowledgeAIService>();



builder.Services.Configure<FormOptions>(options =>
{
	options.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024; // 2 GB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
	options.Limits.MaxRequestBodySize = 2L * 1024 * 1024 * 1024; // 2 GB
});

builder.Services.Configure<IISServerOptions>(options =>
{
	options.MaxRequestBodySize = 2L * 1024 * 1024 * 1024; // 2 GB
});

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

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
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

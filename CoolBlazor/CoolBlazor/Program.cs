using CoolBlazor.Infrastructure.Managers.Identity.Authentication;
using CoolBlazor.Infrastructure.Managers.Identity.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using CoolBlazor.Infrastructure.Managers.Interceptors;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using CoolBlazor.Infrastructure.Managers.Preferences;
using Microsoft.Extensions.Configuration;
using CoolBlazor.Infrastructure.Settings;
using CoolBlazor.Infrastructure.Managers.Identity.Roles;
using CoolBlazor.Infrastructure.Constants.Permission;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using CoolBlazor.Infrastructure.Managers;
using System.Globalization;
using Cropper.Blazor.Extensions;
using Cropper.Blazor.ModuleOptions;
using CoolBlazor.Infrastructure.Managers.File;
using MudBlazor;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCropper();

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Settings");
    options.Conventions.AllowAnonymousToPage("/Home");
});
builder.Services.AddServerSideBlazor()
.AddCircuitOptions(options => { options.DetailedErrors = true; })
.AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 32 * 1024 * 100;
    });



// // Identity
// builder.Services.AddHttpClient<IUserManager, UserManager>();
// builder.Services.AddLocalization(options =>
//                 {
//                     options.ResourcesPath = "Resources";
//                 });

// Authorization
builder.Services.AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddAuthorizationCore(options =>
                {
                    foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                    {
                        var propertyValue = prop.GetValue(null);
                        if (propertyValue is not null)
                        {
                            options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                        }
                    }
                })
                .AddBlazoredLocalStorage()
                .AddScoped<ClientPreferenceManager>()
                // .AddScoped<CoolBlazorStateProvider>()
                .AddScoped<AuthenticationStateProvider, CoolBlazorStateProvider>();
// AddManagers
var managers = typeof(IManager);
var types = managers
    .Assembly
    .GetExportedTypes()
    .Where(t => t.IsClass && !t.IsAbstract)
    .Select(t => new
    {
        Service = t.GetInterface($"I{t.Name}"),
        Implementation = t
    })
    .Where(t => t.Service != null);
foreach (var type in types)
{
    if (managers.IsAssignableFrom(type.Service))
    {
        builder.Services.AddTransient(type.Service, type.Implementation);
    }
};


// Register Services
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();
// builder.Services.AddScoped<AuthenticationStateProvider, CoolBlazorStateProvider>();
builder.Services.AddScoped<CoolBlazorStateProvider>();
builder.Services.AddScoped<IHttpInterceptorManager, HttpInterceptorManager>();
builder.Services.AddScoped<ClientPreferenceManager>();
builder.Services.AddScoped<IRoleManager, RoleManager>();
builder.Services.AddScoped<ImageManager>();

// Add third party libraries
builder.Services.AddMudServices();

builder.Services.AddHttpContextAccessor();
// Get Web Api Setting
var _toolBoxApiConfig = builder.Configuration.GetSection("CoolWebApi").Get<ToolBoxApiConfig>() ?? throw new InvalidOperationException("Connection string 'CoolWebApi' not found."); ;
string? httpClientName = _toolBoxApiConfig.Name;
// ArgumentException.ThrowIfNullOrEmpty(httpClientName);
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["CoolWebApi"] ?? "http://localhost:5232")
    });

builder.Services
.AddTransient<AuthenticationHeaderHandler>()
.AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(httpClientName).EnableIntercept(sp))
.AddHttpClient(httpClientName, httpClient =>
{
    httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
    httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
    httpClient.BaseAddress = new Uri(_toolBoxApiConfig.Url + _toolBoxApiConfig.Port);
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddHttpClientInterceptor();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHttpsRedirection();
    app.UseHsts();
}
else
{
    app.UseHttpsRedirection();
    app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();

// JWT
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


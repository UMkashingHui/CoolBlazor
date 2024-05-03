using CoolBlazor.Infrastructure.Models.HttpClient;
using CoolBlazor.Infrastructure.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reactive;
using ReactiveUI;

namespace CoolBlazor.Shared
{
    public partial class MainLayout : IDisposable
    {
        private MudTheme _currentTheme;
        private bool _rightToLeft = false;
        private NamedClientModel _namedClientModel;
        private readonly IConfiguration _configuration;
        private async Task RightToLeftToggle(bool value)
        {
            _rightToLeft = value;
            await Task.CompletedTask;
        }

        // protected override async Task OnInitializedAsync()
        // {
        // await jsRuntime.InvokeAsync<string>("console.log", "hello world");

        // var _toolBoxApiConfig = _configuration.GetSection("CoolWebApi").Get<ToolBoxApiConfig>() ?? throw new InvalidOperationException("Connection string 'CoolWebApi' not found.");
        // string? httpClientName = _toolBoxApiConfig.Name;
        // // ArgumentException.ThrowIfNullOrEmpty(httpClientName);
        // using HttpClient client = _httpClientFactory.CreateClient(httpClientName ?? "");

        // var response = await _httpClient.GetAsync(
        //                 $"healthcheck");

        // var request = new HttpRequestMessage(HttpMethod.Get,
        // "https://localhost:7033/HealthCheck");
        // var client = _httpClientFactory.CreateClient();
        // var response = await client.SendAsync(request);
        // await jsRuntime.InvokeAsync<string>("console.log", "response = " + response.Content);
        // _currentTheme = CoolBlazorTheme.DefaultTheme;
        // _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
        // _rightToLeft = await _clientPreferenceManager.IsRTL();
        // _interceptor.RegisterEvent();
        // var httpResponseMessage = await _namedClientModel.OnGet();
        // }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _currentTheme = CoolBlazorTheme.DefaultTheme;
                _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
                _rightToLeft = await _clientPreferenceManager.IsRTL();
                _interceptor.RegisterEvent();
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _currentTheme = CoolBlazorTheme.DefaultTheme;
        }

        private async Task DarkMode()
        {
            bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
            _currentTheme = isDarkMode
                ? CoolBlazorTheme.DefaultTheme
                : CoolBlazorTheme.DarkTheme;
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }


    }
}
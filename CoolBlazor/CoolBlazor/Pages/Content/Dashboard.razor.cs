using CoolBlazor.Infrastructure.Managers.Content;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reflection.Metadata;

namespace CoolBlazor.Pages.Content
{
    public partial class Dashboard
    {
        [Inject] private IDashboardManager DashboardManager { get; set; }

        // [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public int ProductCount { get; set; }
        [Parameter] public int BrandCount { get; set; }
        [Parameter] public int DocumentCount { get; set; }
        [Parameter] public int DocumentTypeCount { get; set; }
        [Parameter] public int DocumentExtendedAttributeCount { get; set; }
        [Parameter] public int UserCount { get; set; }
        [Parameter] public int RoleCount { get; set; }

        private readonly string[] _dataEnterBarChartXAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private readonly List<ChartSeries> _dataEnterBarChartSeries = new();
        private bool _loaded;

        // protected override async Task OnInitializedAsync()
        // {
        //     // _loaded = true;
        //     // HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
        //     // HubConnection.On(ApplicationConstants.SignalR.ReceiveUpdateDashboard, async () =>
        //     // {
        //     //     await LoadDataAsync();
        //     //     StateHasChanged();
        //     // });
        //     // if (HubConnection.State == HubConnectionState.Disconnected)
        //     // {
        //     //     await HubConnection.StartAsync();
        //     // }
        // }

        // protected override async Task OnInitializedAsync()
        // {
        //     await LoadDataAsync();
        //     StateHasChanged();

        // }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _loaded = true;
                await LoadDataAsync();
                StateHasChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            var response = await DashboardManager.GetDataAsync();
            if (response.Succeeded)
            {
                ProductCount = response.Data.ProductCount;
                BrandCount = response.Data.BrandCount;
                DocumentCount = response.Data.DocumentCount;
                DocumentTypeCount = response.Data.DocumentTypeCount;
                DocumentExtendedAttributeCount = response.Data.DocumentExtendedAttributeCount;
                UserCount = response.Data.UserCount;
                RoleCount = response.Data.RoleCount;
                foreach (var item in response.Data.DataEnterBarChart)
                {
                    _dataEnterBarChartSeries
                        .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                    _dataEnterBarChartSeries.Add(new ChartSeries { Name = item.Name, Data = item.Data });
                }
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}
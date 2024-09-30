using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Leon.Services;
using Microsoft.JSInterop;
using Leon.Models;
using System.Net.NetworkInformation;
using System.ComponentModel.Design;

namespace Leon.Components.Pages
{


   
    public partial class AppMetrics
    {
        [Inject] AppMetricsService _appMetricsService { get; set; }

        private List<AppMetric> _appMetrics { get; set; }
        private TimeOnly[]? _appMetricsNumbers { get; set; }
        private string[] _appMetricsAppNames { get; set; }

        // Convert to total minutes
        //double totalMinutes = _appMetricsNumbers.ToTimeSpan().TotalMinutes;

        protected override async Task OnInitializedAsync()
        {


            _appMetrics = await _appMetricsService.GetAllAppMetrics();

            _appMetricsAppNames = _appMetrics.Select(a => a.AppName).ToArray();
            _appMetricsNumbers = _appMetrics.Select(a => a.AppUsedInSeconds).ToArray();


        }
    }
}

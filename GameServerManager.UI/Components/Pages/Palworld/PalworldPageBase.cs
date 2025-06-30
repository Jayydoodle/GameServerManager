using GameServerManager.BAL;
using Microsoft.AspNetCore.Components;

namespace GameServerManager.UI.Components.Pages.Palworld
{
    public abstract class PalworldPageBase : ComponentBase, IDisposable
    {
        [Inject] protected PalworldService PalworldService { get; set; }

        protected string SearchString { get; set; } = "";

        protected override void OnInitialized()
        {
            // Subscribe to data updates - this will start the monitoring
            // which includes both user and server status updates
            PalworldService.DataUpdated += OnDataUpdated;
        }

        protected void OnDataUpdated(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            // Unsubscribe from updates - this will stop monitoring
            // if this was the last subscriber
            PalworldService.DataUpdated -= OnDataUpdated;
        }
    }
}

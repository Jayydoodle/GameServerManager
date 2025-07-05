using GameServerManager.BAL;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GameServerManager.UI.Components.Palworld
{
    public abstract class PalworldComponentBase : ComponentBase, IDisposable
    {
        [Inject] private GameServerService GameServerService { get; set; } = null!;
        [Inject] protected ISnackbar Snackbar { get; set; } = null!;
        protected PalworldServer Server { get; set; } = null!;

        protected override void OnInitialized()
        {
            // Subscribe to data updates - this will start the monitoring
            // which includes both user and server status updates
            Server = GameServerService.ManagedServers.FirstOrDefault() as PalworldServer;
            Server.DataUpdated += OnDataUpdated;
        }

        protected void OnDataUpdated(object sender, EventArgs e)
        {
            StateHasChanged();
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            // Unsubscribe from updates - this will stop monitoring
            // if this was the last subscriber
            Server.DataUpdated -= OnDataUpdated;
        }
    }
}

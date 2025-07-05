using GameServerManager.DAL;
using GameServerManager.DAL.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Reflection.Metadata;
using MudColor = MudBlazor.Color;

namespace GameServerManager.UI.Pages
{
    public partial class ServerManagement : ComponentBase
    {
        #region Injected Services

        [Inject] private DatabaseService DatabaseService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        #endregion

        #region Properties

        private List<DALGameServer> Servers { get; set; } = new();
        private bool IsLoading { get; set; } = true;

        #endregion

        #region Lifecycle

        protected override async Task OnInitializedAsync()
        {
            await LoadServers();
        }

        #endregion

        #region Private Methods

        private async Task LoadServers()
        {
            IsLoading = true;

            try
            {
                var servers = await DatabaseService.GetInstancesAsync<DALGameServer>();
                Servers = servers ?? new List<DALGameServer>();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading servers: {ex.Message}", Severity.Error);
                Servers = new List<DALGameServer>();
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private void AddNewServer()
        {
            Navigation.NavigateTo($"{BAL.Constants.Routes.ServerManagement}/0");
        }

        private void EditServer(int serverId)
        {
            Navigation.NavigateTo($"{BAL.Constants.Routes.ServerManagement}/{serverId}");
        }

        private void EditServerClick(MouseEventArgs e, int serverId)
        {
            // Navigate to edit server (no need to prevent propagation in Blazor)
            EditServer(serverId);
        }

        private MudColor GetGameTypeColor(string gameName)
        {
            return gameName?.ToLower() switch
            {
                "palworld" => MudColor.Primary,
                "minecraft" => MudColor.Success,
                "valheim" => MudColor.Warning,
                "rust" => MudColor.Error,
                "ark" => MudColor.Info,
                "terraria" => MudColor.Secondary,
                _ => MudColor.Default
            };
        }

        #endregion
    }
}

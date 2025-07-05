using GameServerManager.DAL;
using GameServerManager.DAL.Services;
using GameServerManager.UI.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GameServerManager.UI.Pages
{
    public partial class ServerEditor : ComponentBase
    {
        #region Injected Services

        [Inject] private DatabaseService DatabaseService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private IFilePickerService FilePickerService { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter] public int? ServerId { get; set; }

        #endregion

        #region Properties

        private DALGameServer GameServer { get; set; } = new();
        private bool IsEditMode => ServerId.HasValue && ServerId.Value > 0;
        private bool IsLoading { get; set; } = false;
        private bool IsSaving { get; set; } = false;

        #endregion

        #region Lifecycle

        protected override async Task OnInitializedAsync()
        {
            await LoadServer();
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadServer();
        }

        #endregion

        #region Private Methods

        private async Task LoadServer()
        {
            if (IsEditMode)
            {
                IsLoading = true;
                try
                {
                    var server = await DatabaseService.GetInstance<DALGameServer>(ServerId!.Value);
                    if (server != null)
                    {
                        GameServer = server;
                    }
                    else
                    {
                        Snackbar.Add($"Server with ID {ServerId} not found.", Severity.Error);
                        Navigation.NavigateTo(BAL.Constants.Routes.ServerManagement);
                    }
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Error loading server: {ex.Message}", Severity.Error);
                    Navigation.NavigateTo(BAL.Constants.Routes.ServerManagement);
                }
                finally
                {
                    IsLoading = false;
                }
            }
            else
            {
                // Initialize new server with default values
                GameServer = new DALGameServer
                {
                    GameName = "Palworld",
                    DisplayName = "",
                    Description = "",
                    FolderPath = "",
                    ExecutablePath = "PalServer.exe",
                    IpAddress = "127.0.0.1",
                    Port = 8211,
                    ApiBaseUrl = "",
                    ApiPort = null,
                    ApiUsername = "",
                    ApiPassword = "",
                    EnableAutoRestart = false,
                    EnableMonitoring = true,
                    MonitoringRefreshInterval = 30,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsNew = true
                };
            }
        }

        private async Task SaveServer()
        {
            if (!ValidateServer())
                return;

            IsSaving = true;
            try
            {
                // Update timestamps
                if (GameServer.IsNew)
                {
                    GameServer.CreatedDate = DateTime.Now;
                }
                GameServer.UpdatedDate = DateTime.Now;

                var rowsAffected = await DatabaseService.SaveItemAsync(GameServer);
                
                if (rowsAffected > 0)
                {
                    var message = IsEditMode ? "Server updated successfully!" : "Server created successfully!";
                    Snackbar.Add(message, Severity.Success);
                    
                    // Navigate back to server management
                    Navigation.NavigateTo(BAL.Constants.Routes.ServerManagement);
                }
                else
                {
                    Snackbar.Add("Failed to save server. No changes were made.", Severity.Warning);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error saving server: {ex.Message}", Severity.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task DeleteServer()
        {
            if (!IsEditMode)
                return;

            var confirmed = await ShowDeleteConfirmation();
            if (!confirmed)
                return;

            try
            {
                var rowsAffected = await DatabaseService.DeleteItemAsync(GameServer);
                
                if (rowsAffected > 0)
                {
                    Snackbar.Add("Server deleted successfully!", Severity.Success);
                    Navigation.NavigateTo(BAL.Constants.Routes.ServerManagement);
                }
                else
                {
                    Snackbar.Add("Failed to delete server.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deleting server: {ex.Message}", Severity.Error);
            }
        }

        private async Task<bool> ShowDeleteConfirmation()
        {
            var result = await DialogService.ShowMessageBox(
                "Delete Server",
                $"Are you sure you want to delete '{GameServer.DisplayName}'? This action cannot be undone.",
                yesText: "Delete",
                cancelText: "Cancel");

            return result == true;
        }

        private bool ValidateServer()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(GameServer.GameName))
                errors.Add("Game Name is required.");

            if (string.IsNullOrWhiteSpace(GameServer.DisplayName))
                errors.Add("Display Name is required.");

            if (string.IsNullOrWhiteSpace(GameServer.FolderPath))
                errors.Add("Server Folder Path is required.");

            if (string.IsNullOrWhiteSpace(GameServer.ExecutablePath))
                errors.Add("Executable Path is required.");

            if (string.IsNullOrWhiteSpace(GameServer.IpAddress))
                errors.Add("IP Address is required.");

            if (GameServer.Port <= 0 || GameServer.Port > 65535)
                errors.Add("Port must be between 1 and 65535.");

            if (GameServer.ApiPort.HasValue && (GameServer.ApiPort <= 0 || GameServer.ApiPort > 65535))
                errors.Add("API Port must be between 1 and 65535.");

            if (GameServer.MonitoringRefreshInterval < 5 || GameServer.MonitoringRefreshInterval > 300)
                errors.Add("Monitoring Interval must be between 5 and 300 seconds.");

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    Snackbar.Add(error, Severity.Error);
                }
                return false;
            }

            return true;
        }

        private void Cancel()
        {
            Navigation.NavigateTo(BAL.Constants.Routes.ServerManagement);
        }

        private async Task BrowseForFolder()
        {
            try
            {
                var selectedPath = await FilePickerService.PickFolderAsync();
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    GameServer.FolderPath = selectedPath;
                    Snackbar.Add("Folder selected successfully!", Severity.Success);
                    StateHasChanged(); // Refresh the UI to show the new path
                }
            }
            catch (InvalidOperationException ioEx)
            {
                Snackbar.Add($"File picker error: {ioEx.Message}", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Unexpected error selecting folder: {ex.Message}", Severity.Error);
            }
        }

        private async Task BrowseForExecutable()
        {
            try
            {
                var selectedPath = await FilePickerService.PickFileAsync(
                    "Select Server Executable", 
                    ".exe", "*.exe", ".bat", "*.bat", ".sh", "*.sh");
                
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // If we have a folder path, make the executable path relative to it
                    if (!string.IsNullOrEmpty(GameServer.FolderPath) && selectedPath.StartsWith(GameServer.FolderPath))
                    {
                        GameServer.ExecutablePath = Path.GetRelativePath(GameServer.FolderPath, selectedPath);
                    }
                    else
                    {
                        GameServer.ExecutablePath = selectedPath;
                    }
                    
                    Snackbar.Add("Executable selected successfully!", Severity.Success);
                    StateHasChanged(); // Refresh the UI to show the new path
                }
            }
            catch (InvalidOperationException ioEx)
            {
                Snackbar.Add($"File picker error: {ioEx.Message}", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Unexpected error selecting executable: {ex.Message}", Severity.Error);
            }
        }

        #endregion
    }
}

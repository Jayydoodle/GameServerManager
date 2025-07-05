using GameServerManager.BAL;
using MudBlazor;

namespace GameServerManager.UI.Components.Palworld
{
    public partial class SettingsComponent : PalworldComponentBase
    {
        #region Properties

        private PalworldSettings CurrentSettings { get; set; } = new();
        private string ConfigOutput => CurrentSettings.GenerateIniContent();

        #endregion

        #region Private API

        protected override async Task OnInitializedAsync()
        {
            await Server.LoadSettings();
            Server.Settings.CopyPropertiesTo(CurrentSettings, x => x.HasAttribute<IniSettingAttribute>());

            await base.OnInitializedAsync();
        }

        private void OnSettingsChanged(PalworldSettings updatedSettings)
        {
            CurrentSettings = updatedSettings;
            StateHasChanged();
        }

        private async void SaveSettings()
        {
            Server.Settings = CurrentSettings;
            await Server.SaveSettings();
            Snackbar.Add("Settings Saved Successfully!", Severity.Success);
        }

        private async Task CopyToClipboard()
        {
            // TODO: Implement clipboard copy functionality
            // This would require JavaScript interop
        }

        private async Task PasteFromClipboard()
        {
            // TODO: Implement clipboard paste functionality
            // This would require JavaScript interop
        }

        #endregion
    }
}

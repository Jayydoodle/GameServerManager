namespace GameServerManager.UI.Components.Palworld
{
    public partial class DashboardComponent : PalworldComponentBase
    {
        private string SearchString = "";

        protected MudBlazor.Color GetFpsColor(int fps)
        {
            if (fps >= 30) return MudBlazor.Color.Success;
            if (fps >= 20) return MudBlazor.Color.Warning;
            return MudBlazor.Color.Error;
        }
    }
}

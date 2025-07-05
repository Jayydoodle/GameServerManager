using Microsoft.AspNetCore.Components;

namespace GameServerManager.UI.Pages
{
    public partial class ServerDetails : ComponentBase
    {
        #region Parameters

        [Parameter] public int? ServerId { get; set; }

        #endregion
    }
}

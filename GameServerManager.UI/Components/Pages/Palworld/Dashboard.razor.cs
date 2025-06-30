using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.UI.Components.Pages.Palworld
{
    public partial class Dashboard : PalworldPageBase
    {
        protected MudBlazor.Color GetFpsColor(int fps)
        {
            if (fps >= 30) return MudBlazor.Color.Success;
            if (fps >= 20) return MudBlazor.Color.Warning;
            return MudBlazor.Color.Error;
        }
    }
}

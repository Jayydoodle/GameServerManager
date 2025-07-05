using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public static class Constants
    {
        public static class Routes
        {
            public const string ServerManagement = "/server-management";
            public const string ServerDetails = "/server";
        }

        public enum GameType
        {
            Palworld
        }

        public static class Palworld
        {
            public static class Paths
            {
                public const string ExecutableFilePath = "Pal/Binaries/Win64/PalServer-Win64-Shipping-Cmd.exe";
                public const string SettingsFilePath = "Pal/Saved/Config/WindowsServer/PalWorldSettings.ini";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public static class AppRoutes
    {
        private const string ServerRoutePrefix = "/servers/";

        public static class Palworld
        {
            public const string Dashboard = ServerRoutePrefix + "dashboard";
        }
    }
}

using GameServerManager.BAL.Models;
using GameServerManager.DAL.Services;
using System.Collections.ObjectModel;

namespace GameServerManager.BAL
{
    public class GameServerService : IDisposable
    {
        #region Properties

        public ObservableCollection<GameServer> ManagedServers { get; private set; } = new ObservableCollection<GameServer>();

        #endregion

        #region Public API

        public async Task InitializeManagedServers()
        {
            var servers = await PalworldServer.GetAll<PalworldServer>();
            servers.ForEach(async server => await server.Init());

            foreach (var server in servers)
                ManagedServers.Add(server);
        }

        public void Dispose()
        {
            ManagedServers = null;
        }

        #endregion
    }
}
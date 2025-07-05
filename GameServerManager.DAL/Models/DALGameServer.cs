using GameServerManager.DAL.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.DAL
{
    public class DALGameServer : DALObjectBase, IUpdatedObject
    {
        [PrimaryKey, AutoIncrement]
        public int GameServerId { get; set; }
        public string GameName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string FolderPath { get; set; }
        public string ExecutablePath { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string? ApiBaseUrl { get; set; }
        public int? ApiPort { get; set; }
        public string? ApiUsername { get; set; }
        public string? ApiPassword { get; set; }
        public bool EnableAutoRestart { get; set; }
        public bool EnableMonitoring { get; set; }
        public int MonitoringRefreshInterval { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

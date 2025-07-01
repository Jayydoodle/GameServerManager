using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace GameServerManager.BAL
{
    /// <summary>
    /// Represents a Palworld game server with all its associated data
    /// </summary>
    public class PalworldServer : GameServerBase
    {
        #region Properties: From API

        /// <summary>
        /// The server version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// The server name.
        /// </summary>
        [JsonPropertyName("servername")]
        public string ServerName { get; set; }

        /// <summary>
        /// The server description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The world GUID.
        /// </summary>
        [JsonPropertyName("worldguid")]
        public string WorldGuid { get; set; }

        #endregion

        /// <summary>
        /// Last time the server data was updated
        /// </summary>
        [JsonIgnore]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Whether the server data is currently being loaded
        /// </summary>
        [JsonIgnore]
        public bool IsLoading { get; set; }

        /// <summary>
        /// Whether the server is currently online
        /// </summary>
        [JsonIgnore]
        public bool IsOnline => Status != null;

        /// <summary>
        /// Returns a formatted server identifier (name + world guid)
        /// </summary>
        [JsonIgnore]
        public string ServerIdentifier => $"{ServerName} ({WorldGuid})";

        /// <summary>
        /// Returns the uptime as a formatted string if the server is online
        /// </summary>
        [JsonIgnore]
        public string UptimeDisplay => Status?.UptimeFormatted ?? "Offline";

        /// <inheritdoc/>
        [JsonIgnore]
        public override string ServerExecutablePath => "G:\\steamcmd\\steamapps\\common\\PalServer\\Pal\\Binaries\\Win64\\PalServer-Win64-Shipping-Cmd.exe";

        /// <summary>
        /// The server's current status information
        /// </summary>
        [JsonIgnore]
        public PalworldServerStatus Status { get; set; }

        /// <summary>
        /// Collection of users currently on the server
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<PalworldUser> Users { get; } = new ObservableCollection<PalworldUser>();
    }
}
using System.Text.Json.Serialization;

namespace GameServerManager.BAL
{
    public class PalworldServerStatus
    {
        /// <summary>
        /// The server FPS.
        /// </summary>
        [JsonPropertyName("serverfps")]
        public int ServerFps { get; set; }

        /// <summary>
        /// The number of current players.
        /// </summary>
        [JsonPropertyName("currentplayernum")]
        public int CurrentPlayerCount { get; set; }

        /// <summary>
        /// Server frame time (ms)
        /// </summary>
        [JsonPropertyName("serverframetime")]
        public double ServerFrameTime { get; set; }

        /// <summary>
        /// The maximum number of players.
        /// </summary>
        [JsonPropertyName("maxplayernum")]
        public int MaxPlayerCount { get; set; }

        /// <summary>
        /// The server uptime in seconds.
        /// </summary>
        [JsonPropertyName("uptime")]
        public int UptimeSeconds { get; set; }

        /// <summary>
        /// The server days of in-game.
        /// </summary>
        [JsonPropertyName("days")]
        public int InGameDays { get; set; }

        // Calculated properties for convenience

        /// <summary>
        /// Gets the server uptime as a TimeSpan.
        /// </summary>
        [JsonIgnore]
        public TimeSpan Uptime => TimeSpan.FromSeconds(UptimeSeconds);

        /// <summary>
        /// Gets the formatted uptime string (e.g., "3d 5h 12m 30s").
        /// </summary>
        [JsonIgnore]
        public string UptimeFormatted
        {
            get
            {
                var uptime = Uptime;
                return $"{(uptime.Days > 0 ? $"{uptime.Days}d " : "")}" +
                       $"{(uptime.Hours > 0 ? $"{uptime.Hours}h " : "")}" +
                       $"{(uptime.Minutes > 0 ? $"{uptime.Minutes}m " : "")}" +
                       $"{uptime.Seconds}s";
            }
        }

        /// <summary>
        /// Gets the player count as a string (e.g., "5/20").
        /// </summary>
        [JsonIgnore]
        public string PlayerCountDisplay => $"{CurrentPlayerCount}/{MaxPlayerCount}";
    }
}
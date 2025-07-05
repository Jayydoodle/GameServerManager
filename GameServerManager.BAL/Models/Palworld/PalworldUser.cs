using System.Text.Json.Serialization;

namespace GameServerManager.BAL
{
    public class PalworldUser
    {
        /// <summary>
        /// The player name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// User's platform account name.
        /// </summary>
        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// The player ID.
        /// </summary>
        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        /// <summary>
        /// The user ID.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// The player IP address.
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// The player ping.
        /// </summary>
        [JsonPropertyName("ping")]
        public double Ping { get; set; }

        /// <summary>
        /// The player location X.
        /// </summary>
        [JsonPropertyName("location_x")]
        public double LocationX { get; set; }

        /// <summary>
        /// The player location Y.
        /// </summary>
        [JsonPropertyName("location_y")]
        public double LocationY { get; set; }

        /// <summary>
        /// Current player game level.
        /// </summary>
        [JsonPropertyName("level")]
        public int Level { get; set; }

        /// <summary>
        /// The number of buildings owned by the player.
        /// </summary>
        [JsonPropertyName("building_count")]
        public int BuildingCount { get; set; }

        // Additional calculated properties for convenience

        /// <summary>
        /// Gets the formatted location as a string (e.g., "123.45, 678.90")
        /// </summary>
        [JsonIgnore]
        public string FormattedLocation => $"{LocationX:F2}, {LocationY:F2}";

        /// <summary>
        /// Gets a shortened version of the player ID (first 8 characters)
        /// </summary>
        [JsonIgnore]
        public string ShortPlayerId => !string.IsNullOrEmpty(PlayerId) && PlayerId.Length > 8
            ? PlayerId.Substring(0, 8) + "..."
            : PlayerId;

        /// <summary>
        /// Gets a shortened version of the user ID (first 8 characters)
        /// </summary>
        [JsonIgnore]
        public string ShortUserId => !string.IsNullOrEmpty(UserId) && UserId.Length > 8
            ? UserId.Substring(0, 8) + "..."
            : UserId;
    }
}
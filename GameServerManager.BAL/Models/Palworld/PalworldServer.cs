using GameServerManager.BAL.Models;
using GameServerManager.BAL.Utilites;
using GameServerManager.DAL;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServerManager.BAL
{
    /// <summary>
    /// Represents a Palworld game server with all its associated data
    /// </summary>
    public class PalworldServer : GameServer<PalworldServer>
    {
        #region Constants

        private const string InfoApiEndpoint = "info";
        private const string StatusApiEndpoint = "metrics";
        private const string UsersApiEndpoint = "players";

        #endregion

        #region Properties

        /// <summary>
        /// Last time the server data was updated
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Whether the server data is currently being loaded
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Whether the server is currently online
        /// </summary>
        public bool IsOnline => Status != null;

        /// <summary>
        /// Returns a formatted server identifier (name + world guid)
        /// </summary>
        public string ServerIdentifier => $"{DisplayName} ({WorldGuid})";

        /// <summary>
        /// Returns the uptime as a formatted string if the server is online
        /// </summary>
        public string UptimeDisplay => Status?.UptimeFormatted ?? "Offline";

        /// <summary>
        /// The server's current status information
        /// </summary>
        public PalworldServerStatus Status { get; set; }

        /// <summary>
        /// The server's settings from PalWorldSettings.ini
        /// </summary>
        public PalworldSettings Settings { get; set; }

        /// <summary>
        /// Collection of users currently on the server
        /// </summary>
        public ObservableCollection<PalworldUser> Users { get; } = new ObservableCollection<PalworldUser>();

        /// <summary>
        /// The <see cref="JsonSerializerOptions" /> used when pulling data from the Api
        /// </summary>
        private JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        #endregion

        #region Properties: From API

        /// <summary>
        /// The server version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <inheritdoc/>
        [JsonPropertyName("servername")]
        public override string DisplayName
        {
            get => base.DisplayName;
            set => base.DisplayName = value;
        }

        /// <inheritdoc/>
        [JsonPropertyName("description")]
        public override string Description 
        { 
            get => base.Description;
            set => base.Description = value; 
        }

        /// <summary>
        /// The world GUID.
        /// </summary>
        [JsonPropertyName("worldguid")]
        public string WorldGuid { get; set; }

        #endregion



        #region Constructor

        public PalworldServer(DALGameServer item)
        : base(item)
        {
        }

        public PalworldServer()
        : base()
        {
        }

        #endregion

        #region Public API

        protected override async Task InternalInit()
        {
            await LoadSettings();
        }

        public override Task OnMonitoringStarted()
        {
            return Task.Run(() => System.Diagnostics.Debug.WriteLine("Palworld monitoring started"));
        }

        public override Task OnMonitoringStopped() 
        {
            return Task.Run(() => System.Diagnostics.Debug.WriteLine("Palworld monitoring stopped"));
        }

        public override async Task RefreshServerInfo()
        {
            // Don't refresh if no one is listening
            //if (!_isMonitoring && !HasUpdateSubscribers)
            //    return;
            bool serverOffliine = false;

            EnterCriticalSection(async () =>
            {
                Action<Exception> logGenericError = (ex) => System.Diagnostics.Debug.WriteLine($"Error refreshing server data: {ex.Message}");

                try
                {
                    IsLoading = true;
                    NotifyDataUpdated();

                    // Create tasks for all API calls
                    await RefreshServerInfoInternal();
                    await RefreshServerStatusInternal();
                    await RefreshUsersInternal();

                    LastUpdated = DateTime.Now;
                }
                catch (HttpRequestException ex)
                {
                    if (ex.HttpRequestError == HttpRequestError.ConnectionError)
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not make a connection to the server '{DisplayName}' (ID: {GameServerId}) via its API.");
                        serverOffliine = true;
                    }
                    else
                    {
                        logGenericError(ex);
                    }
                }
                catch (Exception ex)
                {
                    logGenericError(ex);
                }
                finally
                {
                    IsLoading = false;
                    NotifyDataUpdated();

                    if (serverOffliine)
                        await OnServerStopped();
                }
            });
        }

        public async Task LoadSettings()
        {
            //ToDo
            if (string.IsNullOrEmpty(FolderPath))
                return;

            string path = StringUtil.CombinePath(FolderPath, Constants.Palworld.Paths.SettingsFilePath);
            Settings = await PalworldSettings.LoadFromFileAsync(path);
        }

        public async Task SaveSettings()
        {
            string path = StringUtil.CombinePath(FolderPath, Constants.Palworld.Paths.SettingsFilePath);
            await Settings.SaveToFileAsync(path);
        }

        #endregion

        #region Private API

        /// <summary>
        /// Refreshes the basic server information
        /// </summary>
        private async Task RefreshServerInfoInternal()
        {
            try
            {
                JsonObject jobject = await ApiClient.GetAsync<JsonObject>(InfoApiEndpoint);

                // Deserialize directly into our existing Server object
                JsonSerializer.Deserialize<PalworldServer>(jobject.ToJsonString(), SerializerOptions)
                              .CopyPropertiesTo(this, x => x.HasAttribute<JsonPropertyNameAttribute>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing server info: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Refreshes the server status information
        /// </summary>
        private async Task RefreshServerStatusInternal()
        {
            try
            {
                JsonObject jobject = await ApiClient.GetAsync<JsonObject>(StatusApiEndpoint);
                Status = JsonSerializer.Deserialize<PalworldServerStatus>(jobject.ToJsonString(), SerializerOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing server status: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Refreshes the user list
        /// </summary>
        private async Task RefreshUsersInternal()
        {
            try
            {
                JsonObject jobject = await ApiClient.GetAsync<JsonObject>(UsersApiEndpoint);
                var users = JsonSerializer.Deserialize<List<PalworldUser>>(jobject["players"].ToJsonString(), SerializerOptions);
                await UpdateUsersCollection(users);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing users: {ex.Message}");
                throw;
            }
        }

        private async Task OnServerStopped()
        {
            StopMonitoring();

            if (EnableAutoRestart)
            {
                System.Diagnostics.Debug.WriteLine($"The server {DisplayName} (ID: {GameServerId}) was detected offline, attempting to restart.");
                await StartServer();
                StartMonitoring();
            }
        }

        private async Task UpdateUsersCollection(IEnumerable<PalworldUser> users)
        {
            // Ensure we're on the UI thread
            await Task.Yield();

            // Update the collection
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        #endregion
    }
}

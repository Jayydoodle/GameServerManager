using GameServerManager.BAL.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public class PalworldService : IDisposable
    {
        #region Constants

        private const string InfoApiEndpoint = "info";
        private const string StatusApiEndpoint = "metrics";
        private const string UsersApiEndpoint = "players";

        #endregion


        private Timer _updateTimer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(10);
        private bool _isActive = false;
        private readonly object _lockObject = new object();
        private readonly ApiClient _apiClient;

        /// <summary>
        /// The current Palworld server instance
        /// </summary>
        public PalworldServer Server { get; }

        private event EventHandler _dataUpdated;
        public event EventHandler DataUpdated
        {
            add
            {
                bool wasEmpty = _dataUpdated == null;
                _dataUpdated += value;

                // Start active monitoring when first subscriber is added
                if (wasEmpty)
                {
                    StartMonitoring();
                }
            }
            remove
            {
                _dataUpdated -= value;

                // Stop active monitoring when last subscriber is removed
                if (_dataUpdated == null)
                {
                    StopMonitoring();
                }
            }
        }

        public PalworldService()
        {
            // Initialize the API client once
            _apiClient = new ApiClient("http://localhost:8212/v1/api");
            _apiClient.UseBasicAuthentication("admin", "ChesterPA1.");
            Server = new PalworldServer();
        }

        private void StartMonitoring()
        {
            lock (_lockObject)
            {
                if (!_isActive)
                {
                    // Do an initial refresh of server data
                    _ = RefreshServerData();

                    // Start the timer for periodic updates
                    _updateTimer = new Timer(async _ => await RefreshServerData(), null, _updateInterval, _updateInterval);
                    _isActive = true;
                    Console.WriteLine("Palworld monitoring started");
                }
            }
        }

        private void StopMonitoring()
        {
            lock (_lockObject)
            {
                if (_isActive)
                {
                    // Stop the timer
                    _updateTimer?.Dispose();
                    _updateTimer = null;
                    _isActive = false;
                    Console.WriteLine("Palworld monitoring stopped");
                }
            }
        }

        /// <summary>
        /// Refreshes all server data including server info, status, and users
        /// </summary>
        public async Task RefreshServerData()
        {
            if (!_isActive && _dataUpdated == null)
            {
                // Don't refresh if no one is listening
                return;
            }

            try
            {
                Server.IsLoading = true;
                NotifyDataUpdated();

                // Create tasks for all API calls
                var serverInfoTask = RefreshServerInfoInternal();
                var statusTask = RefreshServerStatusInternal();
                var usersTask = RefreshUsersInternal();

                // Wait for all to complete
                await Task.WhenAll(serverInfoTask, statusTask, usersTask);

                Server.LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error refreshing server data: {ex.Message}");
            }
            finally
            {
                Server.IsLoading = false;
                NotifyDataUpdated();
            }
        }

        /// <summary>
        /// Refreshes the basic server information
        /// </summary>
        private async Task RefreshServerInfoInternal()
        {
            try
            {
                JsonObject jobject = await _apiClient.GetAsync<JsonObject>(InfoApiEndpoint);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize directly into our existing Server object
                JsonSerializer.Deserialize<PalworldServer>(jobject.ToJsonString(), options)
                    .CopyPropertiesTo(Server);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing server info: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes the server status information
        /// </summary>
        private async Task RefreshServerStatusInternal()
        {
            try
            {
                JsonObject jobject = await _apiClient.GetAsync<JsonObject>(StatusApiEndpoint);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Server.Status = JsonSerializer.Deserialize<PalworldServerStatus>(jobject.ToJsonString(), options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing server status: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes the user list
        /// </summary>
        private async Task RefreshUsersInternal()
        {
            try
            {
                JsonObject jobject = await _apiClient.GetAsync<JsonObject>(UsersApiEndpoint);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var users = JsonSerializer.Deserialize<List<PalworldUser>>(jobject["players"].ToJsonString(), options);
                await UpdateUsersCollection(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing users: {ex.Message}");
            }
        }

        private async Task UpdateUsersCollection(IEnumerable<PalworldUser> users)
        {
            // Ensure we're on the UI thread
            await Task.Yield();

            // Update the collection
            Server.Users.Clear();
            foreach (var user in users)
            {
                Server.Users.Add(user);
            }
        }

        private void NotifyDataUpdated()
        {
            _dataUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Force a refresh even if monitoring isn't active
        /// </summary>
        public Task ForceRefresh()
        {
            return RefreshServerData();
        }

        public void Dispose()
        {
            StopMonitoring();
            (_apiClient as IDisposable)?.Dispose();
        }
    }

    // Helper extension method for copying properties
    public static class ObjectExtensions
    {
        public static void CopyPropertiesTo<T>(this T source, T destination)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (property.CanWrite && property.CanRead && !property.Name.Equals("Users", StringComparison.OrdinalIgnoreCase) && !property.Name.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    var value = property.GetValue(source);
                    property.SetValue(destination, value);
                }
            }
        }
    }
}
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GameServerManager.BAL
{
    public class PalworldService : GameServiceBase
    {
        #region Constants

        private const string InfoApiEndpoint = "info";
        private const string StatusApiEndpoint = "metrics";
        private const string UsersApiEndpoint = "players";

        #endregion

        #region Properties

        /// <summary>
        /// The current Palworld server instance
        /// </summary>
        public PalworldServer Server { get; private set; }

        /// <inheritdoc/>
        protected override string ApiBaseUrl => "http://localhost:8212/v1/api";

        private JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        #endregion

        #region Constructor

        public PalworldService()
        : base()
        {
        }

        #endregion

        #region Public API

        /// <summary>
        /// Refreshes all server data including server info, status, and users
        /// </summary>
        public override async Task RefreshServerData()
        {
            // Don't refresh if no one is listening
            if (!_isMonitoring && !HasUpdateSubscribers)
                return;

            try
            {
                Server.IsLoading = true;
                NotifyDataUpdated();

                // Create tasks for all API calls
                await RefreshServerInfoInternal();
                await RefreshServerStatusInternal();
                await RefreshUsersInternal();

                Server.LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                Action printGenericError = () => System.Diagnostics.Debug.WriteLine($"Error refreshing server data: {ex.Message}");

                if (ex is HttpRequestException requestException)
                {
                    if (requestException.HttpRequestError == HttpRequestError.ConnectionError)
                    {
                        System.Diagnostics.Debug.WriteLine($"Server not online, attempting to restart.");
                        await OnServerStopped();
                    }
                    else
                    {
                        printGenericError();
                    }
                }
                else
                {
                    printGenericError();
                }
            }
            finally
            {
                Server.IsLoading = false;
                NotifyDataUpdated();
            }
        }

        #endregion

        #region Private API

        protected override void InitializeServer()
        {
            ApiClient.UseBasicAuthentication("admin", "ChesterPA1.");
            Server = new PalworldServer();
        }

        protected override async Task OnServerStopped()
        {
            StopMonitoring();
            await Server.StartServer();
            StartMonitoring();
        }

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
                    .CopyPropertiesTo(Server);
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

                Server.Status = JsonSerializer.Deserialize<PalworldServerStatus>(jobject.ToJsonString(), SerializerOptions);
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

        #endregion
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
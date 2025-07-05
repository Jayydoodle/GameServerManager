using GameServerManager.BAL.Utilites;
using GameServerManager.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameServerManager.BAL.Models
{
    public abstract class GameServer<TBal> : GameServer
    {
        public GameServer(DALGameServer item)
        : base(item)
        {
        }

        public GameServer()
        : base()
        {
        }
    }

    public abstract class GameServer : BALObjectBase<DALGameServer>
    {
        #region Properties

        /// <summary>
        /// The internal server process
        /// </summary>
        private ServerProcess ServerProcess { get; set; }

        /// <summary>
        /// The absolute path to the base folder of the server
        /// </summary>
        public string FolderPath 
        {
            get => _item.FolderPath; 
            set => _item.FolderPath = value;
        }

        /// <summary>
        /// The relative path to the executable file that will run the server
        /// </summary>
        public string ExecutablePath
        {
            get => _item.ExecutablePath;
            set => _item.ExecutablePath = value;
        }

        /// <summary>
        /// The name of the game (e.g., "Palworld", "Minecraft", etc.)
        /// </summary>
        public string GameName
        {
            get => _item.GameName;
            set => _item.GameName = value;
        }

        /// <summary>
        /// The display name of the game server
        /// </summary>
        public virtual string DisplayName
        {
            get => _item.DisplayName;
            set => _item.DisplayName = value;
        }

        /// <summary>
        /// The description of the game server
        /// </summary>
        public virtual string Description
        {
            get => _item.Description;
            set => _item.Description = value;
        }

        /// <summary>
        /// The IP address the server is bound to
        /// </summary>
        public string IpAddress
        {
            get => _item.IpAddress;
            set => _item.IpAddress = value;
        }

        /// <summary>
        /// The port the server is listening on
        /// </summary>
        public int Port
        {
            get => _item.Port;
            set => _item.Port = value;
        }

        /// <summary>
        /// The base URL for the server's API
        /// </summary>
        public string? ApiBaseUrl
        {
            get => _item.ApiBaseUrl;
            set => _item.ApiBaseUrl = value;
        }

        /// <summary>
        /// The port for the server's API
        /// </summary>
        public int? ApiPort
        {
            get => _item.ApiPort;
            set => _item.ApiPort = value;
        }

        /// <summary>
        /// The username for the server's API
        /// </summary>
        public string? ApiUsername
        {
            get => _item.ApiUsername;
            set => _item.ApiUsername = value;
        }

        /// <summary>
        /// The password for the server's API
        /// </summary>
        public string? ApiPassword
        {
            get => _item.ApiPassword;
            set => _item.ApiPassword = value;
        }

        /// <summary>
        /// Whether auto-restart is enabled for this server
        /// </summary>
        public bool EnableAutoRestart
        {
            get => _item.EnableAutoRestart;
            set => _item.EnableAutoRestart = value;
        }

        /// <summary>
        /// Whether monitoring is enabled for this server
        /// </summary>
        public bool EnableMonitoring
        {
            get => _item.EnableMonitoring;
            set => _item.EnableMonitoring = value;
        }

        /// <summary>
        /// The interval in seconds for monitoring refresh
        /// </summary>
        public int MonitoringRefreshInterval
        {
            get => _item.MonitoringRefreshInterval;
            set => _item.MonitoringRefreshInterval = value;
        }

        /// <summary>
        /// The date when this server configuration was created
        /// </summary>
        public DateTime CreatedDate
        {
            get => _item.CreatedDate;
            set => _item.CreatedDate = value;
        }

        /// <summary>
        /// The date when this server configuration was last updated
        /// </summary>
        public DateTime UpdatedDate
        {
            get => _item.UpdatedDate;
            set => _item.UpdatedDate = value;
        }

        /// <summary>
        /// The unique identifier for this game server
        /// </summary>
        public int GameServerId
        {
            get => _item.GameServerId;
            set => _item.GameServerId = value;
        }

        /// <summary>
        /// The Api client used to make requests to the server
        /// </summary>
        protected ApiClient? ApiClient { get; set; }

        /// <summary>
        /// The timer which will determine how often calls to the API are made for updating sserver info
        /// </summary>
        private Timer? ApiUpdateTimer { get; set; }

        /// <summary>
        /// The interval at which the the <see cref="ApiUpdateTimer"/> should fire its event handler
        /// </summary>
        protected TimeSpan ApiUpdateInterval { get; set; }

        /// <summary>
        /// Occurs when console output has been received.  Used for real-time UI updates
        /// </summary>
        public event Action<string> OnConsoleOutputReceived
        {
            add { ServerProcess.OnConsoleOutputReceived += value; }
            remove { ServerProcess.OnConsoleOutputReceived -= value; }
        }

        private readonly object _lockObject = new object();

        protected bool _isMonitoring;

        #endregion

        #region Events

        private event EventHandler? _statusUpdated;
        public event EventHandler StatusUpdated
        {
            add
            {
                bool noSubscriptions = _statusUpdated == null;
                _statusUpdated += value;

                // Start active monitoring when first subscriber is added
                //if (noSubscriptions)
                //    StartMonitoring();
            }
            remove
            {
                _statusUpdated -= value;

                // Stop active monitoring when last subscriber is removed
                //if (_dataUpdated == null)
                //    StopMonitoring();
            }
        }

        private event EventHandler? _dataUpdated;
        public event EventHandler DataUpdated
        {
            add
            {
                bool noSubscriptions = _dataUpdated == null;
                _dataUpdated += value;
            }
            remove
            {
                _dataUpdated -= value;
            }
        }

        #endregion

        #region Constructor
        public GameServer(DALGameServer item)
        : base(item)
        {
        }

        public GameServer()
        : base()
        {
        }
        #endregion

        #region Public API

        /// <summary>
        /// Perform any server specific initialization logic before <see cref="Init"/> is called
        /// </summary>
        /// <returns></returns>
        protected abstract Task InternalInit();

        /// <summary>
        /// Refreshes server info from the configured <see cref="ApiClient"/> if applicable
        /// </summary>
        /// <returns></returns>
        public abstract Task RefreshServerInfo();

        /// <summary>
        /// Occurs after <see cref="StartMonitoring" />
        /// </summary>
        /// <returns></returns>
        public abstract Task OnMonitoringStarted();

        /// <summary>
        /// Occurs after <see cref="StopMonitoring" />
        /// </summary>
        /// <returns></returns>
        public abstract Task OnMonitoringStopped();

        /// <summary>
        /// Initializes the server
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            await InternalInit();

            if (!string.IsNullOrEmpty(FolderPath) && !string.IsNullOrEmpty(ExecutablePath))
            {
                string executablePath = StringUtil.CombinePath(FolderPath, ExecutablePath);
                ServerProcess = new ServerProcess(executablePath);
            }

            if (!string.IsNullOrEmpty(ApiBaseUrl))
            {
                ApiClient = new ApiClient(ApiBaseUrl);

                if (!string.IsNullOrEmpty(ApiUsername) && !string.IsNullOrEmpty(ApiPassword))
                    ApiClient.UseBasicAuthentication(ApiUsername, ApiPassword);
            }

            ApiUpdateInterval = TimeSpan.FromSeconds(10);

            await StartServer();
            StartMonitoring();
        }

        /// <summary>
        /// Starts the <see cref="BAL.ServerProcess"/>
        /// </summary>
        /// <returns></returns>
        public async Task StartServer()
        {
            //ToDo
            if (ServerProcess == null)
                return;

            await ServerProcess.Start();
        }

        /// <summary>
        /// Stops the <see cref="BAL.ServerProcess"/>
        /// </summary>
        /// <returns></returns>
        public async Task StopServer()
        {
            await ServerProcess.Stop();
        }

        /// <summary>
        /// Starts monitoring the server by calling <see cref="RefreshServerInfo" /> on a provided <see cref="ApiUpdateInterval"/>
        /// </summary>
        public async void StartMonitoring()
        {
            //ToDo
            if (!_isMonitoring && ApiClient != null)
            {
                // Do an initial refresh of server data
                await RefreshServerInfo();

                EnterCriticalSection(async () =>
                {
                    // Start the timer for periodic updates
                    ApiUpdateTimer = new Timer(async _ => await RefreshServerInfo(), null, ApiUpdateInterval, ApiUpdateInterval);
                    _isMonitoring = true;
                    await OnMonitoringStarted();
                });
            }
        }

        /// <summary>
        /// Stops monitoring the server by canceling the invocation of <see cref="RefreshServerInfo"/> on the interval defined by <see cref="ApiUpdateInterval"/>
        /// </summary>
        public void StopMonitoring()
        {
            if (_isMonitoring)
            {
                EnterCriticalSection(async () =>
                {
                    // Stop the timer
                    ApiUpdateTimer?.Dispose();
                    ApiUpdateTimer = null;
                    _isMonitoring = false;
                    await OnMonitoringStopped();
                    System.Diagnostics.Debug.WriteLine("Palworld monitoring stopped");

                });
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            (ApiClient as IDisposable)?.Dispose();
            ApiUpdateTimer?.Dispose();
            ServerProcess?.Dispose();
            base.Dispose();
        }

        #endregion

        #region Private API

        protected void NotifyDataUpdated()
        {
            _dataUpdated?.Invoke(this, EventArgs.Empty);
        }

        protected void EnterCriticalSection(Action action)
        {
            lock (_lockObject)
            {
                action();
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public abstract class GameServiceBase : IDisposable
    {
        #region Properties

        protected Timer? _updateTimer;
        private readonly TimeSpan _updateInterval;
        protected bool _isMonitoring = false;
        protected readonly object _lockObject = new object();
        protected readonly ApiClient ApiClient;

        /// <summary>
        /// Gets the base url of the Api used to get server data
        /// </summary>
        protected abstract string ApiBaseUrl { get; }

        /// <summary>
        /// Gets a value indicating whether a component is subscribed to server updates
        /// </summary>
        protected bool HasUpdateSubscribers => _dataUpdated != null;

        #endregion

        #region Events

        private event EventHandler? _dataUpdated;
        public event EventHandler DataUpdated
        {
            add
            {
                bool noSubscriptions = _dataUpdated == null;
                _dataUpdated += value;

                // Start active monitoring when first subscriber is added
                if (noSubscriptions)
                    StartMonitoring();
            }
            remove
            {
                _dataUpdated -= value;

                // Stop active monitoring when last subscriber is removed
                if (_dataUpdated == null)
                    StopMonitoring();
            }
        }

        #endregion

        #region Constructor

        public GameServiceBase()
        {
            // Initialize the API client once
            ApiClient = new ApiClient(ApiBaseUrl);
            _updateInterval = TimeSpan.FromSeconds(30);

            InitializeServer();
        }

        #endregion

        #region Public API

        public abstract Task RefreshServerData();

        
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
            (ApiClient as IDisposable)?.Dispose();
        }

        #endregion

        #region Private API

        protected abstract void InitializeServer();

        protected abstract Task OnServerStopped();

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

        protected void StartMonitoring()
        {
            if (!_isMonitoring)
            {
                // Do an initial refresh of server data
                _ = RefreshServerData();

                EnterCriticalSection(() =>
                {
                    // Start the timer for periodic updates
                    _updateTimer = new Timer(async _ => await RefreshServerData(), null, _updateInterval, _updateInterval);
                    _isMonitoring = true;
                    System.Diagnostics.Debug.WriteLine("Palworld monitoring started");
                });
            }
        }

        protected void StopMonitoring()
        {
            if (_isMonitoring)
            {
                EnterCriticalSection(() =>
                {
                    // Stop the timer
                    _updateTimer?.Dispose();
                    _updateTimer = null;
                    _isMonitoring = false;
                    System.Diagnostics.Debug.WriteLine("Palworld monitoring stopped");

                });
            }
        }

        #endregion
    }
}

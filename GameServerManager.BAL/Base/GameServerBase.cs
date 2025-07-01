using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public abstract class GameServerBase
    {
        #region Properties

        /// <summary>
        /// The internal server process
        /// </summary>
        [JsonIgnore]
        private ServerProcess ServerProcess { get; set; }

        /// <summary>
        /// The path to the executable file that will run the server
        /// </summary>
        [JsonIgnore]
        public abstract string ServerExecutablePath { get; }

        /// <summary>
        /// Occurs when console output has been received.  Used for real-time UI updates
        /// </summary>
        public event Action<string> OnConsoleOutputReceived
        {
            add { ServerProcess.OnConsoleOutputReceived += value; }
            remove { ServerProcess.OnConsoleOutputReceived -= value; }
        }

        #endregion

        #region Constructor

        public GameServerBase()
        {
            ServerProcess = new ServerProcess(ServerExecutablePath);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Starts the <see cref="BAL.ServerProcess"/>
        /// </summary>
        /// <returns></returns>
        public async Task StartServer()
        {
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

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public class ServerProcess : IDisposable
    {
        #region Properties
        
        /// <summary>
        /// The internal process
        /// </summary>
        private Process InternalProcess { get; set; }

        /// <summary>
        /// A value indicating whether the server is running
        /// </summary>
        public bool IsRunning => InternalProcess != null && !InternalProcess.HasExited;

        /// <summary>
        /// The path to the executable file for the server
        /// </summary>
        private string ExecutablePath { get; set; } 

        /// <summary>
        /// The arguments that will be passed to the executable
        /// </summary>
        private string Args { get; set; }

        /// <summary>
        /// The process cancellation token source
        /// </summary>
        private CancellationTokenSource _processCts;

        /// <summary>
        /// The console output
        /// </summary>
        private readonly StringBuilder ConsoleOutput = new StringBuilder();

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the server is stopped
        /// </summary>
        public event EventHandler ServerStopped;

        /// <summary>
        /// Occurs when console output has been received.  Used for real-time UI updates
        /// </summary>
        public event Action<string> OnConsoleOutputReceived;

        #endregion

        #region Constructor

        public ServerProcess(string path, string arguments = null)
        {
            ExecutablePath = path;
            Args = arguments;
        }

        #endregion

        #region Public API

        public async Task Start()
        {
            if (InternalProcess != null && !InternalProcess.HasExited)
            {
                throw new InvalidOperationException("Server is already running");
            }

            ConsoleOutput.Clear();
            _processCts = new CancellationTokenSource();

            var startInfo = new ProcessStartInfo
            {
                FileName = ExecutablePath,
                Arguments = Args,
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            InternalProcess = new Process { StartInfo = startInfo };

            // Set up output handlers
            InternalProcess.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    ConsoleOutput.AppendLine(args.Data);
                    OnConsoleOutputReceived?.Invoke(args.Data);
                }
            };

            InternalProcess.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    ConsoleOutput.AppendLine($"ERROR: {args.Data}");
                    OnConsoleOutputReceived?.Invoke($"ERROR: {args.Data}");
                }
            };

            // Start the process
            InternalProcess.Start();
            InternalProcess.BeginErrorReadLine();
            InternalProcess.BeginOutputReadLine();


            _ = Task.Run(async () =>
            {
                // Wait for the server process to exit indefinitely.  This will trigger if a console window 
                // for the server is open and gets closed
                try
                {
                    await Task.Run(() => InternalProcess.WaitForExit());
                }
                finally
                {
                    ServerStopped?.Invoke(this, EventArgs.Empty);
                }
            }, _processCts.Token);
        }

        public async Task Stop()
        {
            if (!IsRunning)
                return;

            try
            {
                // Try to gracefully shut down first
                SendCommand("shutdown");

                // Wait up to 10 seconds for graceful shutdown
                if (!InternalProcess.WaitForExit(10000))
                {
                    InternalProcess.Kill();
                    ConsoleOutput.AppendLine("Server forcefully terminated");
                    OnConsoleOutputReceived?.Invoke("Server forcefully terminated");
                }
                else
                {
                    ConsoleOutput.AppendLine("Server gracefully stopped");
                    OnConsoleOutputReceived?.Invoke("Server gracefully stopped");
                }
            }
            catch (Exception ex)
            {
                // If graceful shutdown fails, force kill
                try
                {
                    InternalProcess.Kill();
                    ConsoleOutput.AppendLine($"Error during shutdown, server forcefully terminated: {ex.Message}");
                    OnConsoleOutputReceived?.Invoke($"Error during shutdown, server forcefully terminated: {ex.Message}");
                }
                catch
                {
                    // Ignore errors during force kill
                }
            }
            finally
            {
                _processCts?.Cancel();
            }
        }

        public void SendCommand(string command)
        {
            if (!IsRunning)
                throw new InvalidOperationException("Server is not running");

            InternalProcess.StandardInput.WriteLine(command);
            ConsoleOutput.AppendLine($"Command sent: {command}");
            OnConsoleOutputReceived?.Invoke($"Command sent: {command}");
        }

        public void Dispose()
        {
            Stop();
            InternalProcess?.Dispose();
        }

        #endregion
    }
}

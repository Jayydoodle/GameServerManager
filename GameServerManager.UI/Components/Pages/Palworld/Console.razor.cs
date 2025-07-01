using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Text;

namespace GameServerManager.UI.Components.Pages.Palworld
{
    public partial class Console : PalworldPageBase
    {
        protected string _consoleText = "";
        protected string _command = "";
        protected bool _isServerRunning = false;
        protected StringBuilder _outputBuilder = new StringBuilder();
        protected ElementReference consoleRef;

        protected override void OnInitialized()
        {
            PalworldService.Server.OnConsoleOutputReceived += HandleConsoleOutput;
        }

        private async Task StartServer()
        {
            _isServerRunning = true;
            _outputBuilder.Clear();
            _consoleText = "";

            await PalworldService.Server.StartServer();
        }

        private async Task StopServer()
        {
            await PalworldService.Server.StopServer();
            _isServerRunning = false;
        }

        private void SendCommand()
        {
            // if (!string.IsNullOrWhiteSpace(_command))
            // {
            //     PalworldService.Server.SendCommand(_command);
            //     _command = "";
            // }
        }

        private void HandleConsoleOutput(string output)
        {
            _outputBuilder.AppendLine(output);

            // Update UI
            InvokeAsync(() =>
            {
                _consoleText = _outputBuilder.ToString();
                StateHasChanged();

                // Auto-scroll to bottom
                // Note: This requires JS interop in actual implementation
                ScrollToBottom();
            });
        }

        private void HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                SendCommand();
            }
        }

        private async Task ScrollToBottom()
        {
            await Task.Delay(10); // Small delay to ensure UI has updated
            // await JSRuntime.InvokeVoidAsync("scrollConsoleToBottom", consoleRef);
        }

        public void Dispose()
        {
            PalworldService.Server.OnConsoleOutputReceived -= HandleConsoleOutput;
        }
    }
}

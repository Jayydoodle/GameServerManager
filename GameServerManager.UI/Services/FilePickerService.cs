using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GameServerManager.UI.Services
{
    public class FilePickerService : IFilePickerService
    {
        private readonly ILogger<FilePickerService> _logger;

        public FilePickerService(ILogger<FilePickerService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> PickFolderAsync()
        {
            try
            {
                /// Use file picker and get directory (cross-platform approach)
                var options = new PickOptions()
                {
                    PickerTitle = "Select any file in the target folder",
                    FileTypes = null, // Allow all file types
                };

                var result = await FolderPicker.Default.PickAsync();

                if (result != null && result.IsSuccessful)
                {
                    _logger.LogInformation("Folder selected via file: {FolderPath}", result.Folder?.Path);
                    return result.Folder?.Path;
                }

                _logger.LogInformation("Folder selection cancelled");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error picking folder");
                return null;
            }
        }

        public async Task<string?> PickFileAsync(string title = "Select File", params string[] fileTypes)
        {
            try
            {
                // Ensure we're on the UI thread
                if (!Microsoft.Maui.Controls.Application.Current?.Dispatcher.IsDispatchRequired == false)
                {
                    return await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(async () =>
                    {
                        return await PickFileInternalAsync(title, fileTypes);
                    });
                }
                else
                {
                    return await PickFileInternalAsync(title, fileTypes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error picking file");
                return null;
            }
        }

        private async Task<string?> PickFileInternalAsync(string title, string[] fileTypes)
        {
            try
            {
                FilePickerFileType? customFileType = null;
                
                if (fileTypes?.Length > 0)
                {
                    customFileType = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.iOS, fileTypes },
                            { DevicePlatform.Android, fileTypes },
                            { DevicePlatform.WinUI, fileTypes },
                            { DevicePlatform.Tizen, fileTypes },
                            { DevicePlatform.macOS, fileTypes },
                        });
                }

                var options = new PickOptions()
                {
                    PickerTitle = title,
                    FileTypes = customFileType,
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    _logger.LogInformation("File selected: {FilePath}", result.FullPath);
                    return result.FullPath;
                }
                else
                {
                    _logger.LogInformation("File selection cancelled");
                    return null;
                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                _logger.LogError(comEx, "COM error during file selection. Error code: {ErrorCode}", comEx.HResult);
                throw new InvalidOperationException("File picker is not available or not properly initialized. Please try again.", comEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during file selection");
                throw;
            }
        }
    }
}

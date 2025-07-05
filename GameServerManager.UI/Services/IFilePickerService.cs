using System.Threading.Tasks;

namespace GameServerManager.UI.Services
{
    public interface IFilePickerService
    {
        Task<string?> PickFolderAsync();
        Task<string?> PickFileAsync(string title = "Select File", params string[] fileTypes);
    }
}

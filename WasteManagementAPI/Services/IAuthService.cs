using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Services
{
    public interface IAuthService
    {
        Task<UserTokenViewModel?> RegisterAsync(RegisterViewModel model);
        Task<UserTokenViewModel?> LoginAsync(LoginViewModel model);
    }
}

using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Services
{
    public interface IWasteBinService
    {
        Task<PagedResultViewModel<WasteBinResponseViewModel>> GetBinsAsync(int pageNumber, int pageSize, string? materialType);
        Task<WasteBinResponseViewModel?> GetBinByIdAsync(int id);
        Task<WasteBinResponseViewModel> CreateBinAsync(WasteBinRequestViewModel model);
        Task<WasteBinResponseViewModel?> UpdateFillLevelAsync(int id, double currentFillLevelKg);
        Task<bool> DeleteBinAsync(int id);
    }
}

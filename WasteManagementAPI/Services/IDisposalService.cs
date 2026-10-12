using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Services
{
    public interface IDisposalService
    {
        Task<DisposalResponseViewModel?> RecordDisposalAsync(int userId, DisposalRequestViewModel model);
        Task<PagedResultViewModel<DisposalResponseViewModel>> GetUserDisposalsAsync(int userId, int pageNumber, int pageSize);
        Task<PagedResultViewModel<DisposalResponseViewModel>> GetAllDisposalsAsync(int pageNumber, int pageSize);
    }
}

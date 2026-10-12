using Microsoft.EntityFrameworkCore;
using WasteManagementAPI.Data;
using WasteManagementAPI.Models;
using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Services
{
    public class WasteBinService : IWasteBinService
    {
        private readonly ApplicationDbContext _context;

        public WasteBinService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultViewModel<WasteBinResponseViewModel>> GetBinsAsync(int pageNumber, int pageSize, string? materialType)
        {
            var query = _context.WasteBins.AsQueryable();

            if (!string.IsNullOrWhiteSpace(materialType))
            {
                query = query.Where(b => b.MaterialType.ToLower() == materialType.ToLower());
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new WasteBinResponseViewModel
                {
                    Id = b.Id,
                    Location = b.Location,
                    MaterialType = b.MaterialType,
                    MaxCapacityKg = b.MaxCapacityKg,
                    CurrentFillLevelKg = b.CurrentFillLevelKg,
                    IsFullAlertTriggered = b.IsFullAlertTriggered
                })
                .ToListAsync();

            return new PagedResultViewModel<WasteBinResponseViewModel>(items, pageNumber, pageSize, totalItems);
        }

        public async Task<WasteBinResponseViewModel?> GetBinByIdAsync(int id)
        {
            var bin = await _context.WasteBins.FindAsync(id);
            if (bin == null) return null;

            return MapToResponse(bin);
        }

        public async Task<WasteBinResponseViewModel> CreateBinAsync(WasteBinRequestViewModel model)
        {
            var bin = new WasteBin
            {
                Location = model.Location,
                MaterialType = model.MaterialType,
                MaxCapacityKg = model.MaxCapacityKg,
                CurrentFillLevelKg = 0,
                IsFullAlertTriggered = false
            };

            _context.WasteBins.Add(bin);
            await _context.SaveChangesAsync();

            return MapToResponse(bin);
        }

        public async Task<WasteBinResponseViewModel?> UpdateFillLevelAsync(int id, double currentFillLevelKg)
        {
            var bin = await _context.WasteBins.FindAsync(id);
            if (bin == null) return null;

            // Validate that new fill level doesn't exceed physically impossible limits, though in some cases it could overflow.
            bin.CurrentFillLevelKg = currentFillLevelKg;
            
            // Check capacity threshold (alert triggered when >= 90%)
            var wasAlertTriggered = bin.IsFullAlertTriggered;
            bin.IsFullAlertTriggered = bin.CheckIfFull();

            _context.WasteBins.Update(bin);
            await _context.SaveChangesAsync();

            return MapToResponse(bin);
        }

        public async Task<bool> DeleteBinAsync(int id)
        {
            var bin = await _context.WasteBins.FindAsync(id);
            if (bin == null) return false;

            _context.WasteBins.Remove(bin);
            await _context.SaveChangesAsync();
            return true;
        }

        private static WasteBinResponseViewModel MapToResponse(WasteBin bin)
        {
            return new WasteBinResponseViewModel
            {
                Id = bin.Id,
                Location = bin.Location,
                MaterialType = bin.MaterialType,
                MaxCapacityKg = bin.MaxCapacityKg,
                CurrentFillLevelKg = bin.CurrentFillLevelKg,
                IsFullAlertTriggered = bin.IsFullAlertTriggered
            };
        }
    }
}

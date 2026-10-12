using Microsoft.EntityFrameworkCore;
using WasteManagementAPI.Data;
using WasteManagementAPI.Models;
using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Services
{
    public class DisposalService : IDisposalService
    {
        private readonly ApplicationDbContext _context;

        public DisposalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DisposalResponseViewModel?> RecordDisposalAsync(int userId, DisposalRequestViewModel model)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            var bin = await _context.WasteBins.FindAsync(model.WasteBinId);
            if (bin == null) return null;

            // Calculate recycling points: 10 points per kg of recyclables, 2 points per kg of organic/other.
            var isRecyclable = IsRecyclableMaterial(bin.MaterialType);
            var pointsPerKg = isRecyclable ? 10 : 2;
            var pointsEarned = (int)Math.Round(model.WeightKg * pointsPerKg);

            // Transaction or sequential update
            var disposal = new DisposalRecord
            {
                UserId = user.Id,
                WasteBinId = bin.Id,
                WeightKg = model.WeightKg,
                PointsEarned = pointsEarned,
                Timestamp = DateTime.UtcNow
            };

            // Update user points
            user.RecyclingPoints += pointsEarned;

            // Update bin fill level
            bin.CurrentFillLevelKg += model.WeightKg;
            bin.IsFullAlertTriggered = bin.CheckIfFull();

            _context.DisposalRecords.Add(disposal);
            _context.Users.Update(user);
            _context.WasteBins.Update(bin);

            await _context.SaveChangesAsync();

            return new DisposalResponseViewModel
            {
                Id = disposal.Id,
                UserId = user.Id,
                UserName = user.Name,
                WasteBinId = bin.Id,
                BinLocation = bin.Location,
                MaterialType = bin.MaterialType,
                WeightKg = disposal.WeightKg,
                PointsEarned = disposal.PointsEarned,
                Timestamp = disposal.Timestamp
            };
        }

        public async Task<PagedResultViewModel<DisposalResponseViewModel>> GetUserDisposalsAsync(int userId, int pageNumber, int pageSize)
        {
            var query = _context.DisposalRecords
                .Include(d => d.User)
                .Include(d => d.WasteBin)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Timestamp);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(d => MapToResponse(d))
                .ToListAsync();

            return new PagedResultViewModel<DisposalResponseViewModel>(items, pageNumber, pageSize, totalItems);
        }

        public async Task<PagedResultViewModel<DisposalResponseViewModel>> GetAllDisposalsAsync(int pageNumber, int pageSize)
        {
            var query = _context.DisposalRecords
                .Include(d => d.User)
                .Include(d => d.WasteBin)
                .OrderByDescending(d => d.Timestamp);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(d => MapToResponse(d))
                .ToListAsync();

            return new PagedResultViewModel<DisposalResponseViewModel>(items, pageNumber, pageSize, totalItems);
        }

        private static bool IsRecyclableMaterial(string materialType)
        {
            var lower = materialType.ToLower();
            return lower == "plastic" || lower == "glass" || lower == "paper" || lower == "metal" || lower == "electronic";
        }

        private static DisposalResponseViewModel MapToResponse(DisposalRecord d)
        {
            return new DisposalResponseViewModel
            {
                Id = d.Id,
                UserId = d.UserId,
                UserName = d.User != null ? d.User.Name : "Unknown",
                WasteBinId = d.WasteBinId,
                BinLocation = d.WasteBin != null ? d.WasteBin.Location : "Unknown",
                MaterialType = d.WasteBin != null ? d.WasteBin.MaterialType : "Unknown",
                WeightKg = d.WeightKg,
                PointsEarned = d.PointsEarned,
                Timestamp = d.Timestamp
            };
        }
    }
}

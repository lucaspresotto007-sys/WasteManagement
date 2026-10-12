using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.Models
{
    public class DisposalRecord
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int WasteBinId { get; set; }
        public WasteBin? WasteBin { get; set; }

        [Required]
        public double WeightKg { get; set; }

        public int PointsEarned { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

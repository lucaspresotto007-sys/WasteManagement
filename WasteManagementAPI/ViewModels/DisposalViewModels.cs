using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.ViewModels
{
    public class DisposalRequestViewModel
    {
        [Required]
        public int WasteBinId { get; set; }

        [Required]
        [Range(0.01, 1000.0)]
        public double WeightKg { get; set; }
    }

    public class DisposalResponseViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int WasteBinId { get; set; }
        public string BinLocation { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public int PointsEarned { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

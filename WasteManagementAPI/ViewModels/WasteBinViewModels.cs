using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.ViewModels
{
    public class WasteBinRequestViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string MaterialType { get; set; } = string.Empty;

        [Required]
        [Range(1.0, 10000.0)]
        public double MaxCapacityKg { get; set; }
    }

    public class WasteBinFillLevelUpdateViewModel
    {
        [Required]
        [Range(0.0, 10000.0)]
        public double CurrentFillLevelKg { get; set; }
    }

    public class WasteBinResponseViewModel
    {
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public double MaxCapacityKg { get; set; }
        public double CurrentFillLevelKg { get; set; }
        public bool IsFullAlertTriggered { get; set; }
        public double FillPercentage => Math.Round((CurrentFillLevelKg / MaxCapacityKg) * 100, 2);
    }
}

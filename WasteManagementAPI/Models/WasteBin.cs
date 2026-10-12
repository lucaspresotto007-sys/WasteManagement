using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.Models
{
    public class WasteBin
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string MaterialType { get; set; } = string.Empty; // Plastic, Paper, Glass, Metal, Organic, Electronic

        [Required]
        public double MaxCapacityKg { get; set; }

        [Required]
        public double CurrentFillLevelKg { get; set; }

        public bool IsFullAlertTriggered { get; set; }

        // Logic check: alert is triggered when current fill level is >= 90% of max capacity
        public bool CheckIfFull()
        {
            return CurrentFillLevelKg >= (MaxCapacityKg * 0.9);
        }
    }
}

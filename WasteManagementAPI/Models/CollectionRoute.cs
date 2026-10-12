using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.Models
{
    public class CollectionRoute
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string RouteName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
    }
}

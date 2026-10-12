using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.ViewModels
{
    public class CollectionRouteRequestViewModel
    {
        [Required]
        [MaxLength(100)]
        public string RouteName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledDate { get; set; }
    }

    public class CollectionRouteStatusUpdateViewModel
    {
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
    }

    public class CollectionRouteResponseViewModel
    {
        public int Id { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

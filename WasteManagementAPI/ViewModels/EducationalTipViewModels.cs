using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.ViewModels
{
    public class EducationalTipRequestViewModel
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string MaterialCategory { get; set; } = string.Empty;
    }

    public class EducationalTipResponseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MaterialCategory { get; set; } = string.Empty;
    }
}

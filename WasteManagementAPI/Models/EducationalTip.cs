using System.ComponentModel.DataAnnotations;

namespace WasteManagementAPI.Models
{
    public class EducationalTip
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string MaterialCategory { get; set; } = string.Empty; // General, Plastic, Glass, Paper, Metal, Electronic, Organic
    }
}

using System.ComponentModel.DataAnnotations;

namespace ImageEventApi.Models
{
    public record ImageEvent
    {
        public string ImageUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}

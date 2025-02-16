using System.ComponentModel.DataAnnotations;

namespace ImageEventApi.Core.Domain.Models
{
    public record ImageEvent
    {
        public string ImageUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}

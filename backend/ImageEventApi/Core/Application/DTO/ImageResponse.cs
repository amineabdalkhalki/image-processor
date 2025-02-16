using ImageEventApi.Core.Domain.Models;

namespace ImageEventApi.Core.Application.DTO
{
    public record ImageResponse
    {
        public ImageEvent? Image { get; set; }
        public int LastHourCount { get; set; }
    }
}

using ImageEventApi.Models;

namespace ImageEventApi.DTO
{
    public record ImageResponse
    {
        public ImageEvent? Image { get; set; }
        public int LastHourCount { get; set; }
    }
}

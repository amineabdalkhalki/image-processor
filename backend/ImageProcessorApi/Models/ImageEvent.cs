namespace ImageProcessorApi.Models;
public record ImageEvent(
    string ImageUrl,
    string Description,
    DateTime? ReceivedAt = null);
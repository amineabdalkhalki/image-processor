using ImageProcessorApi.Models;
namespace ImageProcessorApi.Services;

public interface IImageStorage
{
    ImageEvent? LatestImage { get; }
    int LastHourCount { get; }
    void Update(ImageEvent imageEvent);
}
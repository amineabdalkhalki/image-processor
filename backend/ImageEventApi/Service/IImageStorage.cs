using ImageEventApi.Models;
namespace ImageEventApi.Services;

public interface IImageStorage
{
    ImageEvent? LatestImage { get; }
    int LastHourCount { get; }
    void Update(ImageEvent imageEvent);
}
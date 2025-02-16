using ImageEventApi.Models;
namespace ImageEventApi.Services;

public interface IImageStorageService
{
    ImageEvent? LatestImage { get; }
    int LastHourCount { get; }
    void Update(ImageEvent imageEvent);
}
using ImageEventApi.Core.Domain.Models;

namespace ImageEventApi.Core.Domain.Interfaces;

public interface IImageStorageService
{
    ImageEvent? LatestImage { get; }
    int LastHourCount { get; }
    void Update(ImageEvent imageEvent);
}
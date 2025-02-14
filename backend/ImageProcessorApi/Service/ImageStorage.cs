using System.Collections.Concurrent;
using ImageProcessorApi.Services;
namespace ImageProcessorApi.Models;


public class ImageStorage : IImageStorage
{
    private readonly ConcurrentBag<DateTime> _eventTimestamps = new();
    private ImageEvent? _latestImage;

    public ImageEvent? LatestImage => _latestImage;

    public int LastHourCount => _eventTimestamps
        .Count(t => t > DateTime.UtcNow.AddHours(-1));

    public void Update(ImageEvent imageEvent)
    {
        _latestImage = imageEvent;
        _eventTimestamps.Add(DateTime.UtcNow);
        DeleteOldEvents();
    }

    private void DeleteOldEvents()
    {
        var cutoff = DateTime.UtcNow.AddHours(-1);
        while (_eventTimestamps.TryPeek(out var timestamp))
        {
            if (timestamp < cutoff)
            {
                _eventTimestamps.TryTake(out _);
            }
            else
            {
                break;
            }
        }
    }
}
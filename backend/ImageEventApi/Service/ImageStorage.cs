using ImageEventApi.Models;
using ImageEventApi.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ImageProcessorApi.Services;

public class ImageStorage : IImageStorage
{
    private const string LatestImageKey = "LatestImage";
    private const string EventTimestampsKey = "EventTimestamps";

    private readonly IMemoryCache _cache;

    private const int CacheExpirationHours = 1;

    public ImageStorage(IMemoryCache cache)
    {
        _cache = cache;

        // Set cache options
        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(CacheExpirationHours) // Automatically remove items after 1 hour of inactivity
        };

        // Initialize cache with default values
        _cache.Set(LatestImageKey, new ImageEvent(), cacheOptions);
        _cache.Set(EventTimestampsKey, new List<DateTime>(), cacheOptions);
    }

    public ImageEvent? LatestImage
    {
        get => _cache.Get<ImageEvent?>(LatestImageKey);
    }

    public int LastHourCount
    {
        get
        {
            var timestamps = _cache.Get<List<DateTime>>(EventTimestampsKey) ?? new List<DateTime>();
            return timestamps.Count(t => t > DateTime.UtcNow.AddHours(-1));
        }
    }

    public void Update(ImageEvent imageEvent)
    {
        // Update latest image
        _cache.Set(LatestImageKey, imageEvent);

        // Update event timestamps
        var timestamps = _cache.Get<List<DateTime>>(EventTimestampsKey) ?? new List<DateTime>();
        timestamps.Add(DateTime.UtcNow);

        // Prune old events
        timestamps = timestamps.Where(t => t > DateTime.UtcNow.AddHours(-1)).ToList();

        // Store updated timestamps
        _cache.Set(EventTimestampsKey, timestamps);
    }
}
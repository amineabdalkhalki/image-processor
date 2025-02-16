using ImageEventApi.Models;
using ImageEventApi.Services;

namespace ImageProcessorApi.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly object _storageLock = new object();
        private ImageEvent? _latestImage;
        private List<DateTime> _eventTimestamps = new List<DateTime>();

        public ImageEvent? LatestImage
        {
            get
            {
                lock (_storageLock)
                {
                    return _latestImage;
                }
            }
        }

        public int LastHourCount
        {
            get
            {
                lock (_storageLock)
                {
                    var cutoff = DateTime.UtcNow.AddHours(-1);
                    return _eventTimestamps.Count(t => t > cutoff);
                }
            }
        }

        public void Update(ImageEvent imageEvent)
        {
            lock (_storageLock)
            {
                // Update latest image
                _latestImage = imageEvent;

                // Add new timestamp
                _eventTimestamps.Add(DateTime.UtcNow);

                // Prune timestamps older than 1 hour
                var cutoff = DateTime.UtcNow.AddHours(-1);
                _eventTimestamps = _eventTimestamps
                    .Where(t => t > cutoff)
                    .ToList();
            }
        }

        // Add to ImageStorage class for testability
        internal void AddTimestampForTesting(DateTime timestamp)
        {
            lock (_storageLock)
            {
                _eventTimestamps.Add(timestamp);
            }
        }
    }
}
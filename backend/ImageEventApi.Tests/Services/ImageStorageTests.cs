using ImageEventApi.Models;
using ImageProcessorApi.Services;
using Xunit;

namespace ImageProcessorApi.Tests.Services
{
    public class ImageStorageTests
    {
        private readonly ImageStorageService _storage;

        public ImageStorageTests()
        {
            _storage = new ImageStorageService();
        }

        [Fact]
        public void LatestImage_Initially_ShouldBeNull()
        {
            // Act & Assert
            Assert.Null(_storage.LatestImage);
        }

        [Fact]
        public void LatestImage_AfterUpdate_ShouldReturnLastImage()
        {
            // Arrange
            var imageEvent = new ImageEvent { ImageUrl = "test.jpg" };

            // Act
            _storage.Update(imageEvent);

            // Assert
            Assert.Same(imageEvent, _storage.LatestImage);
        }

        [Fact]
        public void LastHourCount_WithNoEvents_ShouldReturnZero()
        {
            // Act & Assert
            Assert.Equal(0, _storage.LastHourCount);
        }

        [Fact]
        public void LastHourCount_AfterSingleUpdate_ShouldReturnOne()
        {
            // Arrange
            _storage.Update(new ImageEvent());

            // Act & Assert
            Assert.Equal(1, _storage.LastHourCount);
        }

        [Fact]
        public void Update_ShouldAddTimestampAndPruneOld()
        {
            // Arrange
            var oldTimestamp = DateTime.UtcNow.AddHours(-2);
            var newTimestamp = DateTime.UtcNow;

            // Add an old event (simulate via reflection)
            AddTimestampDirectly(oldTimestamp);

            // Act - add new event
            _storage.Update(new ImageEvent());

            // Assert
            Assert.Equal(1, _storage.LastHourCount); // Only the new event remains
        }

        [Fact]
        public void Update_ShouldMaintainLatestImage()
        {
            // Arrange
            var firstImage = new ImageEvent { ImageUrl = "first.jpg" };
            var secondImage = new ImageEvent { ImageUrl = "second.jpg" };

            // Act
            _storage.Update(firstImage);
            _storage.Update(secondImage);

            // Assert
            Assert.Same(secondImage, _storage.LatestImage);
        }

        [Fact]
        public void Concurrency_ShouldMaintainConsistency()
        {
            // Arrange
            var iterations = 1000;
            var count = 0;

            // Act
            Parallel.For(0, iterations, i => {
                _storage.Update(new ImageEvent());
                Interlocked.Increment(ref count);
            });

            // Assert
            Assert.Equal(count, _storage.LastHourCount);
            Assert.NotNull(_storage.LatestImage);
        }

        // Helper method to add timestamps directly for testing
        private void AddTimestampDirectly(DateTime timestamp)
        {
            var field = typeof(ImageStorageService).GetField("_eventTimestamps",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            var timestamps = (List<DateTime>)field.GetValue(_storage);
            timestamps.Add(timestamp);
            field.SetValue(_storage, timestamps);
        }
    }
}